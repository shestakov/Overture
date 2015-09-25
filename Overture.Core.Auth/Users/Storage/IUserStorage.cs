using System;

namespace Overture.Core.Auth.Users.Storage
{
	public interface IUserStorage<TUser>
		where TUser : IUser
	{
		void CreateUser(TUser user);
		void UpdateUser(TUser user);
		TUser FindUser(Guid userId);
		TUser GetUser(Guid userId);
		TUser FindUserByLogin(string login);
	}
}