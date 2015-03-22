using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users
{
	public interface IUserManager<TUser, TUserActivationRequest, in TCreateUserForm, in TUpdateUserForm, TChangePasswordRequest>
		where TUser : class, IUser
		where TUserActivationRequest : class, IUserActivationRequest, new()
		where TCreateUserForm : class, IUserForm
		where TChangePasswordRequest : class, IChangePasswordRequest
	{
		TUser CreateUserAndActivationRequest(TCreateUserForm form, Uri activateUserUrl);
		TUser CreateActiveUser(TCreateUserForm form);
		void ActivateUser(Guid userActivationRequestId);
		void SendChangePasswordRequest(string login, Uri setPasswordUrl, Uri activateUserUrl);

		TUser ChangePassword(Guid changePasswordRequestId, string password);
		void UpdateUserDetails(Guid userId, TUpdateUserForm form);
		UserActivationRequestValidationResult<TUserActivationRequest, TUser> ValidateUserActivationRequest(Guid userActivationRequestId);
		ChangePasswordRequestValidationResult<TChangePasswordRequest, TUser> ValidateChangePasswordRequest(Guid changePasswordRequestId);
	}

	public class ChangePasswordRequestValidationResult<TChangePasswordRequest, TUser>
	{
		public ChangePasswordRequestValidationResult(TChangePasswordRequest changePasswordRequest, TUser user)
		{
			ChangePasswordRequest = changePasswordRequest;
			User = user;
		}

		public TChangePasswordRequest ChangePasswordRequest { get; private set; }
		public TUser User { get; private set; }
	}

	public class UserActivationRequestValidationResult<TUserActivationRequest, TUser>
	{
		public UserActivationRequestValidationResult(TUserActivationRequest userActivationRequest, TUser user)
		{
			UserActivationRequest = userActivationRequest;
			User = user;
		}

		public TUserActivationRequest UserActivationRequest { get; private set; }
		public TUser User { get; private set; }
	}

	public class LoginNotFoundException : AuthException
	{
		public LoginNotFoundException(string login)
			: base(string.Format("Login {0} not found", login))
		{
		}
	}

	public class UserAlreadyActiveException : AuthException
	{
		public UserAlreadyActiveException(Guid userId)
			: base(string.Format("User {0} already active", userId))
		{
		}
	}

	public class LoginAlreadyExistsException : AuthException
	{
		public LoginAlreadyExistsException(string login)
			: base(string.Format("Login {0} already exists", login))
		{
		}
	}

	public class LoginAlreadyExistsButNeedsActivationException : AuthException
	{
		public LoginAlreadyExistsButNeedsActivationException(string login)
			: base(string.Format("Login {0} already exists but needs activation", login))
		{
		}
	}

	public class UserActivationRequestAlreadyCompleteException : AuthException
	{
		public UserActivationRequestAlreadyCompleteException(Guid requestId)
			: base(string.Format("User Activation Request {0} already complete", requestId))
		{
		}
	}

	public class UserActivationRequestNotFoundException : AuthException
	{
		public UserActivationRequestNotFoundException(Guid requestId)
			: base(string.Format("User Activation Request {0} not found", requestId))
		{
		}
	}

	public class UserActivationRequestExpiredException : AuthException
	{
		public UserActivationRequestExpiredException(Guid requestId)
			: base(string.Format("User Activation Request {0} expired", requestId))
		{
		}
	}

	public class ChangePasswordRequestNotFoundException : AuthException
	{
		public ChangePasswordRequestNotFoundException(Guid requestId)
			: base(string.Format("Change password request {0} not found", requestId))
		{
		}
	}

	public class ChangePasswordRequestAlreadyCompleteException : AuthException
	{
		public ChangePasswordRequestAlreadyCompleteException(Guid requestId)
			: base(string.Format("Password validation request {0} already compelte", requestId))
		{
		}
	}

	public class ChangePasswordRequestExpriredException : AuthException
	{
		public ChangePasswordRequestExpriredException(Guid requestId)
			: base(string.Format("Password validation request {0} expired", requestId))
		{
		}
	}
}