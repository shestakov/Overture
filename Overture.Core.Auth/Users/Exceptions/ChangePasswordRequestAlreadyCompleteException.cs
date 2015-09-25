using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class ChangePasswordRequestAlreadyCompleteException : AuthException
	{
		public ChangePasswordRequestAlreadyCompleteException(Guid requestId)
			: base(string.Format("Change password request {0} already compelte", requestId))
		{
		}
	}
}