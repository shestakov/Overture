using System.Net.Http;

namespace Overture.WebApi.Auth
{
	public interface IWebApiAuthenticationCookieManager
	{
		string FindValidToken(HttpRequestMessage request);
		void SetTokenCookie(HttpResponseMessage context, string encryptedBase64EncodedToken, bool rememberMe);
		void RemoveTokenCookie(HttpResponseMessage httpContext);
	}
}