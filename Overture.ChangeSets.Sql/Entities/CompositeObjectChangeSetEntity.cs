using System;
using System.Data.Linq.Mapping;

namespace Overture.ChangeSets.Sql.Entities
{
	[Table(Name = "CompositeObjectChangeSets")]
	public class CompositeObjectChangeSetEntity
	{
		[Column(IsPrimaryKey = true)]
		public Guid CompositeObjectId { get; set; }

		[Column(IsPrimaryKey = true)]
		public Guid ChangeSetId { get; set; }

		[Column]
		public long Timestamp { get; set; }

		[Column]
		public int ContentLength { get; set; }
		
		[Column]
		public byte[] Content { get; set; }
	}
}