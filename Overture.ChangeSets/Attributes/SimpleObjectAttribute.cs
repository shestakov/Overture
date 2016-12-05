using System;

namespace Overture.ChangeSets.Attributes
{
	public class SimpleObjectAttribute : Attribute
	{
		public SimpleObjectAttribute(string compositeObjectTypeId, string simpleObjectTypeId)
		{
			CompositeObjectTypeId = new Guid(compositeObjectTypeId);
			SimpleObjectTypeId = new Guid(simpleObjectTypeId);
		}

		public Guid CompositeObjectTypeId { get; private set; }
		public Guid SimpleObjectTypeId { get; private set; }
	}
}