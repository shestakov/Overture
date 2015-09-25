using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Overture.ChangeSets.Attributes;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.Definitions;
using Overture.ChangeSets.Protobuf.AttributeValues;
using Overture.ChangeSets.Protobuf.Composite;
using Overture.ChangeSets.Protobuf.Headers;
using Overture.ChangeSets.Protobuf.Simple;
using ProtoBuf;

namespace Overture.ChangeSets.BusinessObjects
{
	public class CompositeObject
	{
		public readonly ReadOnlyDictionary<AttributeDefinition, object> Attributes;
		public readonly ReadOnlyDictionary<Guid, SimpleObject> SimpleObjects;
		private readonly HashSet<Guid> appliedChangeSets = new HashSet<Guid>();
		private readonly Dictionary<AttributeDefinition, object> attributes = new Dictionary<AttributeDefinition, object>();

		private readonly List<CompositeObjectChangeSetExceptionRecord> complexObjectChangeSetExceptions =
			new List<CompositeObjectChangeSetExceptionRecord>(0);

		private readonly List<SimpleObjectChangeSetExceptionRecord> simpleObjectChangeSetExceptions = new List<SimpleObjectChangeSetExceptionRecord>(0);
		private readonly Dictionary<Guid, SimpleObject> simpleObjects = new Dictionary<Guid, SimpleObject>();
		private CompositeObjectDefinition compositeObjectDefinition;

		public CompositeObject(Guid id, Guid compositeObjectTypeId, DateTimeOffset lastModified, Guid revision,
			IBusinessObjectDefinitionProvider definitionProvider, IDictionary<AttributeDefinition, object> attributeValues, IEnumerable<Guid> appliedChangeSets,
			IEnumerable<SimpleObject> simpleObjects)
		{
			Id = id;
			CompositeObjectTypeId = compositeObjectTypeId;
			LastModified = lastModified;
			Revision = revision;
			compositeObjectDefinition = definitionProvider.GetCompositeObjectDefinition(compositeObjectTypeId);

			attributes = new Dictionary<AttributeDefinition, object>(attributeValues);

			foreach(var changeSetId in appliedChangeSets)
			{
				this.appliedChangeSets.Add(changeSetId);
			}

			foreach(var simpleObject in simpleObjects)
			{
				this.simpleObjects.Add(simpleObject.Id, simpleObject);
			}

			SimpleObjects = new ReadOnlyDictionary<Guid, SimpleObject>(this.simpleObjects);
			
			Attributes = new ReadOnlyDictionary<AttributeDefinition, object>(attributes);
		}

		public CompositeObject(IEnumerable<CompositeObjectChangeSet> changeSets, IBusinessObjectDefinitionProvider definitionProvider)
		{
			foreach(var changeSet in changeSets)
			{
				if(compositeObjectDefinition == null)
					Initialize(definitionProvider, (CreateCompositeObjectChangeSet) changeSet);

				try
				{
					ApplyChangeSet(changeSet, definitionProvider);
				}
				catch(Exception ex)
				{
					complexObjectChangeSetExceptions.Add(new CompositeObjectChangeSetExceptionRecord(changeSet, ex));
				}
			}

			if(compositeObjectDefinition == null)
				throw new Exception("CompositeObject has not been initialized");

			Attributes = new ReadOnlyDictionary<AttributeDefinition, object>(attributes);
			SimpleObjects = new ReadOnlyDictionary<Guid, SimpleObject>(simpleObjects);
		}

		public CompositeObject(byte[] serializedObject, IBusinessObjectDefinitionProvider businessObjectDefinitionProvider)
		{
			var stream = new MemoryStream(serializedObject);

			var header = Serializer.DeserializeWithLengthPrefix<CompositeObjectHeader>(stream, PrefixStyle.Base128, 1);
			Id = header.Id;
			CompositeObjectTypeId = header.CompositeObjectTypeId;
			LastModified = new DateTimeOffset(header.LastModified, new TimeSpan(0));
			Revision = header.Revision;

			compositeObjectDefinition = businessObjectDefinitionProvider.GetCompositeObjectDefinition(CompositeObjectTypeId);

			for(var i = 0; i < header.AttributeCount; i++)
			{
				var attribute = Serializer.DeserializeWithLengthPrefix<AttributeValue>(stream, PrefixStyle.Base128, 2);
				if (compositeObjectDefinition.Attributes.ContainsKey(attribute.Name))
				{
					var attributeDefinition = compositeObjectDefinition.Attributes[attribute.Name];
					attributes.Add(attributeDefinition,
						attributeDefinition.Serializer.Deserialize(attributeDefinition.Name, attributeDefinition.AttributeType, attribute.Value));
				}
			}

			for(var i = 0; i < header.SimpleObjectCount; i++)
			{
				var serializedSimpleObject = Serializer.DeserializeWithLengthPrefix<byte[]>(stream, PrefixStyle.Base128, 3);
				var simpleObject = new SimpleObject(serializedSimpleObject, businessObjectDefinitionProvider);
				simpleObjects.Add(simpleObject.Id, simpleObject);
			}

			Attributes = new ReadOnlyDictionary<AttributeDefinition, object>(attributes);
			SimpleObjects = new ReadOnlyDictionary<Guid, SimpleObject>(simpleObjects);
		}

		public Guid Id { get; private set; }
		public Guid CompositeObjectTypeId { get; private set; }
		public DateTimeOffset LastModified { get; private set; }
		public Guid Revision { get; private set; }

		public Dictionary<Guid, SimpleObject> GetSimpleObjectsOfType<T>()
		{
			var typeId = typeof(T).GetCustomAttribute<SimpleObjectAttribute>().SimpleObjectTypeId;
			return GetSimpleObjectsOfType(typeId);
		}

		public IEnumerable<SimpleObject> GetObjectsOfType<T>()
		{
			var typeId = typeof(T).GetCustomAttribute<SimpleObjectAttribute>().SimpleObjectTypeId;
			return GetObjectsOfType(typeId);
		}

		public IEnumerable<SimpleObject> GetObjectsOfType(Guid simpleObjectTypeId)
		{
			return SimpleObjects.Values.Where(o => o.SimpleObjectTypeId == simpleObjectTypeId);
		}
		
		public Dictionary<Guid, SimpleObject> GetSimpleObjectsOfType(Guid simpleObjectTypeId)
		{
			return SimpleObjects.Where(pair => pair.Value.SimpleObjectTypeId == simpleObjectTypeId)
				.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		public Dictionary<Guid, SimpleObject> GetSimpleObjectsOfType<T>(Guid? parentId)
		{
			var typeId = typeof(T).GetCustomAttribute<SimpleObjectAttribute>().SimpleObjectTypeId;

			return GetSimpleObjectsOfType(typeId, parentId);
		}

		public Dictionary<Guid, SimpleObject> GetSimpleObjectsOfType(Guid simpleObjectTypeId, Guid? parentId)
		{
			return SimpleObjects.Where(pair => pair.Value.SimpleObjectTypeId == simpleObjectTypeId && pair.Value.ParentId == parentId)
				.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		private void Initialize(IBusinessObjectDefinitionProvider definitionProvider, CreateCompositeObjectChangeSet changeSetCreate)
		{
			Id = changeSetCreate.CompositeObjectId;
			CompositeObjectTypeId = changeSetCreate.CompositeObjectTypeId;
			compositeObjectDefinition = definitionProvider.GetCompositeObjectDefinition(CompositeObjectTypeId);
		}

		/// <summary>
		/// Applying a changeset. This method is NOT thread safe.
		/// </summary>
		/// <param name="changeSet"></param>
		/// <param name="definitionProvider"></param>
		public void ApplyChangeSet(CompositeObjectChangeSet changeSet, IBusinessObjectDefinitionProvider definitionProvider)
		{
			if(appliedChangeSets.Contains(changeSet.ChangeSetId))
				return;

			foreach(var modification in changeSet.AttributeValues)
			{
				if(!compositeObjectDefinition.Attributes.ContainsKey(modification.Name))
					continue;

				var attribute = compositeObjectDefinition.Attributes[modification.Name];
				attributes[attribute] = attribute.Serializer.Deserialize(attribute.Name, attribute.AttributeType, modification.Value);
			}

			var lastModified = new DateTimeOffset(changeSet.Timestamp, new TimeSpan(0));

			foreach(var simpleObjectChangeSet in changeSet.ChildObjectChangeSets)
			{
				if(simpleObjectChangeSet.Action == SimpleObjectChangeSetType.Delete && simpleObjects.ContainsKey(simpleObjectChangeSet.SimpleObjectId))
				{
					simpleObjects.Remove(simpleObjectChangeSet.SimpleObjectId);
				}
				else if (simpleObjectChangeSet.Action == SimpleObjectChangeSetType.UpdateParent &&
						 simpleObjects.ContainsKey(simpleObjectChangeSet.SimpleObjectId))
				{
					var simpleObject = simpleObjects[simpleObjectChangeSet.SimpleObjectId];
					var updateSimpleObjectParentChangeSet = (UpdateSimpleObjectParentChangeSet)simpleObjectChangeSet;
					simpleObject.UpdateParentId(updateSimpleObjectParentChangeSet.ParentId);
				}
				else
				{
					SimpleObject simpleObject;
					var createOrUpdateSimpleObjectCommand = simpleObjectChangeSet as CreateOrUpdateSimpleObjectChangeSet;

					SimpleObjectDefinition simpleObjectDefinition;

					if(simpleObjectChangeSet.Action == SimpleObjectChangeSetType.Create)
					{
						var commandCreate = (CreateSimpleObjectChangeSet) simpleObjectChangeSet;
						
						simpleObjectDefinition = definitionProvider.FindSimpleObjectDefinition(commandCreate.SimpleObjectType);
						if (simpleObjectDefinition == null)
							continue;

						simpleObject = new SimpleObject(commandCreate.SimpleObjectId, commandCreate.ParentId, simpleObjectDefinition);
					}
					else if (simpleObjects.ContainsKey(simpleObjectChangeSet.SimpleObjectId))
					{
						simpleObject = simpleObjects[simpleObjectChangeSet.SimpleObjectId];
						simpleObjectDefinition = simpleObject.SimpleObjectDefinition;
					}
					else
					{
						continue;
					}

					try
					{
						simpleObject.ApplyChangeSet(createOrUpdateSimpleObjectCommand, simpleObjectDefinition, changeSet.ChangeSetId, lastModified);
					}
					catch(Exception ex)
					{
						simpleObjectChangeSetExceptions.Add(new SimpleObjectChangeSetExceptionRecord(simpleObjectChangeSet, ex));
					}

					if (!simpleObjects.ContainsKey(simpleObjectChangeSet.SimpleObjectId))
						simpleObjects.Add(simpleObjectChangeSet.SimpleObjectId, simpleObject);
				}
			}

			appliedChangeSets.Add(changeSet.ChangeSetId);
			LastModified = lastModified;
			Revision = changeSet.ChangeSetId;
		}

		public T Attribute<T>(string name)
		{
			var attributeDefinition = compositeObjectDefinition.Attributes[name];
			if (!attributes.ContainsKey(attributeDefinition))
				return default(T);
			return (T)attributes[attributeDefinition];
		}

		public T Attribute<T>(Expression<Func<T>> attribute)
		{
			var member = (MemberExpression)attribute.Body;
			AttributeAttribute a = null;//member.Member.GetCustomAttribute<AttributeAttribute>();
			var name = a != null ? a.Name : member.Member.Name;
			var attributeDefinition = compositeObjectDefinition.Attributes[name];
			if (!attributes.ContainsKey(attributeDefinition))
				return default(T);
			return (T)attributes[attributeDefinition];
		}
		
		public TValue Attribute<TBusinessObject, TValue>(Expression<Func<TBusinessObject, TValue>> attribute)
		{
			var member = (MemberExpression)attribute.Body;
			AttributeAttribute a = null;//member.Member.GetCustomAttribute<AttributeAttribute>();
			var name = a != null ? a.Name : member.Member.Name;
			var attributeDefinition = compositeObjectDefinition.Attributes[name];
			if (!attributes.ContainsKey(attributeDefinition))
				return default(TValue);
			return (TValue)attributes[attributeDefinition];
		}

		public byte[] Serialize()
		{
			Serializer.PrepareSerializer<AttributeValue>();
			var stream = new MemoryStream();
			Serializer.SerializeWithLengthPrefix(stream,
				new CompositeObjectHeader(Id, CompositeObjectTypeId, LastModified.Ticks, Revision, appliedChangeSets.ToArray(), attributes.Count,
					simpleObjects.Count), PrefixStyle.Base128, 1);

			foreach(var attribute in attributes)
			{
				var definition = attribute.Key;
				Serializer.SerializeWithLengthPrefix(stream,
					new AttributeValue(definition.Name, definition.Serializer.Serialize(definition.Name, definition.AttributeType, attribute.Value)),
					PrefixStyle.Base128, 2);
			}

			foreach(var simpleObject in simpleObjects.Values)
			{
				Serializer.SerializeWithLengthPrefix(stream, simpleObject.Serialize(), PrefixStyle.Base128, 3);
			}

			return stream.ToArray();
		}
	}
}