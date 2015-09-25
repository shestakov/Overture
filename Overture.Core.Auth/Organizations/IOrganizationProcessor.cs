using System;

namespace Overture.Core.Auth.Organizations
{
	public interface IOrganizationProcessor<TOrganization, in TOrganizationCreateForm, in TOrganizationUpdateForm>
	{
		TOrganization Create(TOrganizationCreateForm form, Guid? organizationId);
		void Update(TOrganization organization, TOrganizationUpdateForm form);
	}
}