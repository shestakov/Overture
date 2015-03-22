using System;

namespace Overture.ChangeSets.Serializers
{
	public class DoubleSerializer : AttributeValueBinarySerializer<Double>
	{
		protected override byte[] SerializeInternal(string name, Type type, double value)
		{
			return BitConverter.GetBytes(value);
		}

		protected override double DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			return BitConverter.ToDouble(serializedValue, 0);
		}

		protected override bool CanBeNull
		{
			get { return false; }
		}
	}
}