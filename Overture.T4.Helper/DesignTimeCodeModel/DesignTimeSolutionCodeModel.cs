using System.Collections.Generic;

namespace Overture.T4.Helper.DesignTimeCodeModel
{
	public class DesignTimeSolutionCodeModel
	{
		public DesignTimeSolutionCodeModel(Dictionary<string, EnumDefinition> enums, Dictionary<string, ClassDefinition> classes)
		{
			Enums = enums;
			Classes = classes;
		}

		public Dictionary<string, EnumDefinition> Enums { get; private set; }
		public Dictionary<string, ClassDefinition> Classes { get; private set; }
	}
}