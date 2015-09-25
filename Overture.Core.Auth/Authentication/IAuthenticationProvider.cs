namespace Overture.Core.Auth.Authentication
{
	public interface IAuthenticationProvider
	{
		AuthenticationResult Authenticate(string login, string password);
	}
}