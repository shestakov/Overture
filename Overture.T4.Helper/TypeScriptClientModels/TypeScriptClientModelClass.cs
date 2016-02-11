using System.Collections.Generic;

namespace Overture.T4.Helper.TypeScriptClientModels
{
	public class TypeScriptClientModelClass
	{
		public TypeScriptClientModelClass(string name, string originalFullName, string source, IEnumerable<TypeScriptClientModelProperty> members)
		{
			Name = name;
			OriginalFullName = originalFullName;
			Source = source;
			Members = members;
		}

		public string Name { get; private set; }
		public string OriginalFullName { get; private set; }
		public string Source { get; private set; }
		public IEnumerable<TypeScriptClientModelProperty> Members { get; private set; }
	}
}