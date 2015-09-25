using System;
using System.Linq;
using Overture.Core.Auth.Users;
using Overture.Core.Auth.Users.Storage;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Organizations
{
	public class UserInvitationManager<TOrganizationUser, TUserInvitation, TUser, TCreateUserForm, TUpdateUserForm, TUserActivationRequest> : IUserInvitationManager<TOrganizationUser, TCreateUserForm> 
		where TUserInvitation: class, IUserInvitation, new()
		where TCreateUserForm: class, IUserForm, new()
		where TOrganizationUser: IOrganizationUser, new()
		where TUser: class, IUser
		where TUserActivationRequest: class, IUserActivationRequest, new()
	{
		private readonly IUserInvitationStorage<TUserInvitation> userInvitationStorage;
		private readonly IOrganizationUserStorage<TOrganizationUser, TUser> organizationUserStorage;
		private readonly IUserManager<TUser, TUserActivationRequest, TCreateUserForm, TUpdateUserForm> userManager;
		private readonly IUserStorage<TUser> userStorage;
		private readonly IAuthEmailSender authEmailSender;

		public UserInvitationManager(IUserInvitationStorage<TUserInvitation> userInvitationStorage, IUserManager<TUser, TUserActivationRequest, TCreateUserForm, TUpdateUserForm> userManager, IUserStorage<TUser> userStorage, IAuthEmailSender authEmailSender, IOrganizationUserStorage<TOrganizationUser, TUser> organizationUserStorage)
		{
			this.userInvitationStorage = userInvitationStorage;
			this.userManager = userManager;
			this.userStorage = userStorage;
			this.authEmailSender = authEmailSender;
			this.organizationUserStorage = organizationUserStorage;
		}

		public void SendUserInvitation(string email, Guid organizationId, Guid invitingUserId, string organizationTitle,
			Uri acceptInvitationUrl)
		{
			EmailValidator.Validate(email);

			var existingUser = userStorage.FindUserByLogin(email);
			
			var userInvitationId = Guid.NewGuid();
			var invitedUserId = existingUser != null ? existingUser.UserId : (Guid?)null;
			var utcNow = DateTimeOffset.UtcNow;

			var userInvitation = new TUserInvitation
			{
				UserInvitationId = userInvitationId,
				OrganizationId = organizationId,
				InivitingUserId = invitingUserId,
				InvitedUserId = invitedUserId,
				Email = email,
				DateTimeCreated = utcNow,
				Status = UserInvitationStatus.Created
			};

			userInvitationStorage.CreateUserInvitation(userInvitation);

			var builder = new UriBuilder(acceptInvitationUrl);
			if (builder.Query.Length > 1)
				builder.Query = builder.Query.Substring(1) + "&" + "invitationId=" + userInvitationId;
			else
				builder.Query = "invitationId=" + userInvitationId;
			var invitationUrl = builder.Uri.ToString();

			authEmailSender.SendUserInvitation(email, invitationUrl, organizationTitle);
		}

		public TOrganizationUser AcceptUserInvitation(Guid userInvitationId, TCreateUserForm form)
		{
			var userInvitation = GetUserInvitation(userInvitationId);
			form.Login = userInvitation.Email;
			var user = userManager.CreateActiveUser(form);
			var utcNow = DateTimeOffset.UtcNow;

			var organizationUserId = Guid.NewGuid();
			var organizationUser = new TOrganizationUser
			{
				OrganizationUserId = organizationUserId,
				OrganizationId = userInvitation.OrganizationId,
				UserId = user.UserId,
				DateTimeCreated = utcNow
			};

			userInvitation.Status = UserInvitationStatus.Accepted;
			userInvitation.DateTimeProcessed = utcNow;
			userInvitationStorage.UpdateUserInvitation(userInvitation);

			organizationUserStorage.CreateOrganizationUser(organizationUser);
			return organizationUserStorage.FindOrganizationUser(organizationUserId);
		}

		public TOrganizationUser AcceptExistingUserInvitation(Guid userInvitationId)
		{
			var userInvitation = GetUserInvitation(userInvitationId);
			var utcNow = DateTimeOffset.UtcNow;
			
			var user = userInvitation.InvitedUserId.HasValue
				? userStorage.FindUser(userInvitation.InvitedUserId.Value)
				: userStorage.FindUserByLogin(userInvitation.Email);

			if (user == null)
				throw new Exception("Invited user not found");

			var organizationUserId = Guid.NewGuid();
			var existingOrganizationUser = organizationUserStorage
				.FindOrganizationUsersByUser(user.UserId)
				.FirstOrDefault(u => u.OrganizationId == organizationUserId && u.DateTimeDeleted == null);

			if (existingOrganizationUser != null)
				throw new ClientSideErrorMessageException("Извините, это приглашение уже принято", $"User {user.UserId} already added to Organization {organizationUserId} (Invitation {userInvitationId})");

			var organizationUser = new TOrganizationUser
			{
				OrganizationUserId = organizationUserId,
				OrganizationId = userInvitation.OrganizationId,
				UserId = user.UserId,
				DateTimeCreated = utcNow
			};

			userInvitation.Status = UserInvitationStatus.Accepted;
			userInvitation.DateTimeProcessed = utcNow;
			userInvitationStorage.UpdateUserInvitation(userInvitation);

			organizationUserStorage.CreateOrganizationUser(organizationUser);
			return organizationUserStorage.FindOrganizationUser(organizationUserId);
		}

		public void CancelUserInvitation(Guid userInvitationId)
		{
			var userInvitation = GetUserInvitation(userInvitationId);

			var utcNow = DateTimeOffset.UtcNow;

			userInvitation.Status = UserInvitationStatus.Cancelled;
			userInvitation.DateTimeProcessed = utcNow;
			userInvitationStorage.UpdateUserInvitation(userInvitation);
		}

		private TUserInvitation GetUserInvitation(Guid userInvitationId)
		{
			var userInvitation = userInvitationStorage.FindUserInvitation(userInvitationId);
			if (userInvitation == null)
				throw new ObjectNotFoundException(string.Format("User Invitation {0} not found", userInvitationId));

			if (userInvitation.Status == UserInvitationStatus.Cancelled)
				throw new ClientSideErrorMessageException("Извините, это приглашение уже недействительно",
					string.Format("User Invitation {0} cancelled", userInvitationId));

			if (userInvitation.Status == UserInvitationStatus.Accepted)
				throw new ClientSideErrorMessageException("Извините, это приглашение уже принято",
					string.Format("User Invitation {0} already accepted", userInvitationId));

			if (userInvitation.Expired)
				throw new ClientSideErrorMessageException("Извините, срок действия этого приглашения истек",
					string.Format("User Invitation {0} expired", userInvitationId));
			return userInvitation;
		}
	}
}