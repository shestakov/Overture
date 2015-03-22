using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using Overture.ChangeSets.Attributes;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.Definitions;
using Overture.ChangeSets.Protobuf.AttributeValues;
using Overture.ChangeSets.Protobuf.Headers;
using Overture.ChangeSets.Protobuf.Simple;
using ProtoBuf;

namespace Overture.ChangeSets.BusinessObjects
{
	public class SimpleObject
	{
		private readonly Dictionary<AttributeDefinition, object> attributes = new Dictionary<AttributeDefinition, object>();
		public readonly ReadOnlyDictionary<AttributeDefinition, object> Attributes;
		private readonly SimpleObjectDefinition simpleObjectDefinition;

		public SimpleObject(Guid id, Guid simpleObjectTypeId, Guid? parentId, IBusinessObjectDefinitionProvider businessObjectDefinitionProvider)
		{
			Id = id;
			SimpleObjectTypeId = simpleObjectTypeId;
			ParentId = parentId;
			Attributes = new ReadOnlyDictionary<AttributeDefinition, object>(attributes);
			
			simpleObjectDefinition = businessObjectDefinitionProvider.FindSimpleObjectDefinition(SimpleObjectTypeId);
			if (simpleObjectDefinition == null)
				throw new Exception(string.Format("SimpleObject type {0} not found", SimpleObjectTypeId));
		}

		public SimpleObject(byte[] serializedObject, IBusinessObjectDefinitionProvider businessObjectDefinitionProvider)
		{
			var stream = new MemoryStream(serializedObject);

			var header = Serializer.DeserializeWithLengthPrefix<SimpleObjectHeader>(stream, PrefixStyle.Base128, 1);
			Id = header.Id;
			SimpleObjectTypeId = header.SimpleObjectTypeId;
			ParentId = header.ParentId;
			Revision = header.Revision;

			simpleObjectDefinition = businessObjectDefinitionProvider.FindSimpleObjectDefinition(SimpleObjectTypeId);
			if(simpleObjectDefinition == null)
				throw new Exception(string.Format("SimpleObject type {0} not found", SimpleObjectTypeId));

			var attributeValues = Serializer.DeserializeItems<AttributeValue>(stream, PrefixStyle.Base128, 2);
			foreach(var attribute in attributeValues)
			{
				if(simpleObjectDefinition.Attributes.ContainsKey(attribute.Name))
				{
					var attributeDefinition = simpleObjectDefinition.Attributes[attribute.Name];
					attributes.Add(attributeDefinition,
						attributeDefinition.Serializer.Deserialize(attributeDefinition.Name, attributeDefinition.AttributeType, attribute.Value));
				}
			}

			Attributes = new ReadOnlyDictionary<AttributeDefinition, object>(attributes);
		}

		public SimpleObject(Guid id, Guid simpleObjectTypeId, Guid? parentId, IDictionary<AttributeDefinition, object> attributes, IBusinessObjectDefinitionProvider businessObjectDefinitionProvider)
			: this(id, simpleObjectTypeId, parentId, businessObjectDefinitionProvider)
		{
			this.attributes = new Dictionary<AttributeDefinition, object>(attributes);
			Attributes = new ReadOnlyDictionary<AttributeDefinition, object>(attributes);
		}

		public T Attribute<T>(Expression<Func<T>> attribute)
		{
			var member = (MemberExpression)attribute.Body;
			AttributeAttribute a = null;//member.Member.GetCustomAttribute<AttributeAttribute>();
			var name = a != null ? a.Name : member.Member.Name;
			var attributeDefinition = simpleObjectDefinition.Attributes[name];
			if (!attributes.ContainsKey(attributeDefinition))
				return default(T);
			return (T)attributes[attributeDefinition];
		}

		public TValue Attribute<TBusinessObject, TValue>(Expression<Func<TBusinessObject, TValue>> attribute)
		{
			var member = (MemberExpression)attribute.Body;
			AttributeAttribute a = null;//member.Member.GetCustomAttribute<AttributeAttribute>();
			var name = a != null ? a.Name : member.Member.Name;
			var attributeDefinition = simpleObjectDefinition.Attributes[name];
			if (!attributes.ContainsKey(attributeDefinition))
				return default(TValue);
			return (TValue)attributes[attributeDefinition];
		}

		public Guid Id { get; private set; }
		public Guid SimpleObjectTypeId { get; private set; }
		public Guid? ParentId { get; private set; }
		public Guid Revision { get; private set; }

		public void ApplyChangeSet(CreateOrUpdateSimpleObjectChangeSet changeSet, SimpleObjectDefinition definition, Guid revision)
		{
			foreach(var modification in changeSet.AttributeValues)
			{
				if(!definition.Attributes.ContainsKey(modification.Name))
					continue;

				var attribute = definition.Attributes[modification.Name];
				attributes[attribute] = attribute.Serializer.Deserialize(attribute.Name, attribute.AttributeType, modification.Value);
			}

			Revision = revision;
		}

		public byte[] Serialize()
		{
			var stream = new MemoryStream();
			Serializer.SerializeWithLengthPrefix(stream, new SimpleObjectHeader(Id, SimpleObjectTypeId, ParentId, Revision), PrefixStyle.Base128, 1);
			foreach(var attribute in attributes)
			{
				var definition = attribute.Key;
				Serializer.SerializeWithLengthPrefix(stream,
					new AttributeValue(definition.Name, definition.Serializer.Serialize(definition.Name, definition.AttributeType, attribute.Value)),
					PrefixStyle.Base128, 2);
			}
			return stream.ToArray();
		}
	}
}