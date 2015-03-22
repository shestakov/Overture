using System;
using NUnit.Framework;
using Overture.ChangeSets.SerializerProvider;
using Overture.ChangeSets.Tests.Infrastructure;
using Overture.ChangeSets.Tests.MockingNinja;

namespace Overture.ChangeSets.Tests.AttributeValueFactory
{
	public class AttributeValueFactoryTest_Create : TestWithContainer
	{
		private readonly TestSerializer testSerializer = new TestSerializer();

		[Test]
		public void Static_NotNull()
		{
			CompositeObjectTestData.SetupMockAttributeValueSerializerProvider(RegisterStrictMock<IAttributeValueSerializerProvider>());

			Mocks.ReplayAll();

			var factory = Resolve<Protobuf.AttributeValues.AttributeValueFactory>();

			var attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.Int32Property, CompositeObjectTestData.TestValues.Int32Value);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int32)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.Int32Value), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableInt32Property,
				CompositeObjectTestData.TestValues.NullableInt32Value);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int32?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableInt32Value), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.Int64Property, CompositeObjectTestData.TestValues.Int64Value);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int64)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.Int64Value), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableInt64Property,
				CompositeObjectTestData.TestValues.NullableInt64Value);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int64?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableInt64Value), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.DoubleProperty, CompositeObjectTestData.TestValues.DoubleValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Double)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.DoubleValue), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableDoubleProperty,
				CompositeObjectTestData.TestValues.NullableDoubleValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Double?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableDoubleValue), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.GuidProperty, CompositeObjectTestData.TestValues.GuidValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Guid)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.GuidValue), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableGuidProperty, CompositeObjectTestData.TestValues.NullableGuidValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Guid?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableGuidValue), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.DateTimeOffsetProperty,
				CompositeObjectTestData.TestValues.DateTimeOffsetValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(DateTimeOffset)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.DateTimeOffsetValue), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableDateTimeOffsetProperty,
				CompositeObjectTestData.TestValues.NullableDateTimeOffsetValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(DateTimeOffset?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableDateTimeOffsetValue), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.StringProperty, CompositeObjectTestData.TestValues.StringValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(String)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.StringValue), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.EnumProperty, CompositeObjectTestData.TestValues.EnumValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(TestEnum)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.EnumValue), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableEnumProperty, CompositeObjectTestData.TestValues.NullableEnumValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(TestEnum?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableEnumValue), attributeValue.Value);

			Mocks.VerifyAll();
		}

		[Test]
		public void Static_Null()
		{
			CompositeObjectTestData.SetupMockAttributeValueSerializerProvider_NullableOnly(RegisterStrictMock<IAttributeValueSerializerProvider>());

			Mocks.ReplayAll();

			var factory = Resolve<Protobuf.AttributeValues.AttributeValueFactory>();

			var attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableInt32Property, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int32?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((Int32?) null), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableInt64Property, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int64?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((Int64?) null), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableDoubleProperty, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Double?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((Double?) null), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableGuidProperty, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Guid?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((Guid?) null), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableDateTimeOffsetProperty, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(DateTimeOffset?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((DateTimeOffset?) null), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.StringProperty, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(String)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((String) null), attributeValue.Value);

			attributeValue = factory.Create(() => TestCompositeBusinessObjectStatic.NullableEnumProperty, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(TestEnum?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((TestEnum?) null), attributeValue.Value);

			Mocks.VerifyAll();
		}

		[Test]
		public void Concrete()
		{
			CompositeObjectTestData.SetupMockAttributeValueSerializerProvider(RegisterStrictMock<IAttributeValueSerializerProvider>());

			Mocks.ReplayAll();

			var factory = Resolve<Protobuf.AttributeValues.AttributeValueFactory>();

			var attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Int32>(conrete => conrete.Int32Property, CompositeObjectTestData.TestValues.Int32Value);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int32)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.Int32Value), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Int32?>(conrete => conrete.NullableInt32Property, CompositeObjectTestData.TestValues.NullableInt32Value);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int32?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableInt32Value), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Int64>(conrete => conrete.Int64Property, CompositeObjectTestData.TestValues.Int64Value);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int64)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.Int64Value), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Int64?>(conrete => conrete.NullableInt64Property, CompositeObjectTestData.TestValues.NullableInt64Value);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int64?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableInt64Value), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Double>(conrete => conrete.DoubleProperty, CompositeObjectTestData.TestValues.DoubleValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Double)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.DoubleValue), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Double?>(conrete => conrete.NullableDoubleProperty, CompositeObjectTestData.TestValues.NullableDoubleValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Double?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableDoubleValue), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Guid>(conrete => conrete.GuidProperty, CompositeObjectTestData.TestValues.GuidValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Guid)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.GuidValue), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Guid?>(conrete => conrete.NullableGuidProperty, CompositeObjectTestData.TestValues.NullableGuidValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Guid?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableGuidValue), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, DateTimeOffset>(conrete => conrete.DateTimeOffsetProperty, CompositeObjectTestData.TestValues.DateTimeOffsetValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(DateTimeOffset)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.DateTimeOffsetValue), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, DateTimeOffset?>(conrete => conrete.NullableDateTimeOffsetProperty, CompositeObjectTestData.TestValues.NullableDateTimeOffsetValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(DateTimeOffset?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableDateTimeOffsetValue), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, String>(conrete => conrete.StringProperty, CompositeObjectTestData.TestValues.StringValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(String)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.StringValue), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, TestEnum>(conrete => conrete.EnumProperty, CompositeObjectTestData.TestValues.EnumValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(TestEnum)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.EnumValue), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, TestEnum?>(conrete => conrete.NullableEnumProperty, CompositeObjectTestData.TestValues.NullableEnumValue);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(TestEnum?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableEnumValue), attributeValue.Value);

			Mocks.VerifyAll();
		}

		[Test]
		public void Concrete_Null()
		{
			CompositeObjectTestData.SetupMockAttributeValueSerializerProvider_NullableOnly(RegisterStrictMock<IAttributeValueSerializerProvider>());

			Mocks.ReplayAll();

			var factory = Resolve<Protobuf.AttributeValues.AttributeValueFactory>();

			var attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Int32?>(conrete => conrete.NullableInt32Property, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int32?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((Int32?)null), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Int64?>(conrete => conrete.NullableInt64Property, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Int64?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((Int64?)null), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Double?>(conrete => conrete.NullableDoubleProperty, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Double?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((Double?)null), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, Guid?>(conrete => conrete.NullableGuidProperty, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(Guid?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((Guid?)null), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, DateTimeOffset?>(conrete => conrete.NullableDateTimeOffsetProperty, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(DateTimeOffset?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((DateTimeOffset?)null), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, String>(conrete => conrete.StringProperty, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(String)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((String)null), attributeValue.Value);

			attributeValue = factory.Create<TestCompositeBusinessObjectConrete, TestEnum?>(conrete => conrete.NullableEnumProperty, null);
			Assert.AreEqual(CompositeObjectTestData.TestAttributes[typeof(TestEnum?)].Name, attributeValue.Name);
			Assert.AreEqual(testSerializer.Serialize((TestEnum?)null), attributeValue.Value);

			Mocks.VerifyAll();
		}
	}
}