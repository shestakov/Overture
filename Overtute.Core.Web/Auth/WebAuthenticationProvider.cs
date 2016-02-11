using System;
using System.Web;
using Ninject.Extensions.Logging;
using Overture.Core.Auth.Authentication;
using Overture.Core.Auth.BruteForceProtection;
using Overture.Core.Auth.Token;
using Overture.Core.Auth.Users;
using Overture.Core.Auth.Users.Exceptions;
using Overture.Core.Auth.Users.Storage;

namespace Overtute.Core.Web.Auth
{
	public class WebAuthenticationProvider<TUser> :
		IWebAuthenticationProvider<TUser>
		where TUser : class, IUser
	{
		private readonly ILogger log;
		private readonly IUserStorage<TUser> userStorage;
		private readonly IAuthenticationCookieManager authenticationCookieManager;
		private readonly IAuthenticationTokenCryptography authenticationTokenCryptography;
		private readonly IAuthenticationProvider<TUser> authenticationProvider;
		private readonly ILoginBruteForceProtector loginBruteForceProtector;
		private readonly IPasswordBruteForceProtector passwordBruteForceProtector;

		public WebAuthenticationProvider(
			IUserStorage<TUser> userStorage,
			IAuthenticationCookieManager authenticationCookieManager,
			IAuthenticationTokenCryptography authenticationTokenCryptography,
			IAuthenticationProvider<TUser> authenticationProvider, ILoginBruteForceProtector loginBruteForceProtector,
			IPasswordBruteForceProtector passwordBruteForceProtector,
			ILogger log)
		{
			this.userStorage = userStorage;
			this.authenticationCookieManager = authenticationCookieManager;
			this.authenticationTokenCryptography = authenticationTokenCryptography;
			this.authenticationProvider = authenticationProvider;
			this.loginBruteForceProtector = loginBruteForceProtector;
			this.passwordBruteForceProtector = passwordBruteForceProtector;
			this.log = log;
		}

		public void SignIn(HttpContextBase httpContext, string login, string password, bool rememberMe)
		{
			var ip = httpContext.Request.UserHostAddress;

			if (!loginBruteForceProtector.CheckAttemptAllowed(ip))
				throw new IpBanException();

			if (!passwordBruteForceProtector.CheckAttemptAllowed(login))
				throw new LoginBanException();

			AuthenticationResult<TUser> userAuthenticationResult;
			try
			{
				userAuthenticationResult = authenticationProvider.Authenticate(login, password);
			}
			catch (LoginNotFoundException)
			{
				loginBruteForceProtector.AddFailAttempt(ip);
				passwordBruteForceProtector.AddFailAttempt(login);
				throw;
			}
			catch (WrongLoginPasswordException)
			{
				loginBruteForceProtector.AddFailAttempt(ip);
				passwordBruteForceProtector.AddFailAttempt(login);
				throw;
			}

			loginBruteForceProtector.ClearAttemptsForIp(ip);
			passwordBruteForceProtector.ClearAttemptsForIp(login);

			authenticationCookieManager.SetTokenCookie(httpContext, userAuthenticationResult.EncryptedBase64EncodedToken, rememberMe);

			log.Info(string.Format("User authenticated. login: {0}, userId: {1}", login, userAuthenticationResult.User.UserId));
		}

		public void SignUserIn(HttpContextBase httpContext, Guid userId, bool rememberMe)
		{
			var user = userStorage.FindUser(userId);
			if (user == null)
				throw new Exception(string.Format("Cannot find user {0}", userId));
			var authenticationToken = new AuthenticationToken(user.UserId);
			var encryptedBase64EncodedToken = authenticationTokenCryptography.EncryptTokenToBase64(authenticationToken);
			authenticationCookieManager.SetTokenCookie(httpContext, encryptedBase64EncodedToken, rememberMe);
		}

		public TUser Authenticate(HttpContextBase httpContext)
		{
			var encryptedBase64EncodedToken = authenticationCookieManager.FindValidToken(httpContext);
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
					var tokenIsTooLongMessage = string.Format("too long token ({0})", length);
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

		public void SignOut(HttpContextBase httpContext)
		{
			authenticationCookieManager.RemoveTokenCookie(httpContext);
		}
	}
}