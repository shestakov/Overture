namespace Overture.T4.Helper.DesignTimeCodeModel
{
	public abstract class MemeberDefinition
	{
		protected MemeberDefinition(string name, AttributeDefinition[] attributes)
		{
			Name = name;
			Attributes = attributes;
		}

		public string Name { get; private set; }
		public AttributeDefinition[] Attributes { get; private set; }
	}
}