using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class ChangePasswordRequestExpriredException : AuthException
	{
		public ChangePasswordRequestExpriredException(Guid requestId)
			: base(string.Format("Password validation request {0} expired", requestId))
		{
		}
	}
}