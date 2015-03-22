using System;

namespace Overture.ChangeSets.Serializers
{
	public class NullableDateTimeOffsetSerializer : AttributeValueBinarySerializer<DateTimeOffset?>
	{
		protected override bool CanBeNull
		{
			get { return true; }
		}

		protected override byte[] SerializeInternal(string name, Type type, DateTimeOffset? value)
		{
			return BitConverter.GetBytes(value.Value.Ticks);
		}

		protected override DateTimeOffset? DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			return new DateTimeOffset(BitConverter.ToInt64(serializedValue, 0), new TimeSpan(0));
		}
	}
}