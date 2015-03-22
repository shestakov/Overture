using Overture.ChangeSets.Properties;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.AttributeValues
{
	[ProtoContract]
	public class AttributeValue
	{
		[UsedImplicitly]
		public AttributeValue()
		{
		}

		public AttributeValue(string name, byte[] value)
		{
			Name = name;
			Value = value;
		}

		[ProtoMember(1)]
		public string Name { get; private set; }

		[ProtoMember(2)]
		public byte[] Value { get; private set; }
	}
}