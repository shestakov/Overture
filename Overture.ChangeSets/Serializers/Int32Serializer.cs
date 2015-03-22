using System;

namespace Overture.ChangeSets.Serializers
{
	public class Int32Serializer : AttributeValueBinarySerializer<Int32>
	{
		protected override bool CanBeNull
		{
			get { return false; }
		}

		protected override byte[] SerializeInternal(string name, Type type, int value)
		{
			return BitConverter.GetBytes(value);
		}

		protected override int DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			return BitConverter.ToInt32(serializedValue, 0);
		}
	}
}