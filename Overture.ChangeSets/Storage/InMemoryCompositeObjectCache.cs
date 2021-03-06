using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Overture.ChangeSets.BusinessObjects;
using Overture.ChangeSets.DefinitionProvider;

namespace Overture.ChangeSets.Storage
{
	public class InMemoryCompositeObjectCache : ICompositeObjectCache
	{
		private const int repreatCount = 100;
		private static readonly TimeSpan repeatDelay = TimeSpan.FromMilliseconds(300);
		private readonly Dictionary<Guid, CacheObject> globalStorage = new Dictionary<Guid, CacheObject>();
		private readonly Dictionary<Guid, CompositeObject> localStorage = new Dictionary<Guid, CompositeObject>();
		private readonly IBusinessObjectDefinitionProvider businessObjectDefinitionProvider;
		private TimeSpan lockTimespan = TimeSpan.FromSeconds(90);

		public InMemoryCompositeObjectCache(IBusinessObjectDefinitionProvider businessObjectDefinitionProvider)
		{
			this.businessObjectDefinitionProvider = businessObjectDefinitionProvider;
		}

		public CompositeObject Find(Guid id)
		{
			CompositeObject cachedObject = null;

			Trace.WriteLine("InMemoryCompositeObjectCache.Find: locking localStorage");

			lock (localStorage)
			{
				if (localStorage.ContainsKey(id))
					cachedObject = localStorage[id];
			}

			Trace.WriteLine("InMemoryCompositeObjectCache.Find: localStorage lock released");

			if(cachedObject != null)
				return cachedObject;

			object lockHandle;
			cachedObject = FindAndLock(id, lockTimespan, out lockHandle);
			Unlock(id, lockHandle);
			return cachedObject;
		}

		public void Drop(Guid id)
		{
			object lockHandle;
			var cachedObject = FindAndLock(id, lockTimespan, out lockHandle);
			if (cachedObject != null)
			{
				DropAndUnlock(id, lockHandle);
			}
			else
			{
				Unlock(id, lockHandle);
			}
		}

		public CompositeObject FindAndLock(Guid id, TimeSpan lockTimeout, out object lockHandle)
		{
			for (int i = 0; i < repreatCount; i++)
			{
				try
				{
					return FindAndLockInternal(id, lockTimeout, out lockHandle);
				}
				catch (CacheObjectLockException)
				{
					//Some logging goes here
					Thread.Sleep(repeatDelay);
				}
			}

			throw new MaxLockAttemptCountException(id);
		}

		private CompositeObject FindAndLockInternal(Guid id, TimeSpan lockTimeout, out object lockHandle)
		{
			Trace.WriteLine("InMemoryCompositeObjectCache.FindAndLockInternal: locking globalStorage");

			CompositeObject compositeObject;

			lock (globalStorage)
			{
				var utcNow = DateTimeOffset.UtcNow;
				var expirationDateTime = utcNow + lockTimeout;
				var handle = Guid.NewGuid();
				lockHandle = handle;

				if (!globalStorage.ContainsKey(id))
				{
					globalStorage.Add(id, new CacheObject(null, handle, expirationDateTime));
					return null;
				}

				var cacheObject = globalStorage[id];
				if (cacheObject.LockHandle != null && cacheObject.ExpirationDateTime > utcNow)
					throw new CacheObjectLockException(id);

				globalStorage[id] = new CacheObject(cacheObject.Bytes, handle, expirationDateTime);

				compositeObject = cacheObject.Bytes != null
					? new CompositeObject(cacheObject.Bytes, businessObjectDefinitionProvider)
					: null;

				Trace.WriteLine("InMemoryCompositeObjectCache.FindAndLockInternal: locking localStorage");

				lock (localStorage)
				{
					localStorage[id] = compositeObject;
				}

				Trace.WriteLine("InMemoryCompositeObjectCache.FindAndLockInternal: localStorage lock released");
			}

			Trace.WriteLine("InMemoryCompositeObjectCache.FindAndLockInternal: globalStorage lock released");

			return compositeObject;
		}

		public void PutAndUnlock(CompositeObject compositeObject, object lockHandle)
		{
			var utcNow = DateTimeOffset.UtcNow;

			Trace.WriteLine("InMemoryCompositeObjectCache.PutAndUnlock: locking globalStorage");

			lock (globalStorage){

				if (!globalStorage.ContainsKey(compositeObject.Id))
					throw new ArgumentException("There is no such CompositeObject in cache");

				var cacheObject = globalStorage[compositeObject.Id];

				if (cacheObject.LockHandle != (Guid) lockHandle)
					throw new ArgumentException("Incorrect lock");

				if (cacheObject.ExpirationDateTime < utcNow)
					throw new ArgumentException("Lock expired");

				var bytes = compositeObject.Serialize();
				globalStorage[compositeObject.Id] = new CacheObject(bytes, null, null);

				Trace.WriteLine("InMemoryCompositeObjectCache.PutAndUnlock: locking localStorage");

				lock (localStorage)
				{
					localStorage[compositeObject.Id] = compositeObject;
				}

				Trace.WriteLine("InMemoryCompositeObjectCache.PutAndUnlock: localStorage lock released");
			}

			Trace.WriteLine("InMemoryCompositeObjectCache.PutAndUnlock: globalStorage lock released");
		}

		public void DropAndUnlock(Guid id, object lockHandle)
		{
			var utcNow = DateTimeOffset.UtcNow;
			Trace.WriteLine("InMemoryCompositeObjectCache.DropAndUnlock: locking globalStorage");
			lock (globalStorage)
			{
				if (!globalStorage.ContainsKey(id))
					return;

				var cacheObject = globalStorage[id];

				if (cacheObject.LockHandle != (Guid) lockHandle)
					throw new ArgumentException("Incorrect lock");

				if (cacheObject.ExpirationDateTime < utcNow)
					throw new ArgumentException("Lock expired");

				globalStorage.Remove(id);

				Trace.WriteLine("InMemoryCompositeObjectCache.DropAndUnlock: locking localStorage"); 
				lock (localStorage)
				{
					localStorage.Remove(id);
				}
				Trace.WriteLine("InMemoryCompositeObjectCache.DropAndUnlock: localStorage lock released");
			}
			Trace.WriteLine("InMemoryCompositeObjectCache.DropAndUnlock: globalStorage lock released");
		}


		public void Unlock(Guid id, object lockHandle)
		{
			var utcNow = DateTimeOffset.UtcNow;
			Trace.WriteLine("InMemoryCompositeObjectCache.Unlock: locking globalStorage");
			lock (globalStorage)
			{
				if (!globalStorage.ContainsKey(id))
					return;

				var cacheObject = globalStorage[id];

				if (cacheObject.LockHandle != (Guid)lockHandle)
					throw new ArgumentException("Incorrect lock");

				if (cacheObject.ExpirationDateTime < utcNow)
					throw new ArgumentException("Lock expired");

				globalStorage[id] = new CacheObject(cacheObject.Bytes, null, null);
			}
			Trace.WriteLine("InMemoryCompositeObjectCache.Unlock: globalStorage lock released");
		}

		private class CacheObject
		{
			public CacheObject(byte[] bytes, Guid? lockHandle, DateTimeOffset? expirationDateTime)
			{
				Bytes = bytes;
				LockHandle = lockHandle;
				ExpirationDateTime = expirationDateTime;
			}

			public byte[] Bytes { get; private set; }
			public Guid? LockHandle { get; private set; }
			public DateTimeOffset? ExpirationDateTime { get; private set; }
		}
	}

	public class CacheObjectLockException : Exception
	{
		public CacheObjectLockException(Guid id)
			: base(string.Format("Cache object {0} locked", id))
		{
		}
	}

	public class MaxLockAttemptCountException : Exception
	{
		public MaxLockAttemptCountException(Guid id)
			: base(string.Format("Maximum lock attempt count exceeded for Composite Object {0}", id))
		{
		}
	}
}