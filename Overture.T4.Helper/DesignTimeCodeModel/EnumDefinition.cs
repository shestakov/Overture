using System.Collections.Generic;

namespace Overture.T4.Helper.DesignTimeCodeModel
{
	public class EnumDefinition
	{
		public EnumDefinition(string name, string originalFullName, string source, IEnumerable<EnumMemberDefinition> members, AttributeDefinition[] attributes)
		{
			Name = name;
			OriginalFullName = originalFullName;
			Source = source;
			Members = members;
			Attributes = attributes;
		}

		public string Name { get; private set; }
		public string OriginalFullName { get; private set; }
		public string Source { get; private set; }
		public IEnumerable<EnumMemberDefinition> Members { get; private set; }
		public AttributeDefinition[] Attributes { get; private set; }
	}
}