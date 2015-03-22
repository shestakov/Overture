using System;

namespace Overture.Core.Auth.Users
{
	public interface IUserActivationRequest
	{
		Guid RequestId { get; set; }
		Guid DraftUserId { get; set; }
		DateTimeOffset ExpirationDateTime { get; set; }
		DateTimeOffset? ActivationDateTime { get; set; }
		bool IsUsed { get; }
		bool Expired { get; }
	}
}