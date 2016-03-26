namespace Overture.T4.Helper.TypeScriptClientModels
{
	public class TypeScriptClientModelProperty
	{
		public TypeScriptClientModelProperty(string name, string originalName, TypeScriptClientModelPropertyType type)
		{
			Name = name;
			OriginalName = originalName;
			Type = type;
		}

		public string Name { get; private set; }
		public string OriginalName { get; private set; }
		public TypeScriptClientModelPropertyType Type { get; private set; }
	}
}