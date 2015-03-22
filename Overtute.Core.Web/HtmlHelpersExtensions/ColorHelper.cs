namespace Overtute.Core.Web.HtmlHelpersExtensions
{
	public static class ColorHelper
	{
		public static string FormatColorAsRgbHex(this int color)
		{
			return string.Format("#{0:x6}", color);
		}
	}
}