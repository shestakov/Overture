using System;

namespace Overture.Core.Auth.Users
{
	public interface IChangePasswordRequest
	{
		Guid RequestId { get; set; }
		Guid UserId { get; set; }
		DateTimeOffset ExpirationDateTime { get; set; }
		DateTimeOffset? PasswordChangeDateTime { get; set; }
		bool IsUsed { get; }
		bool Expired { get; }
	}
}