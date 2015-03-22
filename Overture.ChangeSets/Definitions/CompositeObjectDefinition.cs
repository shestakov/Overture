using System;
using System.Collections.Generic;

namespace Overture.ChangeSets.Definitions
{
	public class CompositeObjectDefinition : BusinessObjectDefinition
	{
		public CompositeObjectDefinition(Guid compositeObjectTypeId, IEnumerable<AttributeDefinition> attributes)
			: base(attributes)
		{
			CompositeObjectTypeId = compositeObjectTypeId;
		}

		public Guid CompositeObjectTypeId { get; private set; }
	}
}