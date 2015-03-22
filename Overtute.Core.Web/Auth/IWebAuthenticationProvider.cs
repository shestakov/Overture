using System;
using System.Web;
using Overture.Core.Auth.Users;

namespace Overtute.Core.Web.Auth
{
	public interface IWebAuthenticationProvider<out TUser>
		where TUser: class, IUser
	{
		void SignIn(HttpContextBase httpContext, string login, string password, bool rememberMe);
		void SignUserIn(HttpContextBase httpContext, Guid userId, bool rememberMe);
		TUser Authenticate(HttpContextBase httpContext);
		void SignOut(HttpContextBase httpContext);
	}

	public class IpBanException : Exception
	{
	}

	public class LoginBanException : Exception
	{
	}
}