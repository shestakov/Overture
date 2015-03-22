using System;

namespace Overture.ChangeSets.Serializers
{
	public class GuidSerializer : AttributeValueBinarySerializer<Guid>
	{
		protected override bool CanBeNull
		{
			get { return false; }
		}

		protected override byte[] SerializeInternal(string name, Type type, Guid value)
		{
			return (value).ToByteArray();
		}

		protected override Guid DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			return new Guid(serializedValue);
		}
	}
}