using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Overture.ChangeSets.Protobuf.Composite;
using Overture.ChangeSets.Storage;
using ProtoBuf;

namespace Overture.ChangeSets.SqlDb
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class SqlChangeSetStorage : IChangeSetStorage
	{
		private readonly IDbConnectionFactory dbConnectionFactory;

		public SqlChangeSetStorage(IDbConnectionFactory dbConnectionFactory)
		{
			this.dbConnectionFactory = dbConnectionFactory;
			Serializer.PrepareSerializer<CompositeObjectChangeSet>();
		}

		public void AppendChangeSet(CompositeObjectChangeSet changeSet)
		{
			var stream = new MemoryStream();
			Serializer.Serialize(stream, changeSet);
			var content = stream.ToArray();

			using (var connection = dbConnectionFactory.Get())
			{
				connection.Open();

				using (var command = connection.CreateCommand())
				{
					command.CommandText =
						"INSERT INTO CompositeObjectChangeSets (CompositeObjectId, ChangeSetId, UserId, Timestamp, ContentLength, Content) VALUES (@CompositeObjectId, @ChangeSetId, @UserId, @Timestamp, @ContentLength, @Content)";
				
					var p1 = command.CreateParameter();
					p1.ParameterName = "@CompositeObjectId";
					p1.DbType = DbType.Guid;
					p1.Value = changeSet.CompositeObjectId;
					command.Parameters.Add(p1);

					var p2 = command.CreateParameter();
					p2.ParameterName = "@ChangeSetId";
					p2.DbType = DbType.Guid;
					p2.Value = changeSet.ChangeSetId;
					command.Parameters.Add(p2);

					var p3 = command.CreateParameter();
					p3.ParameterName = "@UserId";
					p3.DbType = DbType.Guid;
					p3.Value = changeSet.UserId;
					command.Parameters.Add(p3);

					var p4 = command.CreateParameter();
					p4.ParameterName = "@Timestamp";
					p4.DbType = DbType.Int64;
					p4.Value = changeSet.Timestamp;
					command.Parameters.Add(p4);

					var p5 = command.CreateParameter();
					p5.ParameterName = "@ContentLength";
					p5.DbType = DbType.Int32;
					p5.Value = content.Length;
					command.Parameters.Add(p5);

					var p6 = command.CreateParameter();
					p6.ParameterName = "@Content";
					p6.DbType = DbType.Binary;
					p6.Size = content.Length;
					p6.Value = content;
					command.Parameters.Add(p6);

					command.ExecuteNonQuery();
				}
			}
		}

		public IEnumerable<CompositeObjectChangeSet> GetChangeSets(Guid compositeObjectId)
		{
			return GetChangeSets(compositeObjectId, 0);
		}

		public long GetMaxTimestamp(Guid compositeObjectId)
		{
			using (var connection = dbConnectionFactory.Get())
			{
				connection.Open();

				using (var command = connection.CreateCommand())
				{
					command.CommandText = "SELECT MAX(Timestamp) FROM CompositeObjectChangeSets WHERE CompositeObjectId = @CompositeObjectId";
					
					var p1 = command.CreateParameter();
					p1.ParameterName = "@CompositeObjectId";
					p1.DbType = DbType.Guid;
					p1.Value = compositeObjectId;
					command.Parameters.Add(p1);

					var result = command.ExecuteScalar();
					return result is int maxTimestamp ? maxTimestamp : 0;
				}
			}
		}

		public IEnumerable<CompositeObjectChangeSet> GetChangeSets(Guid compositeObjectId, long sinceTimestamp)
		{
			using (var connection = dbConnectionFactory.Get())
			{
				connection.Open();
				using (var command = connection.CreateCommand())
				{
					command.CommandText =
						"SELECT ChangeSetId, ContentLength, Content FROM CompositeObjectChangeSets WHERE CompositeObjectId = @CompositeObjectId AND Timestamp >= @Timestamp ORDER BY Timestamp";
				
					var p1 = command.CreateParameter();
					p1.ParameterName = "@CompositeObjectId";
					p1.DbType = DbType.Guid;
					p1.Value = compositeObjectId;
					command.Parameters.Add(p1);

					var p2 = command.CreateParameter();
					p2.ParameterName = "@Timestamp";
					p2.DbType = DbType.Int64;
					p2.Value = sinceTimestamp;
					command.Parameters.Add(p2);

					using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
					{
						while (reader.Read())
						{
							var changeSetId = reader.GetGuid(0);
							var contentLength = reader.GetInt32(1);
							var bytes = new byte[contentLength];
							var bytesRead = reader.GetBytes(2, 0, bytes, 0, contentLength);

							if (bytesRead != contentLength)
								throw new IOException($"Only {bytesRead} bytes of content of chage set {changeSetId} read out of {contentLength}");

							var compositeObjectChangeSet = Serializer.Deserialize<CompositeObjectChangeSet>(new MemoryStream(bytes));

							yield return compositeObjectChangeSet;
						}
					}
				}

			}
		}

		public void UpdateChangeSet(CompositeObjectChangeSet changeSet, bool yesIKnowWhatIAmDoing = false)
		{
			if(!yesIKnowWhatIAmDoing)
				throw new Exception("You must know what are you doing!");

			var stream = new MemoryStream();
			Serializer.Serialize(stream, changeSet);
			var content = stream.ToArray();

			using (var connection = dbConnectionFactory.Get())
			{
				connection.Open();

				using (var command = connection.CreateCommand())
				{
					command.CommandText =
						"UPDATE CompositeObjectChangeSets SET Timestamp = @Timestamp, ContentLength = @ContentLength, Content = @Content) WHERE CompositeObjectId = @CompositeObjectId AND ChangeSetId = @ChangeSetId";
				
					var p1 = command.CreateParameter();
					p1.ParameterName = "@CompositeObjectId";
					p1.DbType = DbType.Guid;
					p1.Value = changeSet.CompositeObjectId;
					command.Parameters.Add(p1);

					var p2 = command.CreateParameter();
					p2.ParameterName = "@ChangeSetId";
					p2.DbType = DbType.Guid;
					p2.Value = changeSet.ChangeSetId;
					command.Parameters.Add(p2);

					var p3 = command.CreateParameter();
					p3.ParameterName = "@Timestamp";
					p3.DbType = DbType.Int64;
					p3.Value = changeSet.Timestamp;
					command.Parameters.Add(p3);

					var p4 = command.CreateParameter();
					p4.ParameterName = "@ContentLength";
					p4.DbType = DbType.Int32;
					p4.Value = content.Length;
					command.Parameters.Add(p4);

					var p5 = command.CreateParameter();
					p5.ParameterName = "@Content";
					p5.DbType = DbType.Binary;
					p5.Size = content.Length;
					p5.Value = content;
					command.Parameters.Add(p5);

					command.ExecuteNonQuery();
				}
			}
		}
	}
}