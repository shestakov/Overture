using System;
using System.Linq;
using NUnit.Framework;
using Overture.ChangeSets.DefinitionProvider;
using Overture.ChangeSets.Tests.MockingNinja;
using Rhino.Mocks;

namespace Overture.ChangeSets.Tests.DefinitionProvider
{
	[TestFixture]
	public class BusinessObjectDefinitionProviderTest_CornerCases : TestWithContainer
	{
		private void RegisterMockTypeRetriever()
		{
			RegisterStrictMock<ITypeRetriever>()
				.Expect(e => e.GetPublicTypesOfLoadedAssemblies())
				.Return(Enumerable.Empty<Type>());
		}

		[Test]
		public void CompositeBusinessObject_Nonexistent()
		{
			RegisterMockTypeRetriever();
			Mocks.ReplayAll();
			var provider = Resolve<BusinessObjectDefinitionProvider>();
			var compositeObjectTypeId = Guid.NewGuid();
			Assert.Throws<ArgumentException>(() => provider.GetCompositeObjectDefinition(compositeObjectTypeId),
				$"CompositeObjectType {compositeObjectTypeId} not found");
			Mocks.VerifyAll();
		}

		[Test]
		public void SimpleBusinessObject_Nonexistent()
		{
			RegisterMockTypeRetriever();
			Mocks.ReplayAll();
			var provider = Resolve<BusinessObjectDefinitionProvider>();
			var compositeObjectTypeId = Guid.NewGuid();
			var simpleObjectTypeId = Guid.NewGuid();
			var definition = provider.FindSimpleObjectDefinition(compositeObjectTypeId, simpleObjectTypeId);
			Assert.IsNull(definition);
			Mocks.VerifyAll();
		}
	}
}