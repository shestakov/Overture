using System;
using System.Collections.Generic;
using Overture.Core.Auth.Users;

namespace Overture.Core.Auth.Organizations
{
	public interface IOrganizationStorage<TOrganization>
	{
		void CreateOrganization(TOrganization organization);
		void UpdateOrganization(TOrganization organization);
		TOrganization FindOrganization(Guid organizationId);
		IEnumerable<TOrganization> GetAllOrganizations();
	}

	public interface IOrganizationUserStorage<TOrganizationUser, out TUser>
		where TOrganizationUser: IOrganizationUser
		where TUser: IUser
	{
		void CreateOrganizationUser(TOrganizationUser organizationUser);
		void UpdateOrganizationUser(TOrganizationUser organizationUser);
		void DeleteOrganizationUser(Guid organizationUserId);
		TOrganizationUser FindOrganizationUser(Guid organizationUserId);
		IEnumerable<TOrganizationUser> FindOrganizationUsersByUser(Guid userId);
		IEnumerable<TOrganizationUser> FindOrganizationUsersByOrganization(Guid organizationId);
		IEnumerable<TUser> FindActiveUsersByOrganization(Guid organizationId);
	}

	public interface IUserInvitationStorage<TUserInvitation>
		where TUserInvitation: IUserInvitation
	{
		IEnumerable<TUserInvitation> GetUserInvitations(Guid organizationId);
		void CreateUserInvitation(TUserInvitation userInvitation);
		TUserInvitation FindUserInvitation(Guid userInvitationId);
		void UpdateUserInvitation(TUserInvitation userInvitation);
	}
}