namespace Overture.Core.Auth.BruteForceProtection
{
	public interface ILoginBruteForceProtector
	{
		bool CheckAttemptAllowed(string ip);
		void AddFailAttempt(string ip);
		void ClearAttemptsForIp(string ip);
	}
}