namespace Overture.T4.Helper.DesignTimeCodeModel
{
	public class EnumMemberDefinition
	{
		public EnumMemberDefinition(string text)
		{
			Text = text;
		}

		public string Text { get; private set; }
	}
}