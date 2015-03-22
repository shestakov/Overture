using System;
using Overture.Core.Auth.Users;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Tenants
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class TenantManager<TOrganization, TOrganizationCreateForm, TOrganizationUpdateForm, TOrganizationUser, TUserInvitation, TUser, TCreateUserForm, TUpdateUserForm, TUserActivationRequest, TChangePasswordRequest> 
		: ITenantManager<TOrganization, TOrganizationCreateForm, TOrganizationUpdateForm, TOrganizationUser, TCreateUserForm> 
		where TOrganization: class
		where TUserInvitation: class, IUserInvitation, new()
		where TCreateUserForm: class, IUserForm, new()
		where TOrganizationUser: IOrganizationUser, new()
		where TUser: class, IUser
		where TUserActivationRequest: class, IUserActivationRequest, new()
		where TChangePasswordRequest: class, IChangePasswordRequest
	{
		private readonly ITenantDataStorage<TOrganization, TOrganizationUser, TUserInvitation, TUser> tenantDataStorage;
		private readonly IOrganizationProcessor<TOrganization, TOrganizationCreateForm, TOrganizationUpdateForm> organizationProcessor;
		private readonly IUserManager<TUser, TUserActivationRequest, TCreateUserForm, TUpdateUserForm, TChangePasswordRequest> userManager;
		private readonly IAuthDataStorage<TUser, TUserActivationRequest, TChangePasswordRequest> authDataStorage;
		private readonly IAuthEmailSender authEmailSender;

		public TenantManager(ITenantDataStorage<TOrganization, TOrganizationUser, TUserInvitation, TUser> tenantDataStorage, IOrganizationProcessor<TOrganization, TOrganizationCreateForm, TOrganizationUpdateForm> organizationProcessor, IUserManager<TUser, TUserActivationRequest, TCreateUserForm, TUpdateUserForm, TChangePasswordRequest> userManager, IAuthDataStorage<TUser, TUserActivationRequest, TChangePasswordRequest> authDataStorage, IAuthEmailSender authEmailSender)
		{
			this.tenantDataStorage = tenantDataStorage;
			this.organizationProcessor = organizationProcessor;
			this.userManager = userManager;
			this.authDataStorage = authDataStorage;
			this.authEmailSender = authEmailSender;
		}

		public TOrganization CreateOrganization(TOrganizationCreateForm form, Guid userId)
		{
			var organizationId = Guid.NewGuid();
			var organization = organizationProcessor.Create(form, organizationId);
			tenantDataStorage.CreateOrganization(organization);
			
			var organizationUser = new TOrganizationUser
			{
				OrganizationUserId = Guid.NewGuid(),
				OrganizationId = organizationId,
				UserId = userId,
				DateTimeCreated = DateTimeOffset.UtcNow,
				IsAdministrator = true
			};
			tenantDataStorage.CreateOrganizationUser(organizationUser);

			return tenantDataStorage.FindOrganization(organizationId);
		}

		public void UpdateOrganization(Guid organizationId, TOrganizationUpdateForm form, Guid userId)
		{
			var organization = tenantDataStorage.FindOrganization(organizationId);
			if (organization == null)
				throw new OrganizationNotFoundException(organizationId);

			organizationProcessor.Update(organization, form);
			tenantDataStorage.UpdateOrganization(organization);
		}

		public void SendUserInvitation(string email, Guid organizationId, Guid invitingUserId, string organizationTitle,
			Uri acceptInvitationUrl)
		{
			EmailValidator.Validate(email);

			var existingUser = authDataStorage.FindUserByLogin(email);
			
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

			tenantDataStorage.CreateUserInvitation(userInvitation);

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
			tenantDataStorage.UpdateUserInvitation(userInvitation);

			tenantDataStorage.CreateOrganizationUser(organizationUser);
			return tenantDataStorage.FindOrganizationUser(organizationUserId);
		}

		public TOrganizationUser AcceptExistingUserInvitation(Guid userInvitationId)
		{
			var userInvitation = GetUserInvitation(userInvitationId);
			var utcNow = DateTimeOffset.UtcNow;


			var user = userInvitation.InvitedUserId.HasValue
				? authDataStorage.FindUser(userInvitation.InvitedUserId.Value)
				: authDataStorage.FindUserByLogin(userInvitation.Email);

			if(user == null)
				throw new Exception("Invited user not found");

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
			tenantDataStorage.UpdateUserInvitation(userInvitation);

			tenantDataStorage.CreateOrganizationUser(organizationUser);
			return tenantDataStorage.FindOrganizationUser(organizationUserId);
		}

		public void CancelUserInvitation(Guid userInvitationId)
		{
			var userInvitation = GetUserInvitation(userInvitationId);

			var utcNow = DateTimeOffset.UtcNow;

			userInvitation.Status = UserInvitationStatus.Cancelled;
			userInvitation.DateTimeProcessed = utcNow;
			tenantDataStorage.UpdateUserInvitation(userInvitation);
		}

		private TUserInvitation GetUserInvitation(Guid userInvitationId)
		{
			var userInvitation = tenantDataStorage.FindUserInvitation(userInvitationId);
			if (userInvitation == null)
				throw new ObjectNotFoundException(string.Format("User Invitation {0} not found", userInvitationId));

			if (userInvitation.Status == UserInvitationStatus.Cancelled)
				throw new ClientSideErrorMessageException("»звините, это приглашение уже недействительно",
					string.Format("User Invitation {0} cancelled", userInvitationId));

			if (userInvitation.Status == UserInvitationStatus.Accepted)
				throw new ClientSideErrorMessageException("»звините, это приглашение уже прин€то",
					string.Format("User Invitation {0} already accepted", userInvitationId));

			if (userInvitation.Expired)
				throw new ClientSideErrorMessageException("»звините, срок действи€ этого приглашени€ истек",
					string.Format("User Invitation {0} expired", userInvitationId));
			return userInvitation;
		}
	}
}