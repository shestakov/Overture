using System;
using System.Collections.Generic;

namespace Overture.ChangeSets.Storage
{
	public interface ICompositeObjectIndex
	{
		void Add(Guid ownerId, Guid compositeObjectId, Guid compositeObjectTypeId);
		IEnumerable<Guid> GetList(Guid compositeObjectTypeId, Guid ownerId);
		IEnumerable<Guid> GetList(Guid compositeObjectTypeId);
		Guid GetOwner(Guid compositeObjectId);
	}
}