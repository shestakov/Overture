using System;
using System.Net.Http;
using Ninject.Extensions.Logging;
using Overture.Core.Auth.Authentication;
using Overture.Core.Auth.BruteForceProtection;
using Overture.Core.Auth.Token;
using Overture.Core.Auth.Users;
using Overture.Core.Auth.Users.Exceptions;
using Overture.Core.Auth.Users.Storage;

namespace Overture.WebApi.Auth
{
	public class WebApiAuthenticationProvider<TUser> : IWebApiAuthenticationProvider<TUser> where TUser : class, IUser
	{
		private readonly ILogger log;
		private readonly IUserStorage<TUser> userStorage;
		private readonly IWebApiAuthenticationCookieManager webApiAuthenticationCookieManager;
		private readonly IAuthenticationTokenCryptography authenticationTokenCryptography;
		private readonly IAuthenticationProvider authenticationProvider;
		private readonly ILoginBruteForceProtector loginBruteForceProtector;
		private readonly IPasswordBruteForceProtector passwordBruteForceProtector;

		public WebApiAuthenticationProvider(
			IUserStorage<TUser> userStorage,
			IWebApiAuthenticationCookieManager webApiAuthenticationCookieManager,
			IAuthenticationTokenCryptography authenticationTokenCryptography,
			IAuthenticationProvider authenticationProvider, ILoginBruteForceProtector loginBruteForceProtector,
			IPasswordBruteForceProtector passwordBruteForceProtector,
			ILogger log)
		{
			this.userStorage = userStorage;
			this.webApiAuthenticationCookieManager = webApiAuthenticationCookieManager;
			this.authenticationTokenCryptography = authenticationTokenCryptography;
			this.authenticationProvider = authenticationProvider;
			this.loginBruteForceProtector = loginBruteForceProtector;
			this.passwordBruteForceProtector = passwordBruteForceProtector;
			this.log = log;
		}

		public void SignIn(HttpResponseMessage response, string login, string password, bool rememberMe, string clientIpAddress)
		{
			if (!loginBruteForceProtector.CheckAttemptAllowed(clientIpAddress))
				throw new IpBanException();

			if (!passwordBruteForceProtector.CheckAttemptAllowed(login))
				throw new LoginBanException();

			AuthenticationResult userAuthenticationResult;
			try
			{
				userAuthenticationResult = authenticationProvider.Authenticate(login, password);
			}
			catch (LoginNotFoundException)
			{
				loginBruteForceProtector.AddFailAttempt(clientIpAddress);
				passwordBruteForceProtector.AddFailAttempt(login);
				throw;
			}
			catch (WrongLoginPasswordException)
			{
				loginBruteForceProtector.AddFailAttempt(clientIpAddress);
				passwordBruteForceProtector.AddFailAttempt(login);
				throw;
			}

			loginBruteForceProtector.ClearAttemptsForIp(clientIpAddress);
			passwordBruteForceProtector.ClearAttemptsForIp(login);

			webApiAuthenticationCookieManager.SetTokenCookie(response, userAuthenticationResult.EncryptedBase64EncodedToken, rememberMe);

			log.Info($"User authenticated. login: {login}, userId: {userAuthenticationResult.UserId}");
		}

		public void SignUserIn(HttpResponseMessage response, Guid userId, bool rememberMe)
		{
			var user = userStorage.FindUser(userId);
			if (user == null)
				throw new Exception($"Cannot find user {userId}");
			var authenticationToken = new AuthenticationToken(user.UserId);
			var encryptedBase64EncodedToken = authenticationTokenCryptography.EncryptTokenToBase64(authenticationToken);
			webApiAuthenticationCookieManager.SetTokenCookie(response, encryptedBase64EncodedToken, rememberMe);
		}

		public TUser Authenticate(HttpRequestMessage request)
		{
			var encryptedBase64EncodedToken = webApiAuthenticationCookieManager.FindValidToken(request);
			AuthenticationToken token;
			try
			{
				token = authenticationTokenCryptography.DecryptTokenFromBase64(encryptedBase64EncodedToken);
			}
			catch (Exception e)
			{
				if (encryptedBase64EncodedToken == null)
				{
					log.Info(e, "Empty authenticationToken in a cookie (WEB)");
				}
				else
				{
					var length = encryptedBase64EncodedToken.Length;
					var tokenIsTooLongMessage = $"too long token ({length})";
					log.Info(e, "Failed to extract token (WEB) from: {0}",
						length < 1024 ? encryptedBase64EncodedToken : tokenIsTooLongMessage);
				}
				return null;
			}

			if (token == null || !token.IsValid)
				return null;
			
			var user = userStorage.FindUser(token.UserId);
			if (user == null || !user.IsActive)
				return null;
			return user;
		}

		public void SignOut(HttpResponseMessage response)
		{
			webApiAuthenticationCookieManager.RemoveTokenCookie(response);
		}
	}
}