using System;
using Overture.ChangeSets.Definitions;

namespace Overture.ChangeSets.DefinitionProvider
{
	public interface IBusinessObjectDefinitionProvider
	{
		SimpleObjectDefinition FindSimpleObjectDefinition(Guid simpleObjectTypeId);
		CompositeObjectDefinition GetCompositeObjectDefinition(Guid compositeObjectTypeId);
		void AddSimpleObjectDefinition(SimpleObjectDefinition definition);
	}
}