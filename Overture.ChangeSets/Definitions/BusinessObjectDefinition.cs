using System.Collections.Generic;
using System.Linq;

namespace Overture.ChangeSets.Definitions
{
	public abstract class BusinessObjectDefinition
	{
		protected BusinessObjectDefinition(IEnumerable<AttributeDefinition> attributes)
		{
			Attributes = attributes.ToDictionary(a => a.Name);
		}

		public IDictionary<string, AttributeDefinition> Attributes { get; private set; }
	}
}