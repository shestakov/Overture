using System;
using System.IO;
using System.Security.Cryptography;

namespace Overture.Core.Auth.Token
{
	public class AuthenticationTokenCryptography : IAuthenticationTokenCryptography
	{
		public byte[] EncryptToken(AuthenticationToken authenticationToken)
		{
			var tokenBytes = authenticationToken.Serialize();

			var symmetricAlgorithm = new AesCryptoServiceProvider { Key = Convert.FromBase64String(authenticationKey) };
			symmetricAlgorithm.GenerateIV();
			var transform = symmetricAlgorithm.CreateEncryptor();

			using (var stream = new MemoryStream())
			{
				var writer = new BinaryWriter(stream);
				writer.Write(symmetricAlgorithm.IV.Length);
				writer.Write(symmetricAlgorithm.IV);
				using (var cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
				{
					cryptoStream.Write(tokenBytes, 0, tokenBytes.Length);
				}
				return stream.ToArray();
			}
		}

		public string EncryptTokenToBase64(AuthenticationToken authenticationToken)
		{
			return Convert.ToBase64String(EncryptToken(authenticationToken));
		}

		public AuthenticationToken DecryptToken(byte[] encryptedToken)
		{
			int ivLength;
			byte[] iv;

			using (var stream = new MemoryStream(encryptedToken))
			{
				var reader = new BinaryReader(stream);
				ivLength = reader.ReadInt32();
				iv = reader.ReadBytes(ivLength);
			}

			var symmetricAlgorithm = new AesCryptoServiceProvider { Key = Convert.FromBase64String(authenticationKey), IV = iv };
			var transform = symmetricAlgorithm.CreateDecryptor(Convert.FromBase64String(authenticationKey), iv);
			using (var memoryStrem = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStrem, transform, CryptoStreamMode.Write))
				{
					cryptoStream.Write(encryptedToken, ivLength + 4, encryptedToken.Length - ivLength - 4);
				}
				return AuthenticationToken.Deserialize(memoryStrem.ToArray());
			}
		}

		public AuthenticationToken DecryptTokenFromBase64(string encryptedTokenBase64)
		{
			if (string.IsNullOrEmpty(encryptedTokenBase64))
				throw new ArgumentException("Token string is empty.", "encryptedTokenBase64");
			var tokenBytes = Convert.FromBase64String(encryptedTokenBase64);
			return DecryptToken(tokenBytes);
		}

		private const string authenticationKey = "whYcKmPf8meqMI/jcuKbQRy/w6lpYnHUia79W8U5kFU=";
	}
}