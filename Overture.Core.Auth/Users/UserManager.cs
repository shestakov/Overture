using System;
using Overture.Core.Auth.Passwords;
using Overture.Core.Auth.Users.Exceptions;
using Overture.Core.Auth.Users.Storage;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users
{
	public class UserManager<TUser, TUserActivationRequest, TCreateUserForm, TUpdateUserForm> :
		IUserManager<TUser, TUserActivationRequest, TCreateUserForm, TUpdateUserForm>
		where TUser : class, IUser
		where TUserActivationRequest : class, IUserActivationRequest, new()
		where TCreateUserForm : class, IUserForm
	{
		private readonly TimeSpan activateUserRequestValidityPeriod = new TimeSpan(1, 0, 0, 0);
		private readonly IUserStorage<TUser> userStorage;
		private readonly IUserActivationRequestDataStorage<TUserActivationRequest> userActivationRequestDataStorage;
		private readonly IAuthEmailSender authEmailSender;
		private readonly IPasswordHasher passwordHasher;
		private readonly IPasswordValidator passwordValidator;
		private readonly IUserProcessor<TUser, TCreateUserForm, TUpdateUserForm> userProcessor;

		public UserManager(IUserStorage<TUser> userStorage, IPasswordHasher passwordHasher,
			IAuthEmailSender authEmailSender, IUserProcessor<TUser, TCreateUserForm, TUpdateUserForm> userProcessor, IPasswordValidator passwordValidator, IUserActivationRequestDataStorage<TUserActivationRequest> userActivationRequestDataStorage)
		{
			this.userStorage = userStorage;
			this.passwordHasher = passwordHasher;
			this.authEmailSender = authEmailSender;
			this.userProcessor = userProcessor;
			this.passwordValidator = passwordValidator;
			this.userActivationRequestDataStorage = userActivationRequestDataStorage;
		}

		public TUser CreateUserAndActivationRequest(TCreateUserForm form, Uri activateUserUrl)
		{
			var login = form.Login.Trim();

			if(!EmailValidator.Validate(login))
				throw new IncorrectEmailException();

			var password = form.Password;

			if(!passwordValidator.Validate(password))
				throw new WeakPasswordException();

			var existingUser = userStorage.FindUserByLogin(login);
			if(existingUser != null)
			{
				throw new LoginAlreadyExistsException(login);
			}

			var userId = Guid.NewGuid();
			var user = userProcessor.MakeUser(form, userId);
			SetUserPassword(user, password);
			userStorage.CreateUser(user);
			SendUserActivationRequest(user, activateUserUrl);
			return userStorage.FindUser(userId);
		}

		public TUser CreateActiveUser(TCreateUserForm form)
		{
			EmailValidator.Validate(form.Login);

			var existingUser = userStorage.FindUserByLogin(form.Login);
			if(existingUser != null)
			{
				if(existingUser.IsActive)
					throw new LoginAlreadyExistsException(form.Login);
				throw new LoginAlreadyExistsButNeedsActivationException(form.Login);
			}

			var userId = Guid.NewGuid();
			var utcNow = DateTimeOffset.UtcNow;

			var user = userProcessor.MakeUser(form, userId);
			user.DateTimeActivated = utcNow;
			SetUserPassword(user, form.Password);
			userStorage.CreateUser(user);
			return userStorage.FindUser(userId);
		}

		public UserActivationRequestValidationResult<TUserActivationRequest, TUser> ValidateUserActivationRequest(
			Guid userActivationRequestId)
		{
			return ValidateUserActivationRequestInternal(userActivationRequestId);
		}

		public TUser ActivateUser(Guid userActivationRequestId)
		{
			var result = ValidateUserActivationRequestInternal(userActivationRequestId);
			var request = result.UserActivationRequest;
			var user = result.User;
			var userId = user.UserId;
			var utcNow = DateTimeOffset.UtcNow;

			request.ActivationDateTime = utcNow;
			userActivationRequestDataStorage.UpdateActivateUserRequest(request);

			user.DateTimeActivated = utcNow;
			userStorage.UpdateUser(user);
			return userStorage.GetUser(userId);
		}

		public void UpdateUserDetails(Guid userId, TUpdateUserForm form)
		{
			var user = userStorage.FindUser(userId);
			if(user == null)
				throw new UserNotFoundException(userId);

			userProcessor.Update(user, form);

			userStorage.UpdateUser(user);
		}

		private UserActivationRequestValidationResult<TUserActivationRequest, TUser> ValidateUserActivationRequestInternal(
			Guid userActivationRequestId)
		{
			var userActivationRequest = userActivationRequestDataStorage.FindUserActivationRequest(userActivationRequestId);

			if(userActivationRequest == null)
				throw new UserActivationRequestNotFoundException(userActivationRequestId);

			if(userActivationRequest.IsUsed)
				throw new UserActivationRequestAlreadyCompleteException(userActivationRequestId);

			if(userActivationRequest.Expired)
				throw new UserActivationRequestExpiredException(userActivationRequestId);

			var user = userStorage.FindUser(userActivationRequest.DraftUserId);

			if(user == null)
				throw new UserNotFoundException(userActivationRequest.DraftUserId);

			if(user.IsActive)
				throw new UserAlreadyActiveException(userActivationRequest.DraftUserId);

			return new UserActivationRequestValidationResult<TUserActivationRequest, TUser>(userActivationRequest, user);
		}

		private void SendUserActivationRequest(TUser user, Uri activateUserUrl)
		{
			var userId = user.UserId;
			if(user.IsActive)
				throw new UserAlreadyActiveException(userId);

			var requestId = Guid.NewGuid();
			var expirationDateTime = DateTimeOffset.UtcNow.Add(activateUserRequestValidityPeriod);
			var request = new TUserActivationRequest
			{
				RequestId = requestId,
				DraftUserId = userId,
				ExpirationDateTime = expirationDateTime
			};
			userActivationRequestDataStorage.CreateActivateUserRequest(request);
			var builder = new UriBuilder(activateUserUrl);
			if(builder.Query.Length > 1)
				builder.Query = builder.Query.Substring(1) + "&" + "requestId=" + requestId;
			else
				builder.Query = "requestId=" + requestId;

			authEmailSender.SendActivateUserEmail(user.Login, builder.Uri.ToString());
		}

		private void SetUserPassword(TUser user, string newPassword)
		{
			user.PasswordSalt = passwordHasher.GenerateSalt();
			user.PasswordHash = passwordHasher.HashPassword(newPassword, user.PasswordSalt);
		}
	}
}