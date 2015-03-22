using System;

namespace Overture.ChangeSets.Storage
{
	//Must be singleton
	public class TimestampProvider : ITimestampProvider
	{
		private long previousTimestamp;

		public long GetTimestamp()
		{
			var currentTimestamp = DateTimeOffset.UtcNow.Ticks;
			if (currentTimestamp <= previousTimestamp)
			{
				currentTimestamp = previousTimestamp + 1;
			}

			previousTimestamp = currentTimestamp;

			return currentTimestamp;
		}
	}
}