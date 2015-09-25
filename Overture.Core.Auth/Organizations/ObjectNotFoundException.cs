using System;

namespace Overture.Core.Auth.Organizations
{
	public class ObjectNotFoundException : Exception
	{
		public ObjectNotFoundException(string message) : base(message)
		{
		}
	}
}