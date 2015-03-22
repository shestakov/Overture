using System;
using Overture.ChangeSets.Properties;
using Overture.ChangeSets.Protobuf.AttributeValues;
using Overture.ChangeSets.Protobuf.Simple;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Composite
{
	[ProtoContract]
	public class CreateCompositeObjectChangeSet : CompositeObjectChangeSet
	{
		[UsedImplicitly]
		public CreateCompositeObjectChangeSet()
		{
		}

		public CreateCompositeObjectChangeSet(Guid compositeObjectId, Guid changeSetId, long timestamp, Guid compositeObjectTypeId,
			SimpleObjectChangeSet[] childObjectChangeSets, AttributeValue[] attributeValues, Guid userId, long dateTime)
			: base(compositeObjectId, changeSetId, timestamp, userId, dateTime, attributeValues, childObjectChangeSets)
		{
			CompositeObjectTypeId = compositeObjectTypeId;
		}

		[ProtoMember(1)]
		public Guid CompositeObjectTypeId { get; set; }
	}
}