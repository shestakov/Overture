using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class LoginNotFoundException : AuthException
	{
		public LoginNotFoundException(string login)
			: base(string.Format("Login {0} not found", login))
		{
		}
	}
}