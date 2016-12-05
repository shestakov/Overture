using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Overture.ChangeSets.Attributes;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.Definitions;
using Overture.ChangeSets.SerializerProvider;

namespace Overture.ChangeSets.Protobuf.AttributeValues
{
	public class AttributeValueFactory : IAttributeValueFactory
	{
		private readonly IAttributeValueSerializerProvider attributeValueSerializerProvider;
		private readonly IBusinessObjectDefinitionProvider businessObjectDefinitionProvider;

		public AttributeValueFactory(IAttributeValueSerializerProvider attributeValueSerializerProvider,
			IBusinessObjectDefinitionProvider businessObjectDefinitionProvider)
		{
			this.attributeValueSerializerProvider = attributeValueSerializerProvider;
			this.businessObjectDefinitionProvider = businessObjectDefinitionProvider;
		}

		public AttributeValue Create<T, TP>(Expression<Func<T, TP>> attribute, TP value)
		{
			var member = (MemberExpression) attribute.Body;
			var a = member.Member.GetCustomAttribute<AttributeAttribute>();
			var name = a != null ? a.Name : member.Member.Name;
			var type = member.Type;

			var serializer = attributeValueSerializerProvider.Get(type);
			return new AttributeValue(name, serializer.Serialize(name, type, value));
		}

		public AttributeValue Create<T>(Expression<Func<T>> attribute, T value)
		{
			var member = (MemberExpression) attribute.Body;
			var a = member.Member.GetCustomAttribute<AttributeAttribute>();
			var name = a != null ? a.Name : member.Member.Name;
			var type = member.Type;

			var serializer = attributeValueSerializerProvider.Get(type);
			return new AttributeValue(name, serializer.Serialize(name, type, value));
		}

		public AttributeValue Create(string name, Type type, object value)
		{
			var serializer = attributeValueSerializerProvider.Get(type);
			return new AttributeValue(name, serializer.Serialize(name, type, value));
		}

		public AttributeValue[] MapByName<T>(object dataObject)
		{
			return MapByName(typeof(T), dataObject);
		}

		public AttributeValue[] MapByName(Type businessObjectType, object dataObject)
		{
			var simpleObjectAttribute = businessObjectType.GetCustomAttribute<SimpleObjectAttribute>(true);
			var compositeObjectAttribute = businessObjectType.GetCustomAttribute<CompositeObjectAttribute>(true);
			BusinessObjectDefinition definition;

			if (compositeObjectAttribute != null)
			{
				definition = businessObjectDefinitionProvider.GetCompositeObjectDefinition(compositeObjectAttribute.CompositeObjectTypeId);
			}
			else if (simpleObjectAttribute != null)
			{
				definition = businessObjectDefinitionProvider.FindSimpleObjectDefinition(simpleObjectAttribute.CompositeObjectTypeId, simpleObjectAttribute.SimpleObjectTypeId);
			}
			else
			{
				throw new ArgumentException($"Class {businessObjectType.FullName} is not a business object", nameof(businessObjectType));
			}
			
			var properties = dataObject.GetType().GetProperties().ToDictionary(p => p.Name);
			var attributeValues = definition.Attributes.Values.Where(attribute => properties.ContainsKey(attribute.Name))
				.Select(
					attribute =>
					{
						var attributeName = attribute.Name;
						var propertyType = properties[attribute.Name].PropertyType;
						var attributeType = attribute.AttributeType;
						var dataObjectPropertyValue = properties[attributeName].GetValue(dataObject);
						if (!AssignmentPossible(attributeType, propertyType, dataObjectPropertyValue))
							throw new Exception(
								string.Format("Type of data object property {0} ({1}) does not match type of a business object attribute with the same name ({2})",
									attributeName, propertyType.Name, attributeType.Name));

						var serializer = attribute.Serializer;
						return new AttributeValue(attributeName, serializer.Serialize(attributeName, attributeType, dataObjectPropertyValue));
					});

			return attributeValues.ToArray();
		}

		private static bool AssignmentPossible(Type attributeType, Type propertyType, object propertyValue)
		{
			if (attributeType == propertyType)
				return true;

			if (Nullable.GetUnderlyingType(attributeType) == propertyType)
				return true;

			if (attributeType == Nullable.GetUnderlyingType(propertyType) && propertyValue != null)
				return true;

			return false;
		}
	}
}