using System;
using Overture.Core.Auth.Passwords;
using Overture.Core.Auth.Users.Exceptions;
using Overture.Core.Auth.Users.Storage;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.ChangePasswordRequests
{
	public class ChangePasswordRequestManager<TUser, TChangePasswordRequest> :
		IChangePasswordRequestManager<TUser, TChangePasswordRequest>
		where TUser : class, IUser
		where TChangePasswordRequest : class, IChangePasswordRequest, new()
	{
		private readonly IAuthEmailSender authEmailSender;
		private readonly TimeSpan changePasswordRequestValidityPeriod = new TimeSpan(0, 1, 0, 0);
		private readonly IPasswordHasher passwordHasher;
		private readonly IPasswordValidator passwordValidator;
		private readonly IUserStorage<TUser> userStorage;
		private readonly IChangePasswordRequestDataStorage<TChangePasswordRequest> changePasswordRequestDataStorage;
		
		public ChangePasswordRequestManager(IUserStorage<TUser> userStorage, IPasswordHasher passwordHasher,
			IAuthEmailSender authEmailSender, IPasswordValidator passwordValidator, IChangePasswordRequestDataStorage<TChangePasswordRequest> changePasswordRequestDataStorage)
		{
			this.userStorage = userStorage;
			this.passwordHasher = passwordHasher;
			this.authEmailSender = authEmailSender;
			this.passwordValidator = passwordValidator;
			this.changePasswordRequestDataStorage = changePasswordRequestDataStorage;
		}

		public ChangePasswordRequestValidationResult<TChangePasswordRequest, TUser> ValidateChangePasswordRequest(Guid changePasswordRequestId)
		{
			return ValidateChangePasswordRequestInternal(changePasswordRequestId);
		}

		public void SendChangePasswordRequest(string login, Uri restorePasswordUrl, Uri activateUserUrl)
		{
			var user = userStorage.FindUserByLogin(login);
			if (user == null)
				throw new LoginNotFoundException(login);

			//if (!user.IsActive)
			//{
			//	throw new UserIsNotActiveException(user.UserId);
			//}

			var requestId = Guid.NewGuid();
			var expirationDateTime = DateTimeOffset.UtcNow.Add(changePasswordRequestValidityPeriod);
			var request = new TChangePasswordRequest
			{
				RequestId = requestId,
				UserId = user.UserId,
				ExpirationDateTime = expirationDateTime
			};
			changePasswordRequestDataStorage.CreateChangePasswordRequest(request);

			var builder = new UriBuilder(restorePasswordUrl);
			builder.Query = builder.Query.Length > 1
				? builder.Query.Substring(1) + "&" + "requestId=" + requestId
				: "requestId=" + requestId;

			authEmailSender.SendPasswordResetEmail(user.Login, builder.Uri.ToString());
		}

		public TUser ChangePassword(Guid changePasswordRequestId, string password)
		{
			var changePasswordRequest = changePasswordRequestDataStorage.FindChangePasswordRequest(changePasswordRequestId);
			if (changePasswordRequest == null)
				throw new ChangePasswordRequestNotFoundException(changePasswordRequestId);
			if (changePasswordRequest.IsUsed)
				throw new ChangePasswordRequestAlreadyCompleteException(changePasswordRequestId);
			if (changePasswordRequest.Expired)
				throw new ChangePasswordRequestExpriredException(changePasswordRequestId);

			if (!passwordValidator.Validate(password))
				throw new WeakPasswordException();

			var user = userStorage.FindUser(changePasswordRequest.UserId);
			if (user == null)
				throw new UserNotFoundException(changePasswordRequest.UserId);

			changePasswordRequest.PasswordChangeDateTime = DateTimeOffset.UtcNow;
			changePasswordRequestDataStorage.UpdateChangePasswordRequest(changePasswordRequest);

			SetUserPassword(user, password);

			if (!user.IsActive)
				user.DateTimeActivated = DateTimeOffset.UtcNow;

			userStorage.UpdateUser(user);
			return userStorage.FindUser(user.UserId);
		}

		private ChangePasswordRequestValidationResult<TChangePasswordRequest, TUser> ValidateChangePasswordRequestInternal(Guid changePasswordRequestId)
		{
			var changePasswordRequest = changePasswordRequestDataStorage.FindChangePasswordRequest(changePasswordRequestId);

			if (changePasswordRequest == null)
				throw new ChangePasswordRequestNotFoundException(changePasswordRequestId);

			if (changePasswordRequest.IsUsed)
				throw new ChangePasswordRequestAlreadyCompleteException(changePasswordRequestId);

			if (changePasswordRequest.Expired)
				throw new ChangePasswordRequestExpriredException(changePasswordRequestId);

			var user = userStorage.FindUser(changePasswordRequest.UserId);

			if (user == null)
				throw new UserNotFoundException(changePasswordRequest.UserId);

			return new ChangePasswordRequestValidationResult<TChangePasswordRequest, TUser>(changePasswordRequest, user);
		}

		private void SetUserPassword(TUser user, string newPassword)
		{
			user.PasswordSalt = passwordHasher.GenerateSalt();
			user.PasswordHash = passwordHasher.HashPassword(newPassword, user.PasswordSalt);
		}
	}
}