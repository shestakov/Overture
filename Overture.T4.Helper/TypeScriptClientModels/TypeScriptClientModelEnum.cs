namespace Overture.T4.Helper.TypeScriptClientModels
{
	public class TypeScriptClientModelEnum
	{
		public TypeScriptClientModelEnum(string name, string originalFullName, string source, TypeScriptClientModelEnumMember[] members)
		{
			Name = name;
			OriginalFullName = originalFullName;
			Source = source;
			Members = members;
		}

		public string Name { get; private set; }
		public string OriginalFullName { get; private set; }
		public string Source { get; private set; }
		public TypeScriptClientModelEnumMember[] Members { get; private set; }
	}
}