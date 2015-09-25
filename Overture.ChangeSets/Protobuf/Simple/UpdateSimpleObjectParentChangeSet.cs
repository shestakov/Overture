using System;
using Overture.ChangeSets.Properties;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Simple
{
	[ProtoContract]
	public class UpdateSimpleObjectParentChangeSet : SimpleObjectChangeSet
	{
		[UsedImplicitly]
		public UpdateSimpleObjectParentChangeSet()
		{
		}

		public UpdateSimpleObjectParentChangeSet(Guid changeSetId, Guid simpleObjectId, Guid? parentId)
			: base(changeSetId, simpleObjectId)
		{
			ParentId = parentId;
		}

		[ProtoMember(1)]
		public Guid? ParentId { get; set; }

		public override SimpleObjectChangeSetType Action
		{
			get { return SimpleObjectChangeSetType.UpdateParent; }
		}
	}
}