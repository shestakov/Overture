using System;

namespace Overture.Core.Auth.Tenants
{
	public interface IOrganizationProcessor<TOrganization, in TOrganizationCreateForm, in TOrganizationUpdateForm>
	{
		TOrganization Create(TOrganizationCreateForm form, Guid? organizationId);
		void Update(TOrganization organization, TOrganizationUpdateForm form);
	}
}