using System;
using System.Collections.Generic;

namespace Overture.ChangeSets.DefinitionProvider
{
	public interface ITypeRetriever
	{
		IEnumerable<Type> GetPublicTypesOfLoadedAssemblies();
	}
}