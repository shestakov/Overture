using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class LoginAlreadyExistsException : AuthException
	{
		public LoginAlreadyExistsException(string login)
			: base(string.Format("Login {0} already exists", login))
		{
		}
	}
}