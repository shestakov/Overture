using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Overture.WebApi.Auth
{
	public class WebApiAuthenticationCookieManager : IWebApiAuthenticationCookieManager
	{
		private const string authenticationCookieName = "auth";
		private readonly string domain;

		public WebApiAuthenticationCookieManager(string domain)
		{
			this.domain = domain;
		}

		public string FindValidToken(HttpRequestMessage request)
		{
			var tokenCookie = FindCookie(request, authenticationCookieName);
			if (string.IsNullOrWhiteSpace(tokenCookie)) return null;
			return tokenCookie;
		}

		public void SetTokenCookie(HttpResponseMessage response, string encryptedBase64EncodedToken, bool rememberMe)
		{
			SetTokenCookie(response, authenticationCookieName, encryptedBase64EncodedToken, domain, rememberMe);
		}

		public void RemoveTokenCookie(HttpResponseMessage response)
		{
			RemoveTokenCookie(response, domain);
		}

		private static void SetTokenCookie(HttpResponseMessage response, string cookieName, string base64EncodedToken, string domain, bool rememberMe)
		{
			var expires = rememberMe ? DateTime.UtcNow.AddDays(30) : (DateTime?) null;
			var cookie = CreateTokenCookie(cookieName, base64EncodedToken, expires, domain);
			//cookie.Secure = true;
			response.Headers.AddCookies(new[] {cookie});
		}

		private static CookieHeaderValue CreateTokenCookie(string cookieName, string base64EncodedToken, DateTime? expires, string domain)
		{
			return new CookieHeaderValue(cookieName, base64EncodedToken) { HttpOnly = true, Domain = domain, Expires = expires };
		}

		private static string FindCookie(HttpRequestMessage request, string name)
		{
			var httpCookie = request.Headers.GetCookies(name).FirstOrDefault();
			return httpCookie?[name].Value;
		}

		private static void RemoveTokenCookie(HttpResponseMessage context, string domain)
		{
			var cookie = CreateExpiredCookie(authenticationCookieName, domain);
			context.Headers.AddCookies(new[] {cookie});
		}

		private static CookieHeaderValue CreateExpiredCookie(string cookieName, string domain)
		{
			return CreateTokenCookie(cookieName, string.Empty, DateTime.UtcNow.AddDays(-1), domain);
		}
	}
}