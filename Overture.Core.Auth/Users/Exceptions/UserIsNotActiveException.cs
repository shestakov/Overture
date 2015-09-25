using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class UserIsNotActiveException : AuthException
	{
		public UserIsNotActiveException(Guid userId)
			: base(string.Format("User {0} is not active", userId))
		{
		}
	}
}