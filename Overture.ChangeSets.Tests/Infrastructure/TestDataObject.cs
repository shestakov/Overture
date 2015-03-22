using System;

namespace Overture.ChangeSets.Tests.Infrastructure
{
	internal class TestDataObject
	{
		// ReSharper disable UnusedAutoPropertyAccessor.Global
		public int Int32Property { get; set; }
		public int? NullableInt32Property { get; set; }
		public long Int64Property { get; set; }
		public long? NullableInt64Property { get; set; }
		public double DoubleProperty { get; set; }
		public double? NullableDoubleProperty { get; set; }
		public Guid GuidProperty { get; set; }
		public Guid? NullableGuidProperty { get; set; }
		public DateTimeOffset DateTimeOffsetProperty { get; set; }
		public DateTimeOffset? NullableDateTimeOffsetProperty { get; set; }
		public string StringProperty { get; set; }
		public TestEnum EnumProperty { get; set; }
		public TestEnum? NullableEnumProperty { get; set; }
		public int ExtraProperty { get; set; }
		// ReSharper restore UnusedAutoPropertyAccessor.Global
	}
}