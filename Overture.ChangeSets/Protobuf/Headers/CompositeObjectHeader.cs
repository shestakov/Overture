using System;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Headers
{
	[ProtoContract(SkipConstructor = true)]
	public class CompositeObjectHeader
	{
		public CompositeObjectHeader(Guid id, Guid compositeObjectTypeId, long dateTimeLastUpdated, Guid revision, Guid[] appliedChangeSets, int attributeCount, int simpleObjectCount, long dateTimeCreated, Guid createdByUserId, Guid lastUpdatedByUserId)
		{
			Id = id;
			CompositeObjectTypeId = compositeObjectTypeId;
			DateTimeCreated = dateTimeCreated;
			CreatedByUserId = createdByUserId;
			LastUpdatedByUserId = lastUpdatedByUserId;
			DateTimeLastUpdated = dateTimeLastUpdated;
			Revision = revision;
			AppliedChangeSets = appliedChangeSets;
			AttributeCount = attributeCount;
			SimpleObjectCount = simpleObjectCount;
		}

		[ProtoMember(1)]
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public Guid CompositeObjectTypeId { get; set; }

		[ProtoMember(3)]
		public Guid Revision { get; set; }
		
		[ProtoMember(4)]
		public Guid[] AppliedChangeSets { get; set; }

		[ProtoMember(5)]
		public int AttributeCount { get; set; }

		[ProtoMember(6)]
		public int SimpleObjectCount { get; set; }

		[ProtoMember(7)]
		public long DateTimeCreated { get; set; }

		[ProtoMember(8)]
		public Guid CreatedByUserId { get; set; }

		[ProtoMember(9)]
		public long DateTimeLastUpdated { get; set; }

		[ProtoMember(10)]
		public Guid LastUpdatedByUserId { get; set; }
	}
}