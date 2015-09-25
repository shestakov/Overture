using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class UserActivationRequestNotFoundException : AuthException
	{
		public UserActivationRequestNotFoundException(Guid requestId)
			: base(string.Format("User Activation Request {0} not found", requestId))
		{
		}
	}
}