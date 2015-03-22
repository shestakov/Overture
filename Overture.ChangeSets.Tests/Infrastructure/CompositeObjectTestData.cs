using System;
using System.Collections.Generic;
using Overture.ChangeSets.SerializerProvider;
using Overture.ChangeSets.Serializers;
using Rhino.Mocks;

namespace Overture.ChangeSets.Tests.Infrastructure
{
	internal static class CompositeObjectTestData
	{
		public const string ConcreteCompositeObjectTypeId = "A8CB34BC-9D14-426C-8957-C6A58619C3AE";
		public const string StaticCompositeObjectTypeId = "66EABE19-97EF-4F84-AFF8-133859B894A0";
		public const string ConcreteSimpleObjectTypeId = "79E5D580-4058-4C3A-BDAB-C1D9C7C8785B";
		public const string StaticSimpleObjectTypeId = "BC2FFF5E-D632-48AB-AF4F-267B8772EDCF";

		internal static readonly Dictionary<Type, Guid> ClassBinding = new Dictionary<Type, Guid>
		{
			{ typeof(TestCompositeBusinessObjectConrete), new Guid(ConcreteCompositeObjectTypeId) },
			{ typeof(TestCompositeBusinessObjectStatic), new Guid(StaticCompositeObjectTypeId) }
		};

		internal static readonly Dictionary<Type, AttributeTestData> TestAttributes = new Dictionary<Type, AttributeTestData>
		{
			{ typeof(Int32), new AttributeTestData { Name = "Int32Property", SerializerType = typeof(Int32Serializer) } },
			{ typeof(Int32?), new AttributeTestData { Name = "NullableInt32Property", SerializerType = typeof(NullableInt32Serializer) } },
			{ typeof(Int64), new AttributeTestData { Name = "Int64Property", SerializerType = typeof(Int64Serializer) } },
			{ typeof(Int64?), new AttributeTestData { Name = "NullableInt64Property", SerializerType = typeof(NullableInt64Serializer) } },
			{ typeof(Double), new AttributeTestData { Name = "DoubleProperty", SerializerType = typeof(DoubleSerializer) } },
			{ typeof(Double?), new AttributeTestData { Name = "NullableDoubleProperty", SerializerType = typeof(NullableDoubleSerializer) } },
			{ typeof(Guid), new AttributeTestData { Name = "GuidProperty", SerializerType = typeof(GuidSerializer) } },
			{ typeof(Guid?), new AttributeTestData { Name = "NullableGuidProperty", SerializerType = typeof(NullableGuidSerializer) } },
			{ typeof(DateTimeOffset), new AttributeTestData { Name = "DateTimeOffsetProperty", SerializerType = typeof(DateTimeOffsetSerializer) } },
			{ typeof(DateTimeOffset?), new AttributeTestData { Name = "NullableDateTimeOffsetProperty", SerializerType = typeof(NullableDateTimeOffsetSerializer) } },
			{ typeof(String), new AttributeTestData { Name = "StringProperty", SerializerType = typeof(StringSerializer) } },
			{ typeof(TestEnum), new AttributeTestData { Name = "EnumProperty", SerializerType = typeof(EnumSerializer) } },
			{ typeof(TestEnum?), new AttributeTestData { Name = "NullableEnumProperty", SerializerType = typeof(NullableEnumSerializer) } }
		};

		internal static void SetupMockAttributeValueSerializerProvider(IAttributeValueSerializerProvider mock)
		{
			mock.Expect(e => e.Get(typeof(Int32))).Return(new Int32Serializer());
			mock.Expect(e => e.Get(typeof(Int32?))).Return(new NullableInt32Serializer());
			mock.Expect(e => e.Get(typeof(Int64))).Return(new Int64Serializer());
			mock.Expect(e => e.Get(typeof(Int64?))).Return(new NullableInt64Serializer());
			mock.Expect(e => e.Get(typeof(Double))).Return(new DoubleSerializer());
			mock.Expect(e => e.Get(typeof(Double?))).Return(new NullableDoubleSerializer());
			mock.Expect(e => e.Get(typeof(Guid))).Return(new GuidSerializer());
			mock.Expect(e => e.Get(typeof(Guid?))).Return(new NullableGuidSerializer());
			mock.Expect(e => e.Get(typeof(DateTimeOffset))).Return(new DateTimeOffsetSerializer());
			mock.Expect(e => e.Get(typeof(DateTimeOffset?))).Return(new NullableDateTimeOffsetSerializer());
			mock.Expect(e => e.Get(typeof(String))).Return(new StringSerializer());
			mock.Expect(e => e.Get(typeof(TestEnum))).Return(new EnumSerializer());
			mock.Expect(e => e.Get(typeof(TestEnum?))).Return(new NullableEnumSerializer());
		}

		internal static void SetupMockAttributeValueSerializerProvider_NullableOnly(IAttributeValueSerializerProvider mock)
		{
			mock.Expect(e => e.Get(typeof(Int32?))).Return(new NullableInt32Serializer());
			mock.Expect(e => e.Get(typeof(Int64?))).Return(new NullableInt64Serializer());
			mock.Expect(e => e.Get(typeof(Double?))).Return(new NullableDoubleSerializer());
			mock.Expect(e => e.Get(typeof(Guid?))).Return(new NullableGuidSerializer());
			mock.Expect(e => e.Get(typeof(DateTimeOffset?))).Return(new NullableDateTimeOffsetSerializer());
			mock.Expect(e => e.Get(typeof(String))).Return(new StringSerializer());
			mock.Expect(e => e.Get(typeof(TestEnum?))).Return(new NullableEnumSerializer());
		}

		internal static class TestValues
		{
			// ReSharper disable UnusedField.Compiler
			// ReSharper disable UnusedMember.Global
			public const Int32 Int32Value = Int32.MaxValue;
			public static readonly Int32? NullableInt32Value = Int32.MaxValue;
			public const Int64 Int64Value = Int64.MaxValue;
			public static readonly Int64? NullableInt64Value = Int64.MaxValue;
			public const Double DoubleValue = Double.MaxValue;
			public static readonly Double? NullableDoubleValue = Double.MaxValue;
			public static readonly Guid GuidValue = new Guid("7B7663B4-796C-4621-A6D1-7259A35DCE2A");
			public static readonly Guid? NullableGuidValue = new Guid("4FB2BE1B-921A-4270-B6A0-C105F1C78A72");
			public static readonly DateTimeOffset DateTimeOffsetValue = DateTimeOffset.UtcNow;
			public static readonly DateTimeOffset? NullableDateTimeOffsetValue = DateTimeOffset.UtcNow;
			public const string StringValue = "TEST";
			public const TestEnum EnumValue = TestEnum.TestValue;
			public static readonly TestEnum? NullableEnumValue = TestEnum.TestValue;
			// ReSharper restore UnusedMember.Global
			// ReSharper restore UnusedField.Compiler
		}
	}
}