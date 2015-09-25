using System;

namespace Overture.Core.Auth.Organizations
{
	public class OrganizationNotFoundException : Exception
	{
		public OrganizationNotFoundException(Guid organizationId):base(string.Format("Organization {0} not found", organizationId))
		{
		}
	}
}