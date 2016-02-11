namespace Overture.T4.Helper.TypeScriptClientModels
{
	public class TypeScriptClientModelProperty
	{
		public TypeScriptClientModelProperty(string name, TypeScriptClientModelPropertyType type)
		{
			Name = name;
			Type = type;
		}

		public string Name { get; private set; }
		public TypeScriptClientModelPropertyType Type { get; private set; }
	}
}