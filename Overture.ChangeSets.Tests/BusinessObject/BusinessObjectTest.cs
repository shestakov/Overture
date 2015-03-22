using System;
using System.Linq;
using NUnit.Framework;
using Overture.ChangeSets.BusinessObjects;
using Overture.ChangeSets.Protobuf.Composite;
using Overture.ChangeSets.Protobuf.Simple;
using Overture.ChangeSets.SerializerProvider;
using Overture.ChangeSets.Tests.Infrastructure;
using Overture.ChangeSets.Tests.MockingNinja;

namespace Overture.ChangeSets.Tests.BusinessObject
{
	[TestFixture]
	public class BusinessObjectTest : TestWithContainer
	{
		private readonly TestBusinessObjectDefinitionProvider businessObjectDefinitionProvider = new TestBusinessObjectDefinitionProvider();
		private readonly Guid userId = new Guid("50665A1A-D5A5-423F-B3BF-DB1CCC3C68B5");
		private readonly Guid compositeObjectTypeId = new Guid(CompositeObjectTestData.StaticCompositeObjectTypeId);
		private readonly Guid simpleObjectTypeId = new Guid(CompositeObjectTestData.StaticSimpleObjectTypeId);
		private Protobuf.AttributeValues.AttributeValueFactory attributeValueFactory;

		[SetUp]
		public new void SetUp()
		{
			attributeValueFactory = new Protobuf.AttributeValues.AttributeValueFactory(new AttributeValueSerializerProvider(),
				new TestBusinessObjectDefinitionProvider());
		}

		[Test]
		public void CompositeObject_Initialization()
		{
			var compositeObjectId = new Guid("3C7BEEC3-9418-49CF-9B63-6309EB9154AA");
			var changeSetId1 = new Guid("E84DDA60-F418-4F11-91DE-773CEE47B2C8");
			
			var timeStamp1 = DateTimeOffset.UtcNow.Ticks;
			var dateTime1 = DateTimeOffset.UtcNow.Ticks;
			var compositeObjectAttributeValues1 = new[]
			{
				attributeValueFactory.Create(() => TestCompositeBusinessObjectStatic.Int32Property, CompositeObjectTestData.TestValues.Int32Value),
				attributeValueFactory.Create(() => TestCompositeBusinessObjectStatic.StringProperty, CompositeObjectTestData.TestValues.StringValue)
			};

			var simpleObjectId = new Guid("25E5692F-2262-4013-B668-15EA40F83A9B");
			var simpleObjectChangeSetId1 = new Guid("89E51585-1FF9-458F-9C9D-E66B3EB1F8EC");
			var simpleObjectAttributeValues1 = new[]
			{
				attributeValueFactory.Create(() => TestCompositeBusinessObjectStatic.Int64Property, CompositeObjectTestData.TestValues.Int64Value),
				attributeValueFactory.Create(() => TestCompositeBusinessObjectStatic.EnumProperty, CompositeObjectTestData.TestValues.EnumValue)
			};

			var simpleObjectChangeSet1 = new CreateSimpleObjectChangeSet(simpleObjectChangeSetId1, simpleObjectId, simpleObjectAttributeValues1, simpleObjectTypeId, null);

			SimpleObjectChangeSet[] simpleObjectChangeSets = { simpleObjectChangeSet1 };

			var changeSet1 = new CreateCompositeObjectChangeSet(compositeObjectId, changeSetId1, timeStamp1, compositeObjectTypeId,
				simpleObjectChangeSets, compositeObjectAttributeValues1, userId, dateTime1);

			var changeSets = new[] { changeSet1 };
			var compositeObject = new CompositeObject(changeSets, businessObjectDefinitionProvider);

			Assert.AreEqual(compositeObjectTypeId, compositeObject.CompositeObjectTypeId);
			Assert.AreEqual(compositeObjectId, compositeObject.Id);
			Assert.AreEqual(changeSetId1, compositeObject.Revision);
			Assert.AreEqual(new DateTimeOffset(dateTime1, new TimeSpan(0)), compositeObject.LastModified);
			Assert.AreEqual(compositeObjectAttributeValues1.Length, compositeObject.Attributes.Count);
			Assert.AreEqual(CompositeObjectTestData.TestValues.Int32Value, compositeObject.Attribute(() => TestCompositeBusinessObjectStatic.Int32Property));
			Assert.AreEqual(CompositeObjectTestData.TestValues.StringValue, compositeObject.Attribute(() => TestCompositeBusinessObjectStatic.StringProperty));

			Assert.AreEqual(1, compositeObject.SimpleObjects.Count);
			var simpleObject = compositeObject.SimpleObjects[simpleObjectId];
			Assert.AreEqual(simpleObjectTypeId, simpleObject.SimpleObjectTypeId);
			Assert.AreEqual(simpleObjectId, simpleObject.Id);
			Assert.AreEqual(changeSetId1, simpleObject.Revision);
			Assert.AreEqual(simpleObjectAttributeValues1.Length, simpleObject.Attributes.Count);
			Assert.AreEqual(CompositeObjectTestData.TestValues.Int64Value, simpleObject.Attribute(() => TestCompositeBusinessObjectStatic.Int64Property));
			Assert.AreEqual(CompositeObjectTestData.TestValues.EnumValue, simpleObject.Attribute(() => TestCompositeBusinessObjectStatic.EnumProperty));
		}

		[Test]
		public void CompositeObject_NoChangeSets()
		{
			var changeSets = Enumerable.Empty<CompositeObjectChangeSet>();
			Assert.Throws<Exception>(() => new CompositeObject(changeSets, businessObjectDefinitionProvider), "CompositeObject has not been initialized");
		}
	}
}