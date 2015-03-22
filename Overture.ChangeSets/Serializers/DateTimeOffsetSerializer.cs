using System;

namespace Overture.ChangeSets.Serializers
{
	public class DateTimeOffsetSerializer : AttributeValueBinarySerializer<DateTimeOffset>
	{
		protected override bool CanBeNull
		{
			get { return false; }
		}

		protected override byte[] SerializeInternal(string name, Type type, DateTimeOffset value)
		{
			return BitConverter.GetBytes(value.Ticks);
		}

		protected override DateTimeOffset DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			return new DateTimeOffset(BitConverter.ToInt64(serializedValue, 0), new TimeSpan(0));
		}
	}
}