using System;

namespace Overture.ChangeSets.Serializers
{
	public class EnumSerializer : AttributeValueBinarySerializer<Enum>
	{
		protected override bool CanBeNull
		{
			get { return false; }
		}

		protected override byte[] SerializeInternal(string name, Type type, Enum value)
		{
			var int32Value = Convert.ToInt32(value);
			return BitConverter.GetBytes(int32Value);
		}

		protected override Enum DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			var int32 = BitConverter.ToInt32(serializedValue, 0);
			return (Enum) Enum.ToObject(type, int32);
		}
	}
}