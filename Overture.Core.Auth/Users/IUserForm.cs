namespace Overture.Core.Auth.Users
{
	public interface IUserForm
	{
		string Login { get; set; }
		string Password { get; set; }
	}
}