using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Overture.ChangeSets.Attributes;
using Overture.ChangeSets.Definitions;
using Overture.ChangeSets.SerializerProvider;

namespace Overture.ChangeSets.DefinitionProvider
{
	public class BusinessObjectDefinitionProvider : IBusinessObjectDefinitionProvider
	{
		private readonly IAttributeValueSerializerProvider serializerProvider;
		private readonly ITypeRetriever typeRetriever;
		private Dictionary<Guid, CompositeObjectDefinition> compositeObjectDefinitions;
		private Dictionary<Guid, SimpleObjectDefinition> simpleObjectDefinitions;

		public BusinessObjectDefinitionProvider(IAttributeValueSerializerProvider serializerProvider, ITypeRetriever typeRetriever)
		{
			this.serializerProvider = serializerProvider;
			this.typeRetriever = typeRetriever;
			Initialize();
		}

		public SimpleObjectDefinition FindSimpleObjectDefinition(Guid simpleObjectTypeId)
		{
			return simpleObjectDefinitions.ContainsKey(simpleObjectTypeId) ? simpleObjectDefinitions[simpleObjectTypeId] : null;
		}

		public CompositeObjectDefinition GetCompositeObjectDefinition(Guid compositeObjectTypeId)
		{
			if(compositeObjectDefinitions.ContainsKey(compositeObjectTypeId))
				return compositeObjectDefinitions[compositeObjectTypeId];
			throw new ArgumentException(string.Format("CompositeObjectType {0} not found", compositeObjectTypeId), "compositeObjectTypeId");
		}

		public void AddSimpleObjectDefinition(SimpleObjectDefinition definition)
		{
			simpleObjectDefinitions.Add(definition.SimpleObjectTypeId, definition);
		}

		private void Initialize()
		{
			simpleObjectDefinitions = new Dictionary<Guid, SimpleObjectDefinition>();
			compositeObjectDefinitions = new Dictionary<Guid, CompositeObjectDefinition>();

			var types = typeRetriever.GetPublicTypesOfLoadedAssemblies();
			foreach(var type in types)
			{
				var simpleObjectAttribute = type.GetCustomAttribute<SimpleObjectAttribute>(true);
				var compositeObjectAttribute = type.GetCustomAttribute<CompositeObjectAttribute>(true);

				if(simpleObjectAttribute != null)
					simpleObjectDefinitions.Add(simpleObjectAttribute.SimpleObjectTypeId, BuildSimpleObjectDefinition(simpleObjectAttribute.SimpleObjectTypeId, type));

				if(compositeObjectAttribute != null)
					compositeObjectDefinitions.Add(compositeObjectAttribute.CompositeObjectTypeId,
						BuildCompositeObjectDefinition(compositeObjectAttribute.CompositeObjectTypeId, type));
			}
		}

		private CompositeObjectDefinition BuildCompositeObjectDefinition(Guid compositeObjectTypeId, Type type)
		{
			var attributeDefinitions = GetAttributeDefinitions(type);
			return new CompositeObjectDefinition(compositeObjectTypeId, attributeDefinitions);
		}

		private SimpleObjectDefinition BuildSimpleObjectDefinition(Guid simpleObjectTypeId, Type type)
		{
			var attributeDefinitions = GetAttributeDefinitions(type);
			return new SimpleObjectDefinition(simpleObjectTypeId, attributeDefinitions);
		}

		private IEnumerable<AttributeDefinition> GetAttributeDefinitions(Type type)
		{
			if(type.IsAbstract && type.IsSealed) //BusinessObject is defined through a static class and fields
			{
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
				return fields.Select(f => new AttributeDefinition(f.Name, f.FieldType, serializerProvider.Get(f.FieldType)));
			}
			
			//BusinessObject is defined through a concrete class and properties
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			return properties.Select(p => new AttributeDefinition(p.Name, p.PropertyType, serializerProvider.Get(p.PropertyType)));
		}
	}
}