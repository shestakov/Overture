using System;
using Overture.ChangeSets.BusinessObjects;

namespace Overture.ChangeSets.Storage
{
	public interface ICompositeObjectCache
	{
		CompositeObject Find(Guid id);
		void Drop(Guid id);
		CompositeObject FindAndLock(Guid id, TimeSpan lockTimeout, out object lockHandle);
		void PutAndUnlock(CompositeObject compositeObject, object lockHandle);
		void DropAndUnlock(Guid id, object lockHandle);
		void Unlock(Guid id, object lockHandle);
	}
}