using System;
using System.Web;

namespace Overtute.Core.Web.Auth
{
	public class AuthenticationCookieManager : IAuthenticationCookieManager
	{
		private const string authenticationCookieName = "auth";
		private readonly string domain;

		public AuthenticationCookieManager(string domain)
		{
			this.domain = domain;
		}

		public string FindValidToken(HttpContextBase context)
		{
			var tokenCookie = FindCookie(context.Request, authenticationCookieName);
			if (string.IsNullOrWhiteSpace(tokenCookie)) return null;
			return tokenCookie;
		}

		public void SetTokenCookie(HttpContextBase context, string encryptedBase64EncodedToken, bool rememberMe)
		{
			SetTokenCookie(context, authenticationCookieName, encryptedBase64EncodedToken, domain, rememberMe);
		}

		public void RemoveTokenCookie(HttpContextBase httpContext)
		{
			RemoveTokenCookie(httpContext, domain);
		}

		private static void SetTokenCookie(HttpContextBase context, string cookieName, string base64EncodedToken, string domain, bool rememberMe)
		{
			var expires = rememberMe ? DateTime.UtcNow.AddDays(7) : (DateTime?) null;
			var cookie = CreateTokenCookie(cookieName, base64EncodedToken, expires, domain);
			//cookie.Secure = true;
			context.Response.SetCookie(cookie);
		}

		private static HttpCookie CreateTokenCookie(string cookieName, string base64EncodedToken, DateTime? expires, string domain)
		{
			var cookie = new HttpCookie(cookieName, base64EncodedToken) { HttpOnly = true };
			if (!string.IsNullOrEmpty(domain)) cookie.Domain = domain;
			if (expires.HasValue)
				cookie.Expires = expires.Value;
			return cookie;
		}

		private static string FindCookie(HttpRequestBase request, string name)
		{
			var httpCookie = request.Cookies[name];
			return httpCookie == null ? null : httpCookie.Value;
		}

		private static void RemoveTokenCookie(HttpContextBase context, string domain)
		{
			context.Response.SetCookie(CreateExpiredCookie(authenticationCookieName, domain));
		}

		private static HttpCookie CreateExpiredCookie(string cookieName, string domain)
		{
			return CreateTokenCookie(cookieName, string.Empty, DateTime.UtcNow.AddDays(-1), domain);
		}
	}
}