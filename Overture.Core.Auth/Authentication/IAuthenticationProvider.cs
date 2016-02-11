using Overture.Core.Auth.Users;

namespace Overture.Core.Auth.Authentication
{
	public interface IAuthenticationProvider<TUser> where TUser : class, IUser
	{
		AuthenticationResult<TUser> Authenticate(string login, string password);
	}
}