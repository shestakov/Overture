using System;

namespace Overture.ChangeSets.Serializers
{
	public class NullableInt64Serializer : AttributeValueBinarySerializer<Int64?>
	{
		protected override bool CanBeNull
		{
			get { return true; }
		}

		protected override byte[] SerializeInternal(string name, Type type, long? value)
		{
			return BitConverter.GetBytes(value.Value);
		}

		protected override long? DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			return BitConverter.ToInt64(serializedValue, 0);
		}
	}
}