using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users
{
	public class UserManager<TUser, TUserActivationRequest, TChangePasswordRequest, TCreateUserForm, TUpdateUserForm> :
		IUserManager<TUser, TUserActivationRequest, TCreateUserForm, TUpdateUserForm, TChangePasswordRequest>
		where TUser : class, IUser
		where TUserActivationRequest : class, IUserActivationRequest, new()
		where TChangePasswordRequest : class, IChangePasswordRequest, new()
		where TCreateUserForm : class, IUserForm
	{
		private readonly TimeSpan activateUserRequestValidityPeriod = new TimeSpan(1, 0, 0, 0);
		private readonly IAuthDataStorage<TUser, TUserActivationRequest, TChangePasswordRequest> authDataStorage;
		private readonly IAuthEmailSender authEmailSender;
		private readonly TimeSpan changePasswordRequestValidityPeriod = new TimeSpan(0, 1, 0, 0);
		private readonly IPasswordHasher passwordHasher;
		private readonly IPasswordValidator passwordValidator;
		private readonly IUserProcessor<TUser, TCreateUserForm, TUpdateUserForm> userProcessor;

		public UserManager(IAuthDataStorage<TUser, TUserActivationRequest, TChangePasswordRequest> authDataStorage, IPasswordHasher passwordHasher,
			IAuthEmailSender authEmailSender, IUserProcessor<TUser, TCreateUserForm, TUpdateUserForm> userProcessor, IPasswordValidator passwordValidator)
		{
			this.authDataStorage = authDataStorage;
			this.passwordHasher = passwordHasher;
			this.authEmailSender = authEmailSender;
			this.userProcessor = userProcessor;
			this.passwordValidator = passwordValidator;
		}

		public TUser CreateUserAndActivationRequest(TCreateUserForm form, Uri activateUserUrl)
		{
			if(!EmailValidator.Validate(form.Login))
				throw new IncorrectEmailException();

			var password = form.Password;

			if(!passwordValidator.Validate(password))
				throw new WeakPasswordException();

			var existingUser = authDataStorage.FindUserByLogin(form.Login);
			if(existingUser != null)
			{
				throw new LoginAlreadyExistsException(form.Login);
			}

			var userId = Guid.NewGuid();
			var user = userProcessor.MakeUserFromForm(form, userId);
			SetUserPassword(user, password);
			authDataStorage.CreateUser(user);
			SendUserActivationRequest(user, activateUserUrl);
			return authDataStorage.FindUser(userId);
		}

		public TUser CreateActiveUser(TCreateUserForm form)
		{
			EmailValidator.Validate(form.Login);

			var existingUser = authDataStorage.FindUserByLogin(form.Login);
			if(existingUser != null)
			{
				if(existingUser.IsActive)
					throw new LoginAlreadyExistsException(form.Login);
				throw new LoginAlreadyExistsButNeedsActivationException(form.Login);
			}

			var userId = Guid.NewGuid();
			var utcNow = DateTimeOffset.UtcNow;

			var user = userProcessor.MakeUserFromForm(form, userId);
			user.DateTimeActivated = utcNow;
			SetUserPassword(user, form.Password);
			authDataStorage.CreateUser(user);
			return authDataStorage.FindUser(userId);
		}

		public UserActivationRequestValidationResult<TUserActivationRequest, TUser> ValidateUserActivationRequest(
			Guid userActivationRequestId)
		{
			return ValidateUserActivationRequestInternal(userActivationRequestId);
		}

		public ChangePasswordRequestValidationResult<TChangePasswordRequest, TUser> ValidateChangePasswordRequest(Guid changePasswordRequestId)
		{
			return ValidateChangePasswordRequestInternal(changePasswordRequestId);
		}

		public void ActivateUser(Guid userActivationRequestId)
		{
			var result = ValidateUserActivationRequestInternal(userActivationRequestId);
			var request = result.UserActivationRequest;
			var user = result.User;

			var utcNow = DateTimeOffset.UtcNow;

			request.ActivationDateTime = utcNow;
			authDataStorage.UpdateActivateUserRequest(request);

			user.DateTimeActivated = utcNow;
			authDataStorage.UpdateUser(user);
		}

		public void SendChangePasswordRequest(string login, Uri restorePasswordUrl, Uri activateUserUrl)
		{
			var user = authDataStorage.FindUserByLogin(login);
			if(user == null)
				throw new LoginNotFoundException(login);

			if(!user.IsActive)
			{
				SendUserActivationRequest(user, activateUserUrl);
				return;
			}

			var requestId = Guid.NewGuid();
			var expirationDateTime = DateTimeOffset.UtcNow.Add(changePasswordRequestValidityPeriod);
			var request = new TChangePasswordRequest
			{
				RequestId = requestId,
				UserId = user.UserId,
				ExpirationDateTime = expirationDateTime
			};
			authDataStorage.CreateChangePasswordRequest(request);

			var builder = new UriBuilder(restorePasswordUrl);
			builder.Query = builder.Query.Length > 1
				? builder.Query.Substring(1) + "&" + "requestId=" + requestId
				: "requestId=" + requestId;

			authEmailSender.SendPasswordResetEmail(user.Login, builder.Uri.ToString());
		}

		public TUser ChangePassword(Guid changePasswordRequestId, string password)
		{
			var changePasswordRequest = authDataStorage.FindChangePasswordRequest(changePasswordRequestId);
			if(changePasswordRequest == null)
				throw new ChangePasswordRequestNotFoundException(changePasswordRequestId);
			if(changePasswordRequest.IsUsed)
				throw new ChangePasswordRequestAlreadyCompleteException(changePasswordRequestId);
			if(changePasswordRequest.Expired)
				throw new ChangePasswordRequestExpriredException(changePasswordRequestId);

			var user = authDataStorage.FindUser(changePasswordRequest.UserId);
			if(user == null)
				throw new UserNotFoundException(changePasswordRequest.UserId);

			changePasswordRequest.PasswordChangeDateTime = DateTimeOffset.UtcNow;
			authDataStorage.UpdateChangePasswordRequest(changePasswordRequest);

			SetUserPassword(user, password);
			authDataStorage.UpdateUser(user);
			return authDataStorage.FindUser(user.UserId);
		}

		public void UpdateUserDetails(Guid userId, TUpdateUserForm form)
		{
			var user = authDataStorage.FindUser(userId);
			if(user == null)
				throw new UserNotFoundException(userId);

			userProcessor.Update(user, form);

			authDataStorage.UpdateUser(user);
		}

		private UserActivationRequestValidationResult<TUserActivationRequest, TUser> ValidateUserActivationRequestInternal(
			Guid userActivationRequestId)
		{
			var userActivationRequest = authDataStorage.FindUserActivationRequest(userActivationRequestId);

			if(userActivationRequest == null)
				throw new UserActivationRequestNotFoundException(userActivationRequestId);

			if(userActivationRequest.IsUsed)
				throw new UserActivationRequestAlreadyCompleteException(userActivationRequestId);

			if(userActivationRequest.Expired)
				throw new UserActivationRequestExpiredException(userActivationRequestId);

			var user = authDataStorage.FindUser(userActivationRequest.DraftUserId);

			if(user == null)
				throw new UserNotFoundException(userActivationRequest.DraftUserId);

			if(user.IsActive)
				throw new UserAlreadyActiveException(userActivationRequest.DraftUserId);

			return new UserActivationRequestValidationResult<TUserActivationRequest, TUser>(userActivationRequest, user);
		}

		private ChangePasswordRequestValidationResult<TChangePasswordRequest, TUser> ValidateChangePasswordRequestInternal(Guid changePasswordRequestId)
		{
			var changePasswordRequest = authDataStorage.FindChangePasswordRequest(changePasswordRequestId);

			if(changePasswordRequest == null)
				throw new ChangePasswordRequestNotFoundException(changePasswordRequestId);

			if(changePasswordRequest.IsUsed)
				throw new ChangePasswordRequestAlreadyCompleteException(changePasswordRequestId);

			if(changePasswordRequest.Expired)
				throw new ChangePasswordRequestExpriredException(changePasswordRequestId);

			var user = authDataStorage.FindUser(changePasswordRequest.UserId);

			if(user == null)
				throw new UserNotFoundException(changePasswordRequest.UserId);

			return new ChangePasswordRequestValidationResult<TChangePasswordRequest, TUser>(changePasswordRequest, user);
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
			authDataStorage.CreateActivateUserRequest(request);
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