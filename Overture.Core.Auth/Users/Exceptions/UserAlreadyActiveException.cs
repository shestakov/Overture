using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class UserAlreadyActiveException : AuthException
	{
		public UserAlreadyActiveException(Guid userId)
			: base(string.Format("User {0} already active", userId))
		{
		}
	}
}