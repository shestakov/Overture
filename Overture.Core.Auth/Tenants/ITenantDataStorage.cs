using System;
using System.Collections.Generic;

namespace Overture.Core.Auth.Tenants
{
	public interface ITenantDataStorage<TOrganization, TOrganizationUser, TUserInvitation, out TUser>
	{
		void CreateOrganization(TOrganization organization);
		void UpdateOrganization(TOrganization organization);
		TOrganization FindOrganization(Guid organizationId);
		IEnumerable<TOrganization> GetAllOrganizations();

		void CreateOrganizationUser(TOrganizationUser organizationUser);
		void UpdateOrganizationUser(TOrganizationUser organizationUser);
		void DeleteOrganizationUser(Guid organizationUserId);
		TOrganizationUser FindOrganizationUser(Guid organizationUserId);
		IEnumerable<TOrganizationUser> FindOrganizationUsersByUser(Guid userId);
		IEnumerable<TOrganizationUser> FindOrganizationUsersByOrganization(Guid organizationId);
		IEnumerable<TUser> FindActiveUsersByOrganization(Guid organizationId);

		IEnumerable<TUserInvitation> GetUserInvitations(Guid organizationId);
		void CreateUserInvitation(TUserInvitation userInvitation);
		TUserInvitation FindUserInvitation(Guid userInvitationId);
		void UpdateUserInvitation(TUserInvitation userInvitation);
	}
}