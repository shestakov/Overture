namespace Overture.Core.Auth.Users.ChangePasswordRequests
{
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
}