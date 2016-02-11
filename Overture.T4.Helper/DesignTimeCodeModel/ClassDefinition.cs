namespace Overture.T4.Helper.DesignTimeCodeModel
{
	public class ClassDefinition
	{
		public ClassDefinition(string name, string originalFullName, string source, PropertyDefinition[] properties, AttributeDefinition[] attributes, MethodDefinition[] methods)
		{
			Name = name;
			OriginalFullName = originalFullName;
			Source = source;
			Properties = properties;
			Attributes = attributes;
			Methods = methods;
		}

		public string Name { get; private set; }
		public string OriginalFullName { get; private set; }
		public string Source { get; private set; }
		public PropertyDefinition[] Properties { get; private set; }
		public MethodDefinition[] Methods { get; private set; }
		public AttributeDefinition[] Attributes { get; private set; }
	}
}