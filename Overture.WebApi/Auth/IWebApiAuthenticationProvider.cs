using System;
using System.Net.Http;
using Overture.Core.Auth.Users;

namespace Overture.WebApi.Auth
{
	public interface IWebApiAuthenticationProvider<out TUser>
		where TUser: class, IUser
	{
		void SignIn(HttpResponseMessage response, string login, string password, bool rememberMe, string clientIpAddress);
		void SignUserIn(HttpResponseMessage response, Guid userId, bool rememberMe);
		TUser Authenticate(HttpRequestMessage request);
		void SignOut(HttpResponseMessage response);
	}

}