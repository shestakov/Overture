using System;
using System.Collections.Generic;
using System.Linq;

namespace Overture.Core.Auth.BruteForceProtection
{
	public abstract class BruteForceProtectorBase
	{
		private readonly int attemptsIntervalMinutes;
		private readonly int maxAttemptsCount;
		private readonly int banTimeMinutes;
		private readonly Dictionary<string, IList<DateTimeOffset>> attemptsByIp;
		private readonly Dictionary<string, DateTimeOffset> banList;
		private readonly object loker = new object();

		protected BruteForceProtectorBase(int attemptsIntervalMinutes, int maxAttemptsCount, int banTimeMinutes)
		{
			this.attemptsIntervalMinutes = attemptsIntervalMinutes;
			this.maxAttemptsCount = maxAttemptsCount;
			this.banTimeMinutes = banTimeMinutes;
			attemptsByIp = new Dictionary<string, IList<DateTimeOffset>>();
			banList = new Dictionary<string, DateTimeOffset>();
		}

		public bool CheckAttemptAllowed(string key)
		{
			lock (loker)
			{
				return !banList.ContainsKey(key) || banList[key] + TimeSpan.FromMinutes(banTimeMinutes) < DateTime.UtcNow;
			}
		}

		public void AddFailAttempt(string key)
		{
			lock (loker)
			{
				if (!attemptsByIp.ContainsKey(key))
					attemptsByIp.Add(key, new List<DateTimeOffset>());

				var now = DateTimeOffset.UtcNow;
				attemptsByIp[key].Add(now);

				if (attemptsByIp[key].Count(e => e > now - TimeSpan.FromMinutes(attemptsIntervalMinutes)) < maxAttemptsCount)
					return;

				if (banList.ContainsKey(key))
					banList[key] = now;
				else
					banList.Add(key, now);

				attemptsByIp.Remove(key);
			}
		}

		public void ClearAttemptsForIp(string key)
		{
			lock (loker)
			{
				if (attemptsByIp.ContainsKey(key))
					attemptsByIp.Remove(key);
			}
		}
	}
}