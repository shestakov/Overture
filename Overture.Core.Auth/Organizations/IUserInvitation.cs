using System;

namespace Overture.Core.Auth.Organizations
{
	public interface IUserInvitation
	{
		Guid UserInvitationId { get; set; }
		Guid InivitingUserId { get; set; }
		Guid? InvitedUserId { get; set; }
		string Email { get; set; }
		DateTimeOffset DateTimeCreated { get; set; }
		DateTimeOffset? DateTimeProcessed { get; set; }
		Guid OrganizationId { get; set; }
		UserInvitationStatus Status { get; set; }
		bool Expired { get; }
	}

	public enum UserInvitationStatus
	{
		Created = 0,
		Accepted = 1,
		Cancelled = 2
	}
}