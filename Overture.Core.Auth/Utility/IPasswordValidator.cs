namespace Overture.Core.Auth.Utility
{
	public interface IPasswordValidator
	{
		bool Validate(string password);
	}
}