using System;
using Overture.ChangeSets.Properties;
using Overture.ChangeSets.Protobuf.AttributeValues;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Simple
{
	[ProtoContract]
	public class UpdateSimpleObjectChangeSet : CreateOrUpdateSimpleObjectChangeSet
	{
		[UsedImplicitly]
		public UpdateSimpleObjectChangeSet()
		{
		}

		public UpdateSimpleObjectChangeSet(Guid changeSetId, Guid simpleObjectId, AttributeValue[] attributeValues)
			: base(changeSetId, simpleObjectId, attributeValues)
		{
		}

		public override SimpleObjectChangeSetType Action
		{
			get { return SimpleObjectChangeSetType.Update; }
		}
	}
}