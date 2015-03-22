using System;

namespace Overture.Core.Auth.Users
{
	public interface IUser
	{
		Guid UserId { get; set; }
		string Login { get; set; }
		string PasswordHash { get; set; }
		string PasswordSalt { get; set; }
		DateTimeOffset? DateTimeActivated { get; set; }
		bool IsActive { get; }
	}
}