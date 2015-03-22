using System;

namespace Overture.ChangeSets.Serializers
{
	public class NullableDoubleSerializer : AttributeValueBinarySerializer<double?>
	{
		protected override byte[] SerializeInternal(string name, Type type, double? value)
		{
			return BitConverter.GetBytes(value.Value);
		}

		protected override double? DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			return BitConverter.ToDouble(serializedValue, 0);
		}

		protected override bool CanBeNull { get { return true; } }
	}
}