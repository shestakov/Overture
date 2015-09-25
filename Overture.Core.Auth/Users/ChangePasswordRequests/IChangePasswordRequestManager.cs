using System;

namespace Overture.Core.Auth.Users.ChangePasswordRequests
{
	public interface IChangePasswordRequestManager<TUser, TChangePasswordRequest>
		where TUser : class, IUser
		where TChangePasswordRequest : class, IChangePasswordRequest
	{
		void SendChangePasswordRequest(string login, Uri setPasswordUrl, Uri activateUserUrl);
		TUser ChangePassword(Guid changePasswordRequestId, string password);
		ChangePasswordRequestValidationResult<TChangePasswordRequest, TUser> ValidateChangePasswordRequest(Guid changePasswordRequestId);
	}
}