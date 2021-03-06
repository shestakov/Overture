﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using Overture.ChangeSets.Protobuf.Composite;
using Overture.ChangeSets.Sql.Entities;
using Overture.ChangeSets.Storage;
using Overture.Core;
using ProtoBuf;

namespace Overture.ChangeSets.Sql
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class SqlChangeSetStorage : IChangeSetStorage
	{
		private readonly IDataContextProvider dataContextProvider;

		public SqlChangeSetStorage(IDataContextProvider dataContextProvider)
		{
			this.dataContextProvider = dataContextProvider;
			Serializer.PrepareSerializer<CompositeObjectChangeSet>();
		}

		public void AppendChangeSet(CompositeObjectChangeSet changeSet)
		{
			var entity = ToEntity(changeSet);
			using (var context = GetDataContext())
			{
				context.GetTable<CompositeObjectChangeSetEntity>().InsertOnSubmit(entity);
				context.SubmitChanges();
			}
		}

		public IEnumerable<CompositeObjectChangeSet> GetChangeSets(Guid compositeObjectId)
		{
			return GetChangeSets(compositeObjectId, 0);
		}

		public long GetMaxTimestamp(Guid compositeObjectId)
		{
			using (var context = GetDataContext())
			{
				//return context.GetTable<CompositeObjectChangeSetEntity>()
				//	.Where(e => e.CompositeObjectId == compositeObjectId)
				//	.Select(e => (long?)e.Timestamp)
				//	.Max() ?? 0;

				return context
						   .GetTable<CompositeObjectChangeSetEntity>()
						   .Where(e => e.CompositeObjectId == compositeObjectId)
						   .OrderByDescending(e => e.Timestamp)
						   .FirstOrDefault()?.Timestamp ?? 0;
			}
		}

		public IEnumerable<CompositeObjectChangeSet> GetChangeSets(Guid compositeObjectId, long sinceTimestamp)
		{
			using (var context = GetDataContext())
			{
				return context.GetTable<CompositeObjectChangeSetEntity>()
					.Where(e => e.CompositeObjectId == compositeObjectId && e.Timestamp > sinceTimestamp)
					.OrderBy(e => e.Timestamp)
					.Select(FromEntity)
					.ToArray();
			}
		}

		public void UpdateChangeSet(CompositeObjectChangeSet changeSet, bool yesIKnowWhatIAmDoing = false)
		{
			if(!yesIKnowWhatIAmDoing)
				throw new Exception("You must know what are you doing!");

			var entity = ToEntity(changeSet);
			using (var context = GetDataContext())
			{
				context.GetTable<CompositeObjectChangeSetEntity>().Attach(entity);
				context.Refresh(RefreshMode.KeepCurrentValues, entity);
				context.SubmitChanges();
			}
		}

		private static CompositeObjectChangeSetEntity ToEntity(CompositeObjectChangeSet changeSet)
		{
			var stream = new MemoryStream();
			Serializer.Serialize(stream, changeSet);
			var content = stream.ToArray();

			return new CompositeObjectChangeSetEntity
			{
				CompositeObjectId = changeSet.CompositeObjectId,
				ChangeSetId = changeSet.ChangeSetId,
				UserId = changeSet.UserId,
				Timestamp = changeSet.Timestamp,
				ContentLength = content.Length,
				Content = content
			};
		}

		private static CompositeObjectChangeSet FromEntity(CompositeObjectChangeSetEntity compositeObjectChangeSetEntity)
		{
			return Serializer.Deserialize<CompositeObjectChangeSet>(new MemoryStream(compositeObjectChangeSetEntity.Content));
		}

		private DataContext GetDataContext()
		{
			return dataContextProvider.Get();
		}
	}
}