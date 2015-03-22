using System;
using Overture.ChangeSets.Properties;
using Overture.ChangeSets.Protobuf.AttributeValues;
using Overture.ChangeSets.Protobuf.Simple;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Composite
{
	[ProtoContract]
	public class UpdateCompositeObjectChangeSet : CompositeObjectChangeSet
	{
		[UsedImplicitly]
		public UpdateCompositeObjectChangeSet()
		{
		}

		public UpdateCompositeObjectChangeSet(Guid compositeObjectId, Guid changeSetId, long timestamp,
			SimpleObjectChangeSet[] childObjectChangeSets, AttributeValue[] attributeValues, Guid userId, long dateTime)
			: base(compositeObjectId, changeSetId, timestamp, userId, dateTime, attributeValues, childObjectChangeSets)
		{
		}
	}
}