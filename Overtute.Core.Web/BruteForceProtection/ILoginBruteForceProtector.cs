namespace Overtute.Core.Web.BruteForceProtection
{
	public interface ILoginBruteForceProtector
	{
		bool CheckAttemptAllowed(string ip);
		void AddFailAttempt(string ip);
		void ClearAttemptsForIp(string ip);
	}
}