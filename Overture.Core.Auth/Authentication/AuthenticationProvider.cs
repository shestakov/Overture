using Overture.Core.Auth.Token;
using Overture.Core.Auth.Users;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Authentication
{
	public class AuthenticationProvider<TUser, TUserActivationRquest, TChangePasswordRequest> : IAuthenticationProvider
		where TUser : class, IUser
		where TUserActivationRquest : class, IUserActivationRequest
		where TChangePasswordRequest : class, IChangePasswordRequest
	{
		private readonly IAuthenticationTokenCryptography authenticationTokenCryptography;
		private readonly IPasswordHasher passwordHasher;
		private readonly IAuthDataStorage<TUser, TUserActivationRquest, TChangePasswordRequest> authDataStorage;

		public AuthenticationProvider(IAuthenticationTokenCryptography authenticationTokenCryptography,
			IPasswordHasher passwordHasher,
			IAuthDataStorage<TUser, TUserActivationRquest, TChangePasswordRequest> authDataStorage)
		{
			this.authenticationTokenCryptography = authenticationTokenCryptography;
			this.passwordHasher = passwordHasher;
			this.authDataStorage = authDataStorage;
		}

		public AuthenticationResult Authenticate(string login, string password)
		{
			login = login.Trim();
			if (string.IsNullOrEmpty(login))
				throw new WrongLoginPasswordException();
			if (string.IsNullOrEmpty(password))
				throw new WrongLoginPasswordException();

			var user = authDataStorage.FindUserByLogin(login);
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