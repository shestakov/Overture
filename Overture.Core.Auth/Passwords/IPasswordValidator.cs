namespace Overture.Core.Auth.Passwords
{
	public interface IPasswordValidator
	{
		bool Validate(string password);
	}
}