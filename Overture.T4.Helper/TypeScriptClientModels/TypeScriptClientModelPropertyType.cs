namespace Overture.T4.Helper.TypeScriptClientModels
{
	public class TypeScriptClientModelPropertyType
	{
		public TypeScriptClientModelPropertyType(string name, bool isArray, bool initWithDefaultConstructor)
		{
			Name = name;
			IsArray = isArray;
			InitWithDefaultConstructor = initWithDefaultConstructor;
		}

		public string Name { get; private set; }
		public bool IsArray { get; private set; }
		public bool InitWithDefaultConstructor { get; private set; }
	}
}