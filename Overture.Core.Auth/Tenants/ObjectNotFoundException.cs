using System;

namespace Overture.Core.Auth.Tenants
{
	public class ObjectNotFoundException : Exception
	{
		public ObjectNotFoundException(string message) : base(message)
		{
		}
	}
}