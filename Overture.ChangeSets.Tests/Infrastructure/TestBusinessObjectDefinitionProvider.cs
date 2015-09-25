using System;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.Definitions;
using Overture.ChangeSets.SerializerProvider;

namespace Overture.ChangeSets.Tests.Infrastructure
{
	internal class TestBusinessObjectDefinitionProvider : IBusinessObjectDefinitionProvider
	{
		private readonly BusinessObjectDefinitionProvider provider;

		public TestBusinessObjectDefinitionProvider()
		{
			provider = new BusinessObjectDefinitionProvider(new AttributeValueSerializerProvider(), new TestTypeRetriever());
		}

		public SimpleObjectDefinition FindSimpleObjectDefinition(Guid simpleObjectTypeId)
		{
			return provider.FindSimpleObjectDefinition(simpleObjectTypeId);
		}

		public CompositeObjectDefinition FindCompositeObjectDefinition(Guid compositeObjectTypeId)
		{
			return provider.FindCompositeObjectDefinition(compositeObjectTypeId);
		}

		public CompositeObjectDefinition GetCompositeObjectDefinition(Guid compositeObjectTypeId)
		{
			return provider.GetCompositeObjectDefinition(compositeObjectTypeId);
		}
	}
}