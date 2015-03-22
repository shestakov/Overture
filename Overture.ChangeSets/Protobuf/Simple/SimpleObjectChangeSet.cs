using System;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Simple
{
	[ProtoContract]
	[ProtoInclude(101, typeof (CreateOrUpdateSimpleObjectChangeSet))]
	[ProtoInclude(102, typeof (DeleteSimpleObjectChangeSet))]
	[ProtoInclude(103, typeof (UpdateSimpleObjectParentChangeSet))]
	public abstract class SimpleObjectChangeSet
	{
		protected SimpleObjectChangeSet()
		{
		}

		protected SimpleObjectChangeSet(Guid changeSetId, Guid simpleObjectId)
		{
			ChangeSetId = changeSetId;
			SimpleObjectId = simpleObjectId;
		}

		[ProtoMember(1)]
		public Guid ChangeSetId { get; set; }

		[ProtoMember(2)]
		public Guid SimpleObjectId { get; set; }

		public abstract SimpleObjectChangeSetType Action { get; }
	}
}