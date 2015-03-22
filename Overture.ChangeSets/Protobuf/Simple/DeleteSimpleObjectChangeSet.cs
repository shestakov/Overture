using System;
using Overture.ChangeSets.Properties;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Simple
{
	[ProtoContract]
	public class DeleteSimpleObjectChangeSet : SimpleObjectChangeSet
	{
		[UsedImplicitly]
		public DeleteSimpleObjectChangeSet()
		{
		}

		public DeleteSimpleObjectChangeSet(Guid changeSetId, Guid simpleObjectId)
			: base(changeSetId, simpleObjectId)
		{
		}

		public override SimpleObjectChangeSetType Action
		{
			get { return SimpleObjectChangeSetType.Delete; }
		}
	}
}