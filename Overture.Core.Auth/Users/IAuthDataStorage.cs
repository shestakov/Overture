using System;
using System.Collections.Generic;

namespace Overture.Core.Auth.Users
{
	public interface IAuthDataStorage<TUser, TUserActivationRequest, TChangePasswordRequest>
		where TUser : IUser
		where TUserActivationRequest : IUserActivationRequest
		where TChangePasswordRequest : IChangePasswordRequest
	{
		void CreateUser(TUser user);
		void UpdateUser(TUser user);
		TUser FindUser(Guid userId);
		TUser GetUser(Guid userId);
		TUser FindUserByLogin(string login);

		void CreateActivateUserRequest(TUserActivationRequest request);
		IEnumerable<TUserActivationRequest> FindActivateUserRequestsByUserId(Guid userId);
		TUserActivationRequest FindUserActivationRequest(Guid requestId);
		void UpdateActivateUserRequest(TUserActivationRequest activateUserRequest);

		void CreateChangePasswordRequest(TChangePasswordRequest request);
		TChangePasswordRequest FindChangePasswordRequest(Guid requestId);
		void UpdateChangePasswordRequest(TChangePasswordRequest changePasswordRequest);
	}
}