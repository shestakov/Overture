using System;
using Overture.Core.Auth.Users;

namespace Overture.Core.Auth.Organizations
{
	public class OrganizationManager<TOrganization, TOrganizationCreateForm, TOrganizationUpdateForm, TOrganizationUser, TUser> 
		: IOrganizationManager<TOrganization, TOrganizationCreateForm, TOrganizationUpdateForm> 
		where TOrganization: class
		where TOrganizationUser: IOrganizationUser, new()
		where TUser: class, IUser
	{
		private readonly IOrganizationStorage<TOrganization> organizationStorage;
		private readonly IOrganizationUserStorage<TOrganizationUser, TUser> organizationUserStorage;
		private readonly IOrganizationProcessor<TOrganization, TOrganizationCreateForm, TOrganizationUpdateForm> organizationProcessor;

		public OrganizationManager(IOrganizationStorage<TOrganization> organizationStorage, IOrganizationProcessor<TOrganization, TOrganizationCreateForm, TOrganizationUpdateForm> organizationProcessor, IOrganizationUserStorage<TOrganizationUser, TUser> organizationUserStorage)
		{
			this.organizationStorage = organizationStorage;
			this.organizationProcessor = organizationProcessor;
			this.organizationUserStorage = organizationUserStorage;
		}

		public TOrganization CreateOrganization(TOrganizationCreateForm form, Guid userId)
		{
			var organizationId = Guid.NewGuid();
			var organization = organizationProcessor.Create(form, organizationId);
			organizationStorage.CreateOrganization(organization);
			
			var organizationUser = new TOrganizationUser
			{
				OrganizationUserId = Guid.NewGuid(),
				OrganizationId = organizationId,
				UserId = userId,
				DateTimeCreated = DateTimeOffset.UtcNow,
				IsAdministrator = true
			};
			organizationUserStorage.CreateOrganizationUser(organizationUser);

			return organizationStorage.FindOrganization(organizationId);
		}

		public void UpdateOrganization(Guid organizationId, TOrganizationUpdateForm form, Guid userId)
		{
			var organization = organizationStorage.FindOrganization(organizationId);
			if (organization == null)
				throw new OrganizationNotFoundException(organizationId);

			organizationProcessor.Update(organization, form);
			organizationStorage.UpdateOrganization(organization);
		}
	}
}