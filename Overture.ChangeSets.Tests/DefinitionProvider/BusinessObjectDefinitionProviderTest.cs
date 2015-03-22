using NUnit.Framework;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.SerializerProvider;
using Overture.ChangeSets.Tests.Infrastructure;
using Overture.ChangeSets.Tests.MockingNinja;
using Rhino.Mocks;

namespace Overture.ChangeSets.Tests.DefinitionProvider
{
	[TestFixture(typeof(TestCompositeBusinessObjectConrete))]
	[TestFixture(typeof(TestCompositeBusinessObjectStatic))]
	public class BusinessObjectDefinitionProviderTest_CompositeObjects<T> : TestWithContainer
	{
		[Test]
		public void Test()
		{
			RegisterStrictMock<ITypeRetriever>()
				.Expect(e => e.GetPublicTypesOfLoadedAssemblies())
				.Return(new[] { typeof(T) });

			CompositeObjectTestData.SetupMockAttributeValueSerializerProvider(RegisterStrictMock<IAttributeValueSerializerProvider>());

			Mocks.ReplayAll();

			var provider = Resolve<BusinessObjectDefinitionProvider>();

			var compositeObjectTypeId = CompositeObjectTestData.ClassBinding[typeof(T)];
			var definition = provider.GetCompositeObjectDefinition(compositeObjectTypeId);
			Assert.AreEqual(compositeObjectTypeId, definition.CompositeObjectTypeId);
			var attributes = definition.Attributes;

			Assert.AreEqual(CompositeObjectTestData.TestAttributes.Count, attributes.Count);
			foreach(var testAttribute in CompositeObjectTestData.TestAttributes)
			{
				var attribute = attributes[testAttribute.Value.Name];
				Assert.AreEqual(testAttribute.Value.Name, attribute.Name);
				Assert.AreEqual(testAttribute.Key, attribute.AttributeType);
				Assert.IsInstanceOf(testAttribute.Value.SerializerType, attribute.Serializer);
			}

			Mocks.VerifyAll();
		}
	}
}