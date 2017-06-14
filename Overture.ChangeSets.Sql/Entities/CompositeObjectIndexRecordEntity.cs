using System;
using System.Data.Linq.Mapping;

namespace Overture.ChangeSets.Sql.Entities
{
	[Table(Name = "CompositeObjectIndex")]
	public class CompositeObjectIndexRecordEntity
	{
		[Column(IsPrimaryKey = true)]
		public Guid OwnerId { get; set; }

		[Column(IsPrimaryKey = true)]
		public Guid CompositeObjectId { get; set; }

		[Column]
		public Guid CompositeObjectTypeId { get; set; }

		[Column]
		public DateTimeOffset DateTimeCreated { get; set; }

		[Column]
		public DateTimeOffset? DateTimeDeleted { get; set; }
	}
}