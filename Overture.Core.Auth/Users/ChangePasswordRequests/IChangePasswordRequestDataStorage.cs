using System;

namespace Overture.Core.Auth.Users.ChangePasswordRequests
{
	public interface IChangePasswordRequestDataStorage<TChangePasswordRequest>
		where TChangePasswordRequest : IChangePasswordRequest
	{
		void CreateChangePasswordRequest(TChangePasswordRequest request);
		TChangePasswordRequest FindChangePasswordRequest(Guid requestId);
		void UpdateChangePasswordRequest(TChangePasswordRequest changePasswordRequest);
	}
}