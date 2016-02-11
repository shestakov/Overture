using Overture.Core.Auth.Users;

namespace Overture.Core.Auth.Authentication
{
	public class AuthenticationResult<TUser> where TUser: class, IUser
	{
		public AuthenticationResult(string encryptedBase64EncodedToken, TUser user)
		{
			EncryptedBase64EncodedToken = encryptedBase64EncodedToken;
			User = user;
		}

		public string EncryptedBase64EncodedToken { get; private set; }
		public TUser User { get; private set; }
	}
}