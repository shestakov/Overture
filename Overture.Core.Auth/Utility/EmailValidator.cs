using System;
using System.Net.Mail;

namespace Overture.Core.Auth.Utility
{
	public static class EmailValidator
	{
		public static bool Validate(string email)
		{
			try
			{
				// ReSharper disable once ObjectCreationAsStatement
				new MailAddress(email);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}
	}
}