using System;

namespace Overture.ChangeSets.Attributes
{
	public class SimpleObjectAttribute : Attribute
	{
		public SimpleObjectAttribute(string simpleObjectTypeId)
		{
			SimpleObjectTypeId = new Guid(simpleObjectTypeId);
		}

		public Guid SimpleObjectTypeId { get; private set; }
	}
}