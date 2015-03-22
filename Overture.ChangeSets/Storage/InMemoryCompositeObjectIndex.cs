using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Overture.ChangeSets.Storage
{
	public class InMemoryCompositeObjectIndex : ICompositeObjectIndex
	{
		private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, Guid>> compositeObjectsByOwners =
			new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, Guid>>();
		
		public void Add(Guid ownerId, Guid compositeObjectId, Guid compositeObjectTypeId)
		{
			var compositeObjectChangeSets = compositeObjectsByOwners.GetOrAdd(ownerId, guid => new ConcurrentDictionary<Guid, Guid>());
			compositeObjectChangeSets.TryAdd(compositeObjectId, compositeObjectTypeId);
		}

		public IEnumerable<Guid> GetList(Guid compositeObjectTypeId, Guid ownerId)
		{
			ConcurrentDictionary<Guid, Guid> compositeObjectsByOwner;
			return compositeObjectsByOwners.TryGetValue(ownerId, out compositeObjectsByOwner)
				? compositeObjectsByOwner.Where(c => c.Value == compositeObjectTypeId).Select(c => c.Key)
				: Enumerable.Empty<Guid>();
		}

		public IEnumerable<Guid> GetList(Guid compositeObjectTypeId)
		{
			return
				compositeObjectsByOwners.SelectMany(o => o.Value.Where(c => c.Value == compositeObjectTypeId).Select(c => c.Key));
		}

		public Guid GetOwner(Guid compositeObjectId)
		{
			return compositeObjectsByOwners.Single(entry => entry.Value.ContainsKey(compositeObjectId)).Key;
		}
	}
}