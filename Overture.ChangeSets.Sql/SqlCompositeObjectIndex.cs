using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Overture.ChangeSets.Protobuf.Composite;
using Overture.ChangeSets.Sql.Entities;
using Overture.ChangeSets.Storage;
using Overture.Core;
using ProtoBuf;

namespace Overture.ChangeSets.Sql
{
	public class SqlCompositeObjectIndex : ICompositeObjectIndex
	{
		private readonly IDataContextProvider dataContextProvider;

		public SqlCompositeObjectIndex(IDataContextProvider dataContextProvider)
		{
			this.dataContextProvider = dataContextProvider;
			Serializer.PrepareSerializer<CompositeObjectChangeSet>();
		}

		public void Add(Guid ownerId, Guid compositeObjectId, Guid compositeObjectTypeId)
		{
			var entity = new CompositeObjectIndexRecordEntity
			{
				OwnerId = ownerId,
				CompositeObjectId = compositeObjectId,
				CompositeObjectTypeId = compositeObjectTypeId,
				DateTimeCreated = DateTimeOffset.UtcNow
			};

			using (var context = GetDataContext())
			{
				context.GetTable<CompositeObjectIndexRecordEntity>().InsertOnSubmit(entity);
				context.SubmitChanges();
			}
		}

		public IEnumerable<Guid> GetList(Guid compositeObjectTypeId, Guid ownerId)
		{
			using (var context = GetDataContext())
			{
				return context.GetTable<CompositeObjectIndexRecordEntity>()
					.Where(e => e.OwnerId == ownerId && e.CompositeObjectTypeId == compositeObjectTypeId && e.DateTimeDeleted == null)
					.Select(e => e.CompositeObjectId)
					.ToArray();
			}
		}

		public IEnumerable<Guid> GetList(Guid compositeObjectTypeId)
		{
			using (var context = GetDataContext())
			{
				return context.GetTable<CompositeObjectIndexRecordEntity>()
					.Where(e => e.CompositeObjectTypeId == compositeObjectTypeId && e.DateTimeDeleted == null)
					.Select(e => e.CompositeObjectId)
					.ToArray();
			}
		}

		public Guid GetOwner(Guid compositeObjectId)
		{
			using (var context = GetDataContext())
			{
				return context.GetTable<CompositeObjectIndexRecordEntity>()
					.Single(e => e.CompositeObjectId == compositeObjectId)
					.OwnerId;
			}
		}

		private DataContext GetDataContext()
		{
			return dataContextProvider.Get();
		}
	}
}