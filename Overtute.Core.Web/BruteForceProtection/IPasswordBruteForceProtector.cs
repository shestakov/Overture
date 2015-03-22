namespace Overtute.Core.Web.BruteForceProtection
{
	public interface IPasswordBruteForceProtector
	{
		bool CheckAttemptAllowed(string login);
		void AddFailAttempt(string login);
		void ClearAttemptsForIp(string login);
	}
}