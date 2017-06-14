using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Overture.ChangeSets.Protobuf.Composite;

namespace Overture.ChangeSets.Storage
{
	public class InMemoryChangeSetStorage : IChangeSetStorage
	{
		private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, CompositeObjectChangeSet>> changeSetsByCompositeObjects =
			new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, CompositeObjectChangeSet>>();

		public void AppendChangeSet(CompositeObjectChangeSet changeSet)
		{
			var compositeObjectId = changeSet.CompositeObjectId;
			var compositeObjectChangeSets = changeSetsByCompositeObjects.GetOrAdd(compositeObjectId, guid => new ConcurrentDictionary<Guid, CompositeObjectChangeSet>());
			compositeObjectChangeSets.TryAdd(changeSet.ChangeSetId, changeSet);
		}

		public IEnumerable<CompositeObjectChangeSet> GetChangeSets(Guid compositeObjectId)
		{
			ConcurrentDictionary<Guid, CompositeObjectChangeSet> changeSets;
			return changeSetsByCompositeObjects.TryGetValue(compositeObjectId, out changeSets)
				? changeSets.Values.OrderBy(c => c.Timestamp)
				: Enumerable.Empty<CompositeObjectChangeSet>();
		}

		public IEnumerable<CompositeObjectChangeSet> GetChangeSets(Guid compositeObjectId, long sinceTimestamp)
		{
			ConcurrentDictionary<Guid, CompositeObjectChangeSet> changeSets;
			return changeSetsByCompositeObjects.TryGetValue(compositeObjectId, out changeSets)
				? changeSets.Values.Where(changeSet => changeSet.Timestamp > sinceTimestamp).OrderBy(c => c.Timestamp)
				: Enumerable.Empty<CompositeObjectChangeSet>();
		}

		public void UpdateChangeSet(CompositeObjectChangeSet changeSet, bool yesIKnowWhatIAmDoing = false)
		{
			if (!yesIKnowWhatIAmDoing)
				throw new Exception("You must know what are you doing!");

			var compositeObjectId = changeSet.CompositeObjectId;
			var compositeObjectChangeSets = changeSetsByCompositeObjects[compositeObjectId];
			compositeObjectChangeSets.TryUpdate(compositeObjectId, changeSet, changeSet);
		}
	}
}