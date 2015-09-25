using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class UserActivationRequestExpiredException : AuthException
	{
		public UserActivationRequestExpiredException(Guid requestId)
			: base(string.Format("User Activation Request {0} expired", requestId))
		{
		}
	}
}