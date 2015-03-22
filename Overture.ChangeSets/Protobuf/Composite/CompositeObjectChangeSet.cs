using System;
using Overture.ChangeSets.Protobuf.AttributeValues;
using Overture.ChangeSets.Protobuf.Simple;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Composite
{
	[ProtoContract]
	[ProtoInclude(8, typeof(CreateCompositeObjectChangeSet))]
	[ProtoInclude(9, typeof(UpdateCompositeObjectChangeSet))]
	public abstract class CompositeObjectChangeSet
	{
		protected CompositeObjectChangeSet()
		{
			ChildObjectChangeSets = new SimpleObjectChangeSet[0];
			AttributeValues = new AttributeValue[0];
		}

		[ProtoMember(1)]
		public Guid CompositeObjectId { get; set; }
		
		[ProtoMember(2)]
		public Guid ChangeSetId { get; set; }

		[ProtoMember(3)]
		public long Timestamp { get; set; }

		[ProtoMember(4)]
		public Guid? UserId { get; set; }

		[ProtoMember(5)]
		public long DateTime { get; set; }

		[ProtoMember(6)]
		public AttributeValue[] AttributeValues { get; set; }

		[ProtoMember(7)]
		public SimpleObjectChangeSet[] ChildObjectChangeSets { get; set; }

		protected CompositeObjectChangeSet(Guid compositeObjectId, Guid changeSetId, long timestamp, Guid userId, long dateTime, AttributeValue[] attributeValues, SimpleObjectChangeSet[] childObjectChangeSets)
		{
			CompositeObjectId = compositeObjectId;
			ChangeSetId = changeSetId;
			Timestamp = timestamp;
			UserId = userId;
			DateTime = dateTime;
			AttributeValues = attributeValues;
			ChildObjectChangeSets = childObjectChangeSets;
		}
	}
}