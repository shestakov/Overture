using System;
using System.Collections.Generic;
using Overture.ChangeSets.Protobuf.Composite;

namespace Overture.ChangeSets.Storage
{
	public interface IChangeSetStorage
	{
		void AppendChangeSet(CompositeObjectChangeSet changeSet);
		IEnumerable<CompositeObjectChangeSet> GetChangeSets(Guid compositeObjectId);
		long GetMaxTimestamp(Guid compositeObjectId);
		IEnumerable<CompositeObjectChangeSet> GetChangeSets(Guid compositeObjectId, long sinceTimestamp);
		void UpdateChangeSet(CompositeObjectChangeSet changeSet, bool yesIKnowWhatIAmDoing = false);
	}
}