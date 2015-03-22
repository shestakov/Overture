using System.Text.RegularExpressions;

namespace Overture.Core.Auth.Utility
{
	public class PasswordValidator : IPasswordValidator
	{
		private const int minimumLength = 8;
		private const int upperCaseLength = 0;
		private const int lowerCaseLength = 0;
		private const int nonAlphaLength = 0;

		public bool Validate(string password)
		{
			if (password == null)
				return false;
			if (password.Length < minimumLength)
				return false;
			if (UpperCaseCount(password) < upperCaseLength)
				return false;
			if (LowerCaseCount(password) < lowerCaseLength)
				return false;
			if (NumericCount(password) < 1)
				return false;
			if (NonAlphaCount(password) < nonAlphaLength)
				return false;
			return true;
		}

		private static int UpperCaseCount(string password)
		{
			return Regex.Matches(password, "[A-Z]").Count;
		}

		private static int LowerCaseCount(string password)
		{
			return Regex.Matches(password, "[a-z]").Count;
		}
		private static int NumericCount(string password)
		{
			return Regex.Matches(password, "[0-9]").Count;
		}
		private static int NonAlphaCount(string password)
		{
			return Regex.Matches(password, @"[^A-Za-z0-9]").Count;
		}
	}
}