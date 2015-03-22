using System;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Headers
{
	[ProtoContract(SkipConstructor = true)]
	public class SimpleObjectHeader
	{
		public SimpleObjectHeader(Guid id, Guid simpleObjectTypeId, Guid? parentId, Guid revision)
		{
			Id = id;
			SimpleObjectTypeId = simpleObjectTypeId;
			ParentId = parentId;
			Revision = revision;
		}

		[ProtoMember(1)]
		public Guid Id { get; set; }
		
		[ProtoMember(2)]
		public Guid SimpleObjectTypeId { get; set; }
		
		[ProtoMember(3)]
		public Guid? ParentId { get; set; }

		[ProtoMember(4)]
		public Guid Revision { get; set; }
	}
}