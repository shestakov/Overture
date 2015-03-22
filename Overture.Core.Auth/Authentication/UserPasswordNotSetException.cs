using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Authentication
{
	public class UserPasswordNotSetException : AuthException
	{
		public UserPasswordNotSetException(Guid userId)
			: base(string.Format("User {0} is active yet has no password hash and salt", userId))
		{
		}
	}
}