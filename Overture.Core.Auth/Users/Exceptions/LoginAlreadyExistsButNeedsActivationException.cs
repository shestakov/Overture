using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class LoginAlreadyExistsButNeedsActivationException : AuthException
	{
		public LoginAlreadyExistsButNeedsActivationException(string login)
			: base(string.Format("Login {0} already exists but needs activation", login))
		{
		}
	}
}