using System;

namespace Overture.Core.Auth.Organizations
{
	public interface IOrganizationManager<out TOrganization, in TOrganizationCreateForm, in TOrganizationUpdateForm>
	{
		TOrganization CreateOrganization(TOrganizationCreateForm form, Guid userId);
		void UpdateOrganization(Guid organizationId, TOrganizationUpdateForm form, Guid userId);
	}

	public interface IUserInvitationManager<out TOrganizationUser, in TCreateUserForm>
	{
		void SendUserInvitation(string email, Guid organizationId, Guid invitingUserId, string organizationTitle, Uri acceptInvitationUrl);
		TOrganizationUser AcceptUserInvitation(Guid userInvitationId, TCreateUserForm form);
		TOrganizationUser AcceptExistingUserInvitation(Guid userInvitationId);
		void CancelUserInvitation(Guid userInvitationId);
	}
}