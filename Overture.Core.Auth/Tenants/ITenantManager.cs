using System;

namespace Overture.Core.Auth.Tenants
{
	public interface ITenantManager<out TOrganization, in TOrganizationCreateForm, in TOrganizationUpdateForm, out TOrganizationUser, in TCreateUserForm>
	{
		TOrganization CreateOrganization(TOrganizationCreateForm form, Guid userId);
		void UpdateOrganization(Guid organizationId, TOrganizationUpdateForm form, Guid userId);

		void SendUserInvitation(string email, Guid organizationId, Guid invitingUserId, string organizationTitle, Uri acceptInvitationUrl);
		TOrganizationUser AcceptUserInvitation(Guid userInvitationId, TCreateUserForm form);
		TOrganizationUser AcceptExistingUserInvitation(Guid userInvitationId);
		void CancelUserInvitation(Guid userInvitationId);
	}
}