using System;
using Overture.ChangeSets.Attributes;

namespace Overture.ChangeSets.Tests.Infrastructure
{
	[SimpleObject(CompositeObjectTestData.StaticSimpleObjectTypeId)]
	internal static class TestSimpleBusinessObjectStatic
	{
#pragma warning disable 0649
		// ReSharper disable UnusedField.Compiler
		public static int Int32Property;
		public static int? NullableInt32Property;
		public static long Int64Property;
		public static long? NullableInt64Property;
		public static double DoubleProperty;
		public static double? NullableDoubleProperty;
		public static Guid GuidProperty;
		public static Guid? NullableGuidProperty;
		public static DateTimeOffset DateTimeOffsetProperty;
		public static DateTimeOffset? NullableDateTimeOffsetProperty;
		public static string StringProperty;
		public static TestEnum EnumProperty;
		public static TestEnum? NullableEnumProperty;
		// ReSharper restore UnusedField.Compiler
#pragma warning restore 0649
	}
}