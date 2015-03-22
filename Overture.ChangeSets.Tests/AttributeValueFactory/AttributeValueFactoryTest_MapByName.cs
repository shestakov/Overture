using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.Protobuf.AttributeValues;
using Overture.ChangeSets.Tests.Infrastructure;
using Overture.ChangeSets.Tests.MockingNinja;
using Rhino.Mocks;

namespace Overture.ChangeSets.Tests.AttributeValueFactory
{
	public class AttributeValueFactoryTest_MapByName : TestWithContainer
	{
		private readonly TestBusinessObjectDefinitionProvider definitionProvider = new TestBusinessObjectDefinitionProvider();
		private readonly TestSerializer testSerializer = new TestSerializer();

		[Test]
		public void Concrete_AllValues()
		{
			MockBusinessObjectDefinitionProvider_Concrete();
			var dataObject = MakeDataObject_AllValues();
			var factory = Resolve<Protobuf.AttributeValues.AttributeValueFactory>();
			var attributeValues = factory.MapByName<TestCompositeBusinessObjectConrete>(dataObject);
			AssertAttrubuteValues_AllValues(attributeValues);
			Mocks.VerifyAll();
		}

		[Test]
		public void Concrete_NullableOnly()
		{
			MockBusinessObjectDefinitionProvider_Concrete();
			var dataObject = MakeDataObject_NullableOnly();
			var factory = Resolve<Protobuf.AttributeValues.AttributeValueFactory>();
			var attributeValues = factory.MapByName<TestCompositeBusinessObjectConrete>(dataObject);
			AssertAttributeValues_NullableOnly(attributeValues);
			Mocks.VerifyAll();
		}

		[Test]
		public void Static_AllValues()
		{
			MockBusinessObjectDefinitionProvider_Static();
			var dataObject = MakeDataObject_AllValues();
			var factory = Resolve<Protobuf.AttributeValues.AttributeValueFactory>();
			var attributeValues = factory.MapByName(typeof(TestCompositeBusinessObjectStatic), dataObject);
			AssertAttrubuteValues_AllValues(attributeValues);
			Mocks.VerifyAll();
		}

		[Test]
		public void Static_NullableOnly()
		{
			MockBusinessObjectDefinitionProvider_Static();
			var dataObject = MakeDataObject_NullableOnly();
			var factory = Resolve<Protobuf.AttributeValues.AttributeValueFactory>();
			var attributeValues = factory.MapByName(typeof(TestCompositeBusinessObjectStatic), dataObject);
			AssertAttributeValues_NullableOnly(attributeValues);
			Mocks.VerifyAll();
		}

		private void MockBusinessObjectDefinitionProvider_Concrete()
		{
			var compositeObjectTypeId = new Guid(CompositeObjectTestData.ConcreteCompositeObjectTypeId);
			RegisterStrictMock<IBusinessObjectDefinitionProvider>()
				.Expect(e => e.GetCompositeObjectDefinition(compositeObjectTypeId))
				.Return(definitionProvider.GetCompositeObjectDefinition(compositeObjectTypeId));
		}

		private void MockBusinessObjectDefinitionProvider_Static()
		{
			var compositeObjectTypeId = new Guid(CompositeObjectTestData.StaticCompositeObjectTypeId);
			RegisterStrictMock<IBusinessObjectDefinitionProvider>()
				.Expect(e => e.GetCompositeObjectDefinition(compositeObjectTypeId))
				.Return(definitionProvider.GetCompositeObjectDefinition(compositeObjectTypeId));
		}

		private void AssertAttrubuteValues_AllValues(ICollection<AttributeValue> attributeValues)
		{
			if(attributeValues == null) throw new ArgumentNullException("attributeValues");
			Assert.AreEqual(CompositeObjectTestData.TestAttributes.Count, attributeValues.Count);

			var dictionary = attributeValues.ToDictionary(av => av.Name);

			var attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Int32)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.Int32Value), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Int32?)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableInt32Value), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Int64)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.Int64Value), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Int64?)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableInt64Value), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Double)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.DoubleValue), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Double?)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableDoubleValue), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Guid)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.GuidValue), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Guid?)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableGuidValue), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(DateTimeOffset)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.DateTimeOffsetValue), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(DateTimeOffset?)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableDateTimeOffsetValue), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(String)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.StringValue), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(TestEnum)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.EnumValue), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(TestEnum?)].Name];
			Assert.AreEqual(testSerializer.Serialize(CompositeObjectTestData.TestValues.NullableEnumValue), attributeValue.Value);
		}

		private void AssertAttributeValues_NullableOnly(ICollection<AttributeValue> attributeValues)
		{
			Assert.AreEqual(CompositeObjectTestData.TestAttributes.Count, attributeValues.Count);

			var dictionary = attributeValues.ToDictionary(av => av.Name);

			var attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Int32)].Name];
			Assert.AreEqual(testSerializer.Serialize(default(Int32)), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Int32?)].Name];
			Assert.AreEqual(testSerializer.Serialize((int?) null), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Int64)].Name];
			Assert.AreEqual(testSerializer.Serialize(default(Int64)), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Int64?)].Name];
			Assert.AreEqual(testSerializer.Serialize((long?) null), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Double)].Name];
			Assert.AreEqual(testSerializer.Serialize(default(Double)), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Double?)].Name];
			Assert.AreEqual(testSerializer.Serialize((double?) null), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Guid)].Name];
			Assert.AreEqual(testSerializer.Serialize(default(Guid)), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(Guid?)].Name];
			Assert.AreEqual(testSerializer.Serialize((Guid?) null), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(DateTimeOffset)].Name];
			Assert.AreEqual(testSerializer.Serialize(default(DateTimeOffset)), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(DateTimeOffset?)].Name];
			Assert.AreEqual(testSerializer.Serialize((DateTimeOffset?) null), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(String)].Name];
			Assert.AreEqual(testSerializer.Serialize((string) null), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(TestEnum)].Name];
			Assert.AreEqual(testSerializer.Serialize(default(TestEnum)), attributeValue.Value);

			attributeValue = dictionary[CompositeObjectTestData.TestAttributes[typeof(TestEnum?)].Name];
			Assert.AreEqual(testSerializer.Serialize((TestEnum?) null), attributeValue.Value);
		}

		private static TestDataObject MakeDataObject_AllValues()
		{
			var dataObject = new TestDataObject
			{
				Int32Property = CompositeObjectTestData.TestValues.Int32Value,
				NullableInt32Property = CompositeObjectTestData.TestValues.NullableInt32Value,
				Int64Property = CompositeObjectTestData.TestValues.Int64Value,
				NullableInt64Property = CompositeObjectTestData.TestValues.NullableInt64Value,
				DoubleProperty = CompositeObjectTestData.TestValues.DoubleValue,
				NullableDoubleProperty = CompositeObjectTestData.TestValues.NullableDoubleValue,
				GuidProperty = CompositeObjectTestData.TestValues.GuidValue,
				NullableGuidProperty = CompositeObjectTestData.TestValues.NullableGuidValue,
				DateTimeOffsetProperty = CompositeObjectTestData.TestValues.DateTimeOffsetValue,
				NullableDateTimeOffsetProperty = CompositeObjectTestData.TestValues.NullableDateTimeOffsetValue,
				StringProperty = CompositeObjectTestData.TestValues.StringValue,
				EnumProperty = CompositeObjectTestData.TestValues.EnumValue,
				NullableEnumProperty = CompositeObjectTestData.TestValues.NullableEnumValue,
				ExtraProperty = 1,
			};
			return dataObject;
		}

		private static TestDataObject MakeDataObject_NullableOnly()
		{
			var dataObject = new TestDataObject
			{
				NullableInt32Property = null,
				NullableInt64Property = null,
				NullableDoubleProperty = null,
				NullableGuidProperty = null,
				NullableDateTimeOffsetProperty = null,
				StringProperty = null,
				NullableEnumProperty = null,
				ExtraProperty = 1,
			};
			return dataObject;
		}
	}
}