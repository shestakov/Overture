using System;
using System.Collections.Generic;
using System.Linq;

namespace Overture.ChangeSets.DefinitionProvider
{
	public class TypeRetriever : ITypeRetriever
	{
		public IEnumerable<Type> GetPublicTypesOfLoadedAssemblies()
		{
			return
				AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(assembly => assembly.GetExportedTypes());
		}
	}
}