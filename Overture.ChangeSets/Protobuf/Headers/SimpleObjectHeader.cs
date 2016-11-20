using System;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Headers
{
	[ProtoContract(SkipConstructor = true)]
	public class SimpleObjectHeader
	{
		public SimpleObjectHeader(Guid id, Guid simpleObjectTypeId, Guid? parentId, Guid revision, long dateTimeCreated, Guid createdByUserId, long dateTimeLastUpdated, Guid lastUpdatedByUserId)
		{
			Id = id;
			SimpleObjectTypeId = simpleObjectTypeId;
			ParentId = parentId;
			Revision = revision;
			DateTimeCreated = dateTimeCreated;
			CreatedByUserId = createdByUserId;
			DateTimeLastUpdated = dateTimeLastUpdated;
			LastUpdatedByUserId = lastUpdatedByUserId;
		}

		[ProtoMember(1)]
		public Guid Id { get; set; }
		
		[ProtoMember(2)]
		public Guid SimpleObjectTypeId { get; set; }
		
		[ProtoMember(3)]
		public Guid? ParentId { get; set; }

		[ProtoMember(4)]
		public Guid Revision { get; set; }

		[ProtoMember(5)]
		public long DateTimeCreated { get; set; }

		[ProtoMember(6)]
		public Guid CreatedByUserId { get; set; }

		[ProtoMember(7)]
		public long DateTimeLastUpdated { get; set; }

		[ProtoMember(8)]
		public Guid LastUpdatedByUserId { get; set; }
	}
}