using System;
using System.Collections.Generic;

namespace Overture.ChangeSets.Definitions
{
	public class SimpleObjectDefinition : BusinessObjectDefinition
	{
		public SimpleObjectDefinition(Guid simpleObjectTypeId, IEnumerable<AttributeDefinition> attributes) : base(attributes)
		{
			SimpleObjectTypeId = simpleObjectTypeId;
		}

		public Guid SimpleObjectTypeId { get; private set; }
	}
}