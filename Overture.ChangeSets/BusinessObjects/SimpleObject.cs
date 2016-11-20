using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using Overture.ChangeSets.Attributes;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.Definitions;
using Overture.ChangeSets.Properties;
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
		public SimpleObjectDefinition SimpleObjectDefinition { get; }

		public SimpleObject(Guid id, Guid? parentId, [NotNull] SimpleObjectDefinition simpleObjectDefinition, Guid createdByUserId, DateTimeOffset dateTimeCreated, Guid revision)
		{
			if (simpleObjectDefinition == null)
				throw new ArgumentNullException(nameof(simpleObjectDefinition));

			Id = id;
			SimpleObjectDefinition = simpleObjectDefinition;
			ParentId = parentId;
			Attributes = new ReadOnlyDictionary<AttributeDefinition, object>(attributes);
			CreatedByUserId = createdByUserId;
			DateTimeCreated = dateTimeCreated;
			LastUpdatedByUserId = createdByUserId;
			DateTimeLastUpdated = dateTimeCreated;
			Revision = revision;
		}

		public SimpleObject(byte[] serializedObject, IBusinessObjectDefinitionProvider businessObjectDefinitionProvider)
		{
			var stream = new MemoryStream(serializedObject);

			var header = Serializer.DeserializeWithLengthPrefix<SimpleObjectHeader>(stream, PrefixStyle.Base128, 1);
			Id = header.Id;
			ParentId = header.ParentId;
			Revision = header.Revision;
			var simpleObjectTypeId = header.SimpleObjectTypeId;

			CreatedByUserId = header.CreatedByUserId;
			DateTimeCreated = new DateTimeOffset(header.DateTimeCreated, new TimeSpan(0));
			LastUpdatedByUserId = header.LastUpdatedByUserId;
			DateTimeLastUpdated = new DateTimeOffset(header.DateTimeLastUpdated, new TimeSpan(0));

			SimpleObjectDefinition = businessObjectDefinitionProvider.FindSimpleObjectDefinition(simpleObjectTypeId);
			if(SimpleObjectDefinition == null)
				throw new Exception($"SimpleObject type {SimpleObjectTypeId} not found");

			var attributeValues = Serializer.DeserializeItems<AttributeValue>(stream, PrefixStyle.Base128, 2);
			foreach(var attribute in attributeValues)
			{
				if(SimpleObjectDefinition.Attributes.ContainsKey(attribute.Name))
				{
					var attributeDefinition = SimpleObjectDefinition.Attributes[attribute.Name];
					attributes.Add(attributeDefinition,
						attributeDefinition.Serializer.Deserialize(attributeDefinition.Name, attributeDefinition.AttributeType, attribute.Value));
				}
			}

			Attributes = new ReadOnlyDictionary<AttributeDefinition, object>(attributes);
		}

		public SimpleObject(Guid id, Guid? parentId, IDictionary<AttributeDefinition, object> attributes, [NotNull] SimpleObjectDefinition simpleObjectDefinition, Guid createdByUserId, DateTimeOffset dateTimeCreated, Guid revision)
			: this(id, parentId, simpleObjectDefinition, createdByUserId, dateTimeCreated, revision)
		{
			this.attributes = new Dictionary<AttributeDefinition, object>(attributes);
			Attributes = new ReadOnlyDictionary<AttributeDefinition, object>(attributes);
		}

		public T Attribute<T>(string name)
		{
			var attributeDefinition = SimpleObjectDefinition.Attributes[name];
			if (!attributes.ContainsKey(attributeDefinition))
				return default(T);
			return (T)attributes[attributeDefinition];
		}

		public T Attribute<T>(Expression<Func<T>> attribute)
		{
			var member = (MemberExpression)attribute.Body;
			AttributeAttribute a = null;//member.Member.GetCustomAttribute<AttributeAttribute>();
			var name = a != null ? a.Name : member.Member.Name;
			var attributeDefinition = SimpleObjectDefinition.Attributes[name];
			if (!attributes.ContainsKey(attributeDefinition))
				return default(T);
			return (T)attributes[attributeDefinition];
		}

		public TValue Attribute<TBusinessObject, TValue>(Expression<Func<TBusinessObject, TValue>> attribute)
		{
			var member = (MemberExpression)attribute.Body;
			AttributeAttribute a = null;//member.Member.GetCustomAttribute<AttributeAttribute>();
			var name = a != null ? a.Name : member.Member.Name;
			var attributeDefinition = SimpleObjectDefinition.Attributes[name];
			if (!attributes.ContainsKey(attributeDefinition))
				return default(TValue);
			return (TValue)attributes[attributeDefinition];
		}

		public Guid Id { get; private set; }
		public Guid? ParentId { get; private set; }
		public Guid Revision { get; private set; }
		public DateTimeOffset DateTimeCreated { get; }
		public DateTimeOffset DateTimeLastUpdated { get; private set; }
		public Guid CreatedByUserId { get; }
		public Guid LastUpdatedByUserId { get; private set; }

		public Guid SimpleObjectTypeId
		{
			get { return SimpleObjectDefinition.SimpleObjectTypeId; }
		}

		public void UpdateParentId(Guid? parentId)
		{
			ParentId = parentId;
		}

		public void ApplyChangeSet(CreateOrUpdateSimpleObjectChangeSet changeSet, SimpleObjectDefinition definition, Guid revision, DateTimeOffset lastModified, Guid updatedByUserId)
		{
			foreach(var modification in changeSet.AttributeValues)
			{
				if(!definition.Attributes.ContainsKey(modification.Name))
					continue;

				var attribute = definition.Attributes[modification.Name];
				attributes[attribute] = attribute.Serializer.Deserialize(attribute.Name, attribute.AttributeType, modification.Value);
			}

			Revision = revision;
			DateTimeLastUpdated = lastModified;
			LastUpdatedByUserId = updatedByUserId;
		}

		public byte[] Serialize()
		{
			var stream = new MemoryStream();
			Serializer.SerializeWithLengthPrefix(stream, new SimpleObjectHeader(Id, SimpleObjectTypeId, ParentId, Revision, DateTimeCreated.Ticks, CreatedByUserId, DateTimeLastUpdated.Ticks, LastUpdatedByUserId), PrefixStyle.Base128, 1);
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