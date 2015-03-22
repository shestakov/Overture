using System;

namespace Overture.ChangeSets.Serializers
{
	public class NullableGuidSerializer : AttributeValueBinarySerializer<Guid?>
	{
		protected override bool CanBeNull
		{
			get { return true; }
		}

		protected override byte[] SerializeInternal(string name, Type type, Guid? value)
		{
			return (value.Value).ToByteArray();
		}

		protected override Guid? DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			return new Guid(serializedValue);
		}
	}
}