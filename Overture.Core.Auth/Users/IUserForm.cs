using System;

namespace Overture.Core.Auth.Users
{
	public interface IUserForm
	{
		string Login { get; set; }
		string Password { get; set; }
	}

	public interface IUserProcessor<TUser, in TCreateUserForm, in TUpdateUserForm>
	{
		TUser MakeUserFromForm(TCreateUserForm form, Guid? userId);
		void Update(TUser user, TUpdateUserForm form);
	}
}