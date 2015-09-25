using Overture.Core.Auth.Passwords;
using Overture.Core.Auth.Token;
using Overture.Core.Auth.Users;
using Overture.Core.Auth.Users.Storage;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Authentication
{
	public class AuthenticationProvider<TUser> : IAuthenticationProvider
		where TUser : class, IUser
	{
		private readonly IAuthenticationTokenCryptography authenticationTokenCryptography;
		private readonly IPasswordHasher passwordHasher;
		private readonly IUserStorage<TUser> userStorage;

		public AuthenticationProvider(IAuthenticationTokenCryptography authenticationTokenCryptography,
			IPasswordHasher passwordHasher,
			IUserStorage<TUser> userStorage)
		{
			this.authenticationTokenCryptography = authenticationTokenCryptography;
			this.passwordHasher = passwordHasher;
			this.userStorage = userStorage;
		}

		public AuthenticationResult Authenticate(string login, string password)
		{
			if (string.IsNullOrWhiteSpace(login))
				throw new WrongLoginPasswordException();
			if (string.IsNullOrEmpty(password))
				throw new WrongLoginPasswordException();

			login = login.Trim();

			var user = userStorage.FindUserByLogin(login);
			if (user == null)
				throw new WrongLoginPasswordException();

			if (!user.IsActive)
				throw new InactiveUserException();

			if (string.IsNullOrEmpty(user.PasswordHash) || string.IsNullOrEmpty(user.PasswordSalt))
			{
				throw new UserPasswordNotSetException(user.UserId);
			}

			var passwordHash = passwordHasher.HashPassword(password, user.PasswordSalt);
			if (user.PasswordHash != passwordHash)
				throw new WrongLoginPasswordException();

			var authenticationToken = new AuthenticationToken(user.UserId);
			var encryptedBase64EncodedToken = authenticationTokenCryptography.EncryptTokenToBase64(authenticationToken);

			return new AuthenticationResult(encryptedBase64EncodedToken, user.UserId);
		}
	}
}