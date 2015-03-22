namespace Overtute.Core.Web.BruteForceProtection
{
	public class LoginBruteForceProtector : BruteForceProtectorBase, ILoginBruteForceProtector
	{
		private const int maxAttemptsCount = 10;
		private const int attemptsIntervalMinutes = 1;
		public const int BanTimeMinutes = 1;

		public LoginBruteForceProtector()
			: base(attemptsIntervalMinutes, maxAttemptsCount, BanTimeMinutes)
		{
		}
	}
}