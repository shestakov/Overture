namespace Overture.Core.Auth.BruteForceProtection
{
	public class PasswordBruteForceProtector : BruteForceProtectorBase, IPasswordBruteForceProtector
	{
		private const int maxAttemptsCount = 3;
		private const int attemptsIntervalMinutes = 1;
		public const int BanTimeMinutes = 1;

		public PasswordBruteForceProtector()
			: base(attemptsIntervalMinutes, maxAttemptsCount, BanTimeMinutes)
		{
		}
	}
}