using System;

namespace Overture.ChangeSets.Attributes
{
	public class CompositeObjectAttribute : Attribute
	{
		public CompositeObjectAttribute(string compositeObjectTypeId)
		{
			CompositeObjectTypeId = new Guid(compositeObjectTypeId);
		}

		public Guid CompositeObjectTypeId { get; private set; }
	}
}