using System;
using Overture.ChangeSets.Definitions;

namespace Overture.ChangeSets.DefinitionProvider
{
	public interface IBusinessObjectDefinitionProvider
	{
		SimpleObjectDefinition FindSimpleObjectDefinition(Guid compositeObjectTypeId, Guid simpleObjectTypeId);
		CompositeObjectDefinition FindCompositeObjectDefinition(Guid compositeObjectTypeId);
		CompositeObjectDefinition GetCompositeObjectDefinition(Guid compositeObjectTypeId);
	}
}