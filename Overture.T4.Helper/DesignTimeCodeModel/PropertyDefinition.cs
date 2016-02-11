namespace Overture.T4.Helper.DesignTimeCodeModel
{
	public class PropertyDefinition : MemeberDefinition
	{
		public PropertyDefinition(string name, string typeName, AttributeDefinition[] attributes) : base(name, attributes)
		{
			TypeName = typeName;
		}

		public string TypeName { get; private set; }
	}
}