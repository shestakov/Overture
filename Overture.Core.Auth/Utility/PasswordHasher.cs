using System;
using System.Security.Cryptography;
using System.Text;

namespace Overture.Core.Auth.Utility
{
	public interface IPasswordHasher
	{
		string GenerateSalt();
		string HashPassword(string password, string salt);
	}

	public class PasswordHasher : IPasswordHasher
	{
		public string GenerateSalt()
		{
			var buffer = new byte[16];
			(new RNGCryptoServiceProvider()).GetBytes(buffer);
			return Convert.ToBase64String(buffer);
		}

		public string HashPassword(string password, string salt)
		{
			var passwordBinary = Encoding.Unicode.GetBytes(password);
			var saltBinary = Convert.FromBase64String(salt);
			var passwordSaltCombined = new byte[saltBinary.Length + passwordBinary.Length];
			Buffer.BlockCopy(saltBinary, 0, passwordSaltCombined, 0, saltBinary.Length);
			Buffer.BlockCopy(passwordBinary, 0, passwordSaltCombined, saltBinary.Length, passwordBinary.Length);
			return Convert.ToBase64String(new SHA1Managed().ComputeHash(passwordSaltCombined));
		}
	}
}