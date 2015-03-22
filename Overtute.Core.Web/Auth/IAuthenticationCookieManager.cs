using System.Web;

namespace Overtute.Core.Web.Auth
{
	public interface IAuthenticationCookieManager
	{
		string FindValidToken(HttpContextBase context);
		void SetTokenCookie(HttpContextBase context, string encryptedBase64EncodedToken, bool rememberMe);
		void RemoveTokenCookie(HttpContextBase httpContext);
	}
}