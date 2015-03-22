using System;
using Overture.ChangeSets.Properties;
using Overture.ChangeSets.Protobuf.AttributeValues;
using ProtoBuf;

namespace Overture.ChangeSets.Protobuf.Simple
{
	[ProtoContract]
	public class CreateSimpleObjectChangeSet : CreateOrUpdateSimpleObjectChangeSet
	{
		[UsedImplicitly]
		public CreateSimpleObjectChangeSet()
		{
		}

		public CreateSimpleObjectChangeSet(Guid changeSetId, Guid simpleObjectId, AttributeValue[] attributeValues, Guid simpleObjectType, Guid? parentId)
			: base(changeSetId, simpleObjectId, attributeValues)
		{
			SimpleObjectType = simpleObjectType;
			ParentId = parentId;
		}

		[ProtoMember(1)]
		public Guid SimpleObjectType { get; set; }

		[ProtoMember(2)]
		public Guid? ParentId { get; set; }

		public override SimpleObjectChangeSetType Action
		{
			get { return SimpleObjectChangeSetType.Create; }
		}
	}
}