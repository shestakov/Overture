namespace Overture.T4.Helper.TypeScriptClientModels
{
	public class TypeScriptClientModelPropertyType
	{
		public TypeScriptClientModelPropertyType(string name, bool isArray, bool initWithDefaultConstructor, bool isStringDictionary)
		{
			Name = name;
			IsArray = isArray;
			InitWithDefaultConstructor = initWithDefaultConstructor;
			IsStringDictionary = isStringDictionary;
		}

		public string Name { get; private set; }
		public bool IsArray { get; private set; }
		public bool IsStringDictionary { get; private set; }
		public bool InitWithDefaultConstructor { get; private set; }
	}
}