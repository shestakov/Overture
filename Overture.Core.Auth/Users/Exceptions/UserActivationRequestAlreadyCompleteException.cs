using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class UserActivationRequestAlreadyCompleteException : AuthException
	{
		public UserActivationRequestAlreadyCompleteException(Guid requestId)
			: base(string.Format("User Activation Request {0} already complete", requestId))
		{
		}
	}
}