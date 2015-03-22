using System;

namespace Overture.Core.Auth.Users
{
	public class RandomPasswordGenerator :IRandomPasswordGenerator
	{
		private const int passwordLength = 8;
		private const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!?";

		public string Generate()
		{
			var chars = new char[passwordLength];
			var rd = new Random();

			for (var i = 0; i < passwordLength; i++)
			{
				chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
			}

			return new string(chars);
		}
	}
}