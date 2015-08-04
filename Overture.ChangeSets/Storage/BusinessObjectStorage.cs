using System;
using System.Collections.Generic;
using System.Reflection;
using Overture.ChangeSets.Attributes;
using Overture.ChangeSets.BusinessObjects;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.Protobuf.Composite;

namespace Overture.ChangeSets.Storage
{
	public class BusinessObjectStorage : IBusinessObjectStorage
	{
		private readonly IBusinessObjectDefinitionProvider businessObjectDefinitionProvider;
		private readonly IChangeSetStorage changeSetStorage;
		private readonly ICompositeObjectIndex compositeObjectIndex;
		private readonly ICompositeObjectCache compositeObjectCache;

		public BusinessObjectStorage(IChangeSetStorage changeSetStorage, ICompositeObjectIndex compositeObjectIndex,
			IBusinessObjectDefinitionProvider businessObjectDefinitionProvider, ICompositeObjectCache compositeObjectCache)
		{
			this.changeSetStorage = changeSetStorage;
			this.compositeObjectIndex = compositeObjectIndex;
			this.businessObjectDefinitionProvider = businessObjectDefinitionProvider;
			this.compositeObjectCache = compositeObjectCache;
		}

		public void PushNewComplexObjectChangeSet(Guid ownerId, CreateCompositeObjectChangeSet changeSet)
		{
			PushExistingComplexObjectChangeSet(ownerId, changeSet);
			compositeObjectIndex.Add(ownerId, changeSet.CompositeObjectId, changeSet.CompositeObjectTypeId);
		}
		
		public void PushExistingComplexObjectChangeSet(Guid ownerId, CompositeObjectChangeSet changeSet)
		{
			var compositeObjectId = changeSet.CompositeObjectId;
			object lockHandle;

			var cachedObject = compositeObjectCache.FindAndLock(compositeObjectId, TimeSpan.FromSeconds(30), out lockHandle);
			if (cachedObject != null)
			{
				cachedObject.ApplyChangeSet(changeSet, businessObjectDefinitionProvider);
			}

			changeSetStorage.AppendChangeSet(changeSet);

			if (cachedObject != null)
			{
				compositeObjectCache.PutAndUnlock(cachedObject, lockHandle);
			}
			else
			{
				compositeObjectCache.Unlock(compositeObjectId, lockHandle);
			}
		}

		public CompositeObject Get(Guid ownerId, Guid compositeObjectId)
		{
			var cachedObject = compositeObjectCache.Find(compositeObjectId);

			if (cachedObject != null)
			{
				return cachedObject;
			}

			object lockHandle;

			compositeObjectCache.FindAndLock(compositeObjectId, TimeSpan.FromSeconds(30), out lockHandle);

			var changeSets = changeSetStorage.GetChangeSets(compositeObjectId);

			var compositeObject = new CompositeObject(changeSets, businessObjectDefinitionProvider);
			
			compositeObjectCache.PutAndUnlock(compositeObject, lockHandle);

			return compositeObject;
		}

		public IEnumerable<Guid> GetList(Guid compositeObjectTypeId, Guid ownerId)
		{
			return compositeObjectIndex.GetList(compositeObjectTypeId, ownerId);
		}

		public IEnumerable<Guid> GetList(Guid compositeObjectTypeId)
		{
			return compositeObjectIndex.GetList(compositeObjectTypeId);
		}

		public IEnumerable<Guid> GetList<T>(Guid ownerId)
		{
			var compositeObjectAttribute = typeof(T).GetCustomAttribute<CompositeObjectAttribute>(true);
			if(compositeObjectAttribute == null)
				throw new ArgumentException("Specified type does not have a CompositeObjectAttribute");
			return GetList(compositeObjectAttribute.CompositeObjectTypeId, ownerId);
		}

		public Guid GetOwner(Guid compositeObjectId)
		{
			return compositeObjectIndex.GetOwner(compositeObjectId);
		}
	}
}