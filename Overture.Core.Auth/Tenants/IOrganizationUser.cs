using System;

namespace Overture.Core.Auth.Tenants
{
	public interface IOrganizationUser
	{
		Guid OrganizationUserId { get; set; }
		Guid UserId { get; set; }
		Guid OrganizationId { get; set; }
		bool IsAdministrator { get; set; }
		DateTimeOffset DateTimeCreated { get; set; }
		DateTimeOffset? DateTimeDeleted { get; set; }
	}
}