using System;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Headers
{
	[ProtoContract(SkipConstructor = true)]
	public class CompositeObjectHeader
	{
		public CompositeObjectHeader(Guid id, Guid compositeObjectTypeId, long lastModified, Guid revision, Guid[] appliedChangeSets, int attributeCount, int simpleObjectCount)
		{
			Id = id;
			CompositeObjectTypeId = compositeObjectTypeId;
			LastModified = lastModified;
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
		public long LastModified { get; set; }

		[ProtoMember(5)]
		public Guid[] AppliedChangeSets { get; set; }

		[ProtoMember(6)]
		public int AttributeCount { get; set; }

		[ProtoMember(7)]
		public int SimpleObjectCount { get; set; }
	}
}