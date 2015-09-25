using System;
using Overture.Core.Auth.Utility;

namespace Overture.Core.Auth.Users.Exceptions
{
	public class ChangePasswordRequestNotFoundException : AuthException
	{
		public ChangePasswordRequestNotFoundException(Guid requestId)
			: base(string.Format("Change password request {0} not found", requestId))
		{
		}
	}
}