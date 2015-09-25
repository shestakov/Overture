using System;
using System.Collections.Generic;

namespace Overture.Core.Auth.Users.Storage
{
	public interface IUserActivationRequestDataStorage<TUserActivationRequest>
		where TUserActivationRequest : IUserActivationRequest
	{
		void CreateActivateUserRequest(TUserActivationRequest request);
		IEnumerable<TUserActivationRequest> FindActivateUserRequestsByUserId(Guid userId);
		TUserActivationRequest FindUserActivationRequest(Guid requestId);
		void UpdateActivateUserRequest(TUserActivationRequest activateUserRequest);
	}
}