using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Overture.ChangeSets.BusinessObjects;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.Definitions;
using Overture.ChangeSets.Serializers;
using Overture.ChangeSets.Tests.Infrastructure;
using Overture.ChangeSets.Tests.MockingNinja;
using Rhino.Mocks;

namespace Overture.ChangeSets.Tests.BusinessObject
{
	[TestFixture]
	public class BusinessObjectSerialization : TestWithContainer
	{
		private readonly TestBusinessObjectDefinitionProvider testBusinessObjectDefinitionProvider = new TestBusinessObjectDefinitionProvider();

		[Test]
		public void Serialization_CompositeObject()
		{
			var id = new Guid("5080EE19-FA4B-4A66-B104-0D5B4AA76DE2");
			var compositeObjectTypeId = new Guid("B6B586E1-8D7A-46EF-B5F3-BE68A9B4D79C");
			var simpleObjectTypeId1 = new Guid("F73A4A18-C06F-4DCD-A521-FEA2D42B57F3");
			var simpleObjectTypeId2 = new Guid("3DAE0268-F85B-47BA-9A0E-33EFED9ADAB2");
			var userId = new Guid("AD8C3E44-3A3E-452E-ACEC-8E224C4702D5");
			var changeSetId = new Guid("65ABF599-10D1-41DD-90B4-EAB1B0691881");

			var stringAttribute = new AttributeDefinition("StringAttribute", typeof(string), new StringSerializer());
			const string stringAttributeValue = "TEST";

			var int32Attribute = new AttributeDefinition("Int32Attribute", typeof(Int32), new Int32Serializer());
			const int int32AttributeValue = Int32.MaxValue;

			var compositeObjectDefinition = new CompositeObjectDefinition(compositeObjectTypeId, new[] { stringAttribute, int32Attribute });
			var simpleObjectDefinition1 = new SimpleObjectDefinition(simpleObjectTypeId1, new[] { stringAttribute, int32Attribute });
			var simpleObjectDefinition2 = new SimpleObjectDefinition(simpleObjectTypeId2, new[] { int32Attribute, stringAttribute });

			var objectDefinitionProvider = RegisterStrictMock<IBusinessObjectDefinitionProvider>();
			objectDefinitionProvider
				.Expect(e => e.GetCompositeObjectDefinition(compositeObjectTypeId))
				.Return(compositeObjectDefinition);
			objectDefinitionProvider
				.Expect(e => e.GetCompositeObjectDefinition(compositeObjectTypeId))
				.Return(compositeObjectDefinition);
			objectDefinitionProvider
				.Expect(e => e.FindSimpleObjectDefinition(compositeObjectTypeId, simpleObjectTypeId1)).Return(simpleObjectDefinition1);
			objectDefinitionProvider
				.Expect(e => e.FindSimpleObjectDefinition(compositeObjectTypeId, simpleObjectTypeId2))
				.Return(simpleObjectDefinition2);
			objectDefinitionProvider
				.Expect(e => e.FindSimpleObjectDefinition(compositeObjectTypeId, simpleObjectTypeId1)).Return(simpleObjectDefinition1);
			objectDefinitionProvider
				.Expect(e => e.FindSimpleObjectDefinition(compositeObjectTypeId, simpleObjectTypeId2))
				.Return(simpleObjectDefinition2);

			var businessObjectDefinitionProvider = Resolve<IBusinessObjectDefinitionProvider>();

			var attributeValues = new Dictionary<AttributeDefinition, object>
			{
				{ stringAttribute, stringAttributeValue },
				{ int32Attribute, int32AttributeValue }
			};

			var simpleObjectId1 = new Guid("5F6EE049-8308-4DB2-9689-5327C0584B6A");
			var simpleObjectAttributes1 = new Dictionary<AttributeDefinition, object>
			{
				{ stringAttribute, stringAttributeValue },
				{ int32Attribute, int32AttributeValue }
			};

			var simpleObject1 = new SimpleObject(simpleObjectId1, null, simpleObjectAttributes1, simpleObjectDefinition1, userId, new DateTimeOffset(), changeSetId);

			var simpleObjectId2 = new Guid("670FEED5-726F-43F8-896F-0F1C598F7A0F");
			var simpleObjectAttributes2 = new Dictionary<AttributeDefinition, object>
			{
				{ int32Attribute, int32AttributeValue },
				{ stringAttribute, stringAttributeValue }
			};
			var simpleObject2 = new SimpleObject(simpleObjectId2, simpleObjectId1, simpleObjectAttributes2, simpleObjectDefinition2, userId, new DateTimeOffset(), changeSetId);

			var simpleObjects = new[] { simpleObject1, simpleObject2 };
			var compositeObject = new CompositeObject(id, compositeObjectTypeId, new DateTimeOffset(), Guid.NewGuid(), businessObjectDefinitionProvider,
				attributeValues, Enumerable.Empty<Guid>(), simpleObjects, userId, userId, new DateTimeOffset());

			var serializedObject = compositeObject.Serialize();

			var deserializedObject = new CompositeObject(serializedObject, businessObjectDefinitionProvider);

			Assert.AreEqual(compositeObject.Id, deserializedObject.Id);
			Assert.AreEqual(compositeObject.CompositeObjectTypeId, deserializedObject.CompositeObjectTypeId);
			Assert.AreEqual(compositeObject.LastModified, deserializedObject.LastModified);
			Assert.AreEqual(compositeObject.Revision, deserializedObject.Revision);
			Assert.AreEqual(compositeObject.Attributes.Count, deserializedObject.Attributes.Count);
			Assert.IsTrue(deserializedObject.Attributes.ContainsKey(stringAttribute));
			Assert.AreEqual(compositeObject.Attributes[stringAttribute], deserializedObject.Attributes[stringAttribute]);

			Assert.IsTrue(deserializedObject.Attributes.ContainsKey(int32Attribute));
			Assert.AreEqual(compositeObject.Attributes[int32Attribute], deserializedObject.Attributes[int32Attribute]);

			Assert.AreEqual(2, deserializedObject.SimpleObjects.Count);

			Assert.IsTrue(deserializedObject.SimpleObjects.ContainsKey(simpleObjectId1));
			var deserializedSimpleObject1 = deserializedObject.SimpleObjects[simpleObjectId1];
			Assert.IsTrue(deserializedSimpleObject1.Attributes.ContainsKey(stringAttribute));
			Assert.AreEqual(simpleObject1.Attributes[stringAttribute], deserializedSimpleObject1.Attributes[stringAttribute]);
			Assert.IsTrue(deserializedSimpleObject1.Attributes.ContainsKey(int32Attribute));
			Assert.AreEqual(simpleObject1.Attributes[int32Attribute], deserializedSimpleObject1.Attributes[int32Attribute]);

			Assert.IsTrue(deserializedObject.SimpleObjects.ContainsKey(simpleObjectId2));
			var deserializedSimpleObject2 = deserializedObject.SimpleObjects[simpleObjectId2];
			Assert.IsTrue(deserializedSimpleObject2.Attributes.ContainsKey(stringAttribute));
			Assert.AreEqual(simpleObject2.Attributes[stringAttribute], deserializedSimpleObject2.Attributes[stringAttribute]);
			Assert.IsTrue(deserializedSimpleObject2.Attributes.ContainsKey(int32Attribute));
			Assert.AreEqual(simpleObject2.Attributes[int32Attribute], deserializedSimpleObject2.Attributes[int32Attribute]);
		}

		[Test]
		public void Serialization_SimpleObject()
		{
			var id = new Guid("452EED4D-F32D-4A2A-8509-3555F860DACB");
			var compositeObjectTypeId = new Guid(CompositeObjectTestData.StaticCompositeObjectTypeId);
			var simpleObjectTypeId = new Guid(CompositeObjectTestData.StaticSimpleObjectTypeId);
			Guid? parentId = new Guid("3D73633B-2ABF-4210-9227-0B2FA626E6E6");
			var userId = new Guid("35BC092C-F443-4E41-A199-A89475C496A6");
			var changeSetId = new Guid("65ABF599-10D1-41DD-90B4-EAB1B0691881");
			
			var stringAttribute = new AttributeDefinition("StringAttribute", typeof(string), new StringSerializer());
			const string stringAttributeValue = "TEST";

			var int32Attribute = new AttributeDefinition("Int32Attribute", typeof(Int32), new Int32Serializer());
			const int int32AttributeValue = Int32.MaxValue;

			var simpleObjectAttributes = new Dictionary<AttributeDefinition, object>
			{
				{ stringAttribute, stringAttributeValue },
				{ int32Attribute, int32AttributeValue }
			};

			var simpleObjectDefinition = new SimpleObjectDefinition(simpleObjectTypeId, new[] { stringAttribute, int32Attribute });

			RegisterStrictMock<IBusinessObjectDefinitionProvider>()
				.Expect(e => e.FindSimpleObjectDefinition(compositeObjectTypeId, simpleObjectTypeId))
				.Return(simpleObjectDefinition);

			var simpleObject = new SimpleObject(id, parentId, simpleObjectAttributes, simpleObjectDefinition, userId, new DateTimeOffset(), changeSetId);

			var serializedObject = simpleObject.Serialize();

			var businessObjectDefinitionProvider = Resolve<IBusinessObjectDefinitionProvider>();

			var deserializedObject = new SimpleObject(serializedObject, businessObjectDefinitionProvider, compositeObjectTypeId);

			Assert.AreEqual(simpleObject.Id, deserializedObject.Id);
			Assert.AreEqual(simpleObject.SimpleObjectTypeId, deserializedObject.SimpleObjectTypeId);
			Assert.AreEqual(simpleObject.ParentId, deserializedObject.ParentId);
			Assert.AreEqual(simpleObject.Revision, deserializedObject.Revision);
			Assert.AreEqual(simpleObject.Attributes.Count, deserializedObject.Attributes.Count);

			Assert.IsTrue(deserializedObject.Attributes.ContainsKey(stringAttribute));
			Assert.AreEqual(simpleObject.Attributes[stringAttribute], deserializedObject.Attributes[stringAttribute]);

			Assert.IsTrue(deserializedObject.Attributes.ContainsKey(int32Attribute));
			Assert.AreEqual(simpleObject.Attributes[int32Attribute], deserializedObject.Attributes[int32Attribute]);
		}
	}
}