using System;

namespace Overture.Core.Auth.Authentication
{
	public interface IAuthenticationProvider
	{
		AuthenticationResult Authenticate(string login, string password);
	}

	public class WrongLoginPasswordException : Exception
	{
	}

	public class InactiveUserException : Exception
	{
	}
}