using System;
using Overture.ChangeSets.Protobuf.AttributeValues;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Simple
{
	[ProtoContract]
	[ProtoInclude(101, typeof (CreateSimpleObjectChangeSet))]
	[ProtoInclude(102, typeof (UpdateSimpleObjectChangeSet))]
	public abstract class CreateOrUpdateSimpleObjectChangeSet : SimpleObjectChangeSet
	{
		protected CreateOrUpdateSimpleObjectChangeSet(Guid changeSetId, Guid simpleObjectId,
			AttributeValue[] attributeValues)
			: base(changeSetId, simpleObjectId)
		{
			AttributeValues = attributeValues;
		}

		protected CreateOrUpdateSimpleObjectChangeSet()
		{
			AttributeValues = new AttributeValue[0];
		}

		[ProtoMember(1)]
		public AttributeValue[] AttributeValues { get; set; }
	}
}