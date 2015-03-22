using System;
using System.Collections.Generic;
using Overture.ChangeSets.BusinessObjects;
using Overture.ChangeSets.Protobuf.Composite;

namespace Overture.ChangeSets.Storage
{
	public interface IBusinessObjectStorage
	{
		void PushNewComplexObjectChangeSet(Guid ownerId, CreateCompositeObjectChangeSet changeSet);
		void PushExistingComplexObjectChangeSet(Guid ownerId, CompositeObjectChangeSet changeSet);
		CompositeObject Get(Guid ownerId, Guid compositeObjectId);
		IEnumerable<Guid> GetList(Guid compositeObjectTypeId, Guid ownerId);
		IEnumerable<Guid> GetList(Guid compositeObjectTypeId);
		IEnumerable<Guid> GetList<T>(Guid ownerId);
		Guid GetOwner(Guid compositeObjectId);
	}
}