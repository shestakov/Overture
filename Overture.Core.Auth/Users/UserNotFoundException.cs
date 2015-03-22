using System;

namespace Overture.Core.Auth.Users
{
	public class UserNotFoundException : Exception
	{
		public UserNotFoundException(Guid userId)
			: base(string.Format("User {0} not found", userId))
		{
		}
	}
}