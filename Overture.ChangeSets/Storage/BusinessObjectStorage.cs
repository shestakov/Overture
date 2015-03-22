using System;
using System.Collections.Generic;
using System.Reflection;
using Overture.ChangeSets.Attributes;
using Overture.ChangeSets.BusinessObjects;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.Protobuf.Composite;
using StackExchange.Profiling;

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
			var profiler = MiniProfiler.Current; // it's ok if this is null

			var cachedObject = compositeObjectCache.Find(compositeObjectId);

			if (cachedObject != null)
			{
				return cachedObject;
			}

			object lockHandle;

			using (profiler.Step("Obtaining lock"))
			{
				compositeObjectCache.FindAndLock(compositeObjectId, TimeSpan.FromSeconds(30), out lockHandle);
			}
			
			IEnumerable<CompositeObjectChangeSet> changeSets;

			using (profiler.Step("Getting changesets"))
			{
				changeSets = changeSetStorage.GetChangeSets(compositeObjectId);
			}
			
			CompositeObject compositeObject;

			using (profiler.Step("Applying changesets"))
			{
				compositeObject = new CompositeObject(changeSets, businessObjectDefinitionProvider);
			}

			using (profiler.Step("Putting to cache and unlocking"))
			{
				compositeObjectCache.PutAndUnlock(compositeObject, lockHandle);
			}

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