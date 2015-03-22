using System;
using System.Collections.Generic;
using Overture.ChangeSets.DefinitionProvider;

namespace Overture.ChangeSets.Tests.Infrastructure
{
	internal class TestTypeRetriever : ITypeRetriever
	{
		public IEnumerable<Type> GetPublicTypesOfLoadedAssemblies()
		{
			return new[]
			{
				typeof(TestCompositeBusinessObjectConrete),
				typeof(TestCompositeBusinessObjectStatic),
				typeof(TestSimpleBusinessObjectConcrete),
				typeof(TestSimpleBusinessObjectStatic)
			};
		}
	}
}