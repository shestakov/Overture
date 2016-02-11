namespace Overture.T4.Helper.DesignTimeCodeModel
{
	public class AttributeDefinition
	{
		public AttributeDefinition(string className)
		{
			ClassName = className;
		}

		public string ClassName { get; private set; }
	}
}