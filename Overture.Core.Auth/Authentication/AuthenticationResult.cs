using System;

namespace Overture.Core.Auth.Authentication
{
	public class AuthenticationResult
	{
		public AuthenticationResult(string encryptedBase64EncodedToken, Guid userId)
		{
			EncryptedBase64EncodedToken = encryptedBase64EncodedToken;
			UserId = userId;
		}

		public string EncryptedBase64EncodedToken { get; private set; }
		public Guid UserId { get; private set; }
	}
}