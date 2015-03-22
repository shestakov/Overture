using System;

namespace Overture.Core.Auth.Utility
{
	public class AuthException : Exception
	{
		protected AuthException(string message) : base(message)
		{
		}

		protected AuthException()
		{
		}
	}
}