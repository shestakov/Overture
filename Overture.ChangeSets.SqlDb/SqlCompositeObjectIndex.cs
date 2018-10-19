using System;
using System.Collections.Generic;
using System.Data;
using Overture.ChangeSets.Protobuf.Composite;
using Overture.ChangeSets.Storage;
using ProtoBuf;

namespace Overture.ChangeSets.SqlDb
{
	public class SqlCompositeObjectIndex : ICompositeObjectIndex
	{
		private readonly IDbConnectionFactory dbConnectionFactory;

		public SqlCompositeObjectIndex(IDbConnectionFactory dbConnectionFactory)
		{
			this.dbConnectionFactory = dbConnectionFactory;
			Serializer.PrepareSerializer<CompositeObjectChangeSet>();
		}

		public void Add(Guid ownerId, Guid compositeObjectId, Guid compositeObjectTypeId)
		{
			var utcNow = DateTimeOffset.UtcNow;

			using (var connection = dbConnectionFactory.Get())
			{
				connection.Open();

				using (var command = connection.CreateCommand())
				{
					command.CommandText =
						"INSERT INTO CompositeObjectIndex (OwnerId, CompositeObjectId, CompositeObjectTypeId, DateTimeCreated, DateTimeDeleted) VALUES (@OwnerId, @CompositeObjectId, @CompositeObjectTypeId, @DateTimeCreated, NULL)";
				
					var p1 = command.CreateParameter();
					p1.ParameterName = "@OwnerId";
					p1.DbType = DbType.Guid;
					p1.Value = ownerId;
					command.Parameters.Add(p1);

					var p2 = command.CreateParameter();
					p2.ParameterName = "@CompositeObjectId";
					p2.DbType = DbType.Guid;
					p2.Value = compositeObjectId;
					command.Parameters.Add(p2);

					var p3 = command.CreateParameter();
					p3.ParameterName = "@CompositeObjectTypeId";
					p3.DbType = DbType.Guid;
					p3.Value = compositeObjectTypeId;
					command.Parameters.Add(p3);

					var p4 = command.CreateParameter();
					p4.ParameterName = "@DateTimeCreated";
					p4.DbType = DbType.DateTimeOffset;
					p4.Value = utcNow;
					command.Parameters.Add(p4);

					command.ExecuteNonQuery();
				}
			}
		}

		public IEnumerable<Guid> GetList(Guid compositeObjectTypeId, Guid ownerId)
		{
			using (var connection = dbConnectionFactory.Get())
			{
				connection.Open();
				using (var command = connection.CreateCommand())
				{
					command.CommandText =
						"SELECT CompositeObjectId FROM CompositeObjectIndex WHERE CompositeObjectTypeId = @CompositeObjectTypeId AND OwnerId = @OwnerId AND DateTimeDeleted IS NULL";
				
					var p1 = command.CreateParameter();
					p1.ParameterName = "@CompositeObjectTypeId";
					p1.DbType = DbType.Guid;
					p1.Value = compositeObjectTypeId;
					command.Parameters.Add(p1);

					var p2 = command.CreateParameter();
					p2.ParameterName = "@OwnerId";
					p2.DbType = DbType.Guid;
					p2.Value = ownerId;
					command.Parameters.Add(p2);

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var compositeObjectId = reader.GetGuid(0);
							yield return compositeObjectId;
						}
					}
				}
			}
		}

		public IEnumerable<Guid> GetList(Guid compositeObjectTypeId)
		{
			using (var connection = dbConnectionFactory.Get())
			{
				connection.Open();
				using (var command = connection.CreateCommand())
				{
					command.CommandText =
						"SELECT CompositeObjectId FROM CompositeObjectIndex WHERE CompositeObjectTypeId = @CompositeObjectTypeId AND DateTimeDeleted IS NULL";
				
					var p1 = command.CreateParameter();
					p1.ParameterName = "@CompositeObjectTypeId";
					p1.DbType = DbType.Guid;
					p1.Value = compositeObjectTypeId;
					command.Parameters.Add(p1);

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var compositeObjectId = reader.GetGuid(0);
							yield return compositeObjectId;
						}
					}
				}
			}
		}

		public Guid GetOwner(Guid compositeObjectId)
		{
			using (var connection = dbConnectionFactory.Get())
			{
				connection.Open();

				using (var command = connection.CreateCommand())
				{
					command.CommandText = "SELECT OwnerId FROM CompositeObjectIndex WHERE CompositeObjectId = @CompositeObjectId";

					var p1 = command.CreateParameter();
					p1.ParameterName = "@CompositeObjectId";
					p1.DbType = DbType.Guid;
					p1.Value = compositeObjectId;
					command.Parameters.Add(p1);

					var result = command.ExecuteScalar();
					return result is Guid ownerId ? ownerId : throw new Exception($"CompositeObject {compositeObjectId} not found");
				}
			}
		}
	}
}