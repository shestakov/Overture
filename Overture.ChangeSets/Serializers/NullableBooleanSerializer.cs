using System;

namespace Overture.ChangeSets.Serializers
{
	public class NullableBooleanSerializer : AttributeValueBinarySerializer<bool?>
	{
		protected override bool CanBeNull
		{
			get { return true; }
		}

		protected override byte[] SerializeInternal(string name, Type type, bool? value)
		{
			return BitConverter.GetBytes(value.Value);
		}

		protected override bool? DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			return BitConverter.ToBoolean(serializedValue, 0);
		}
	}
}