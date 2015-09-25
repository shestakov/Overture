using System;

namespace Overture.Core.Auth.Users
{
	public interface IUserProcessor<TUser, in TCreateUserForm, in TUpdateUserForm>
	{
		TUser MakeUser(TCreateUserForm form, Guid? userId);
		void Update(TUser user, TUpdateUserForm form);
	}
}