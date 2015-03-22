namespace Overtute.Core.Web.JsonNet
{
	public class JsonNetGetResult : JsonNetResultBase
	{
		public JsonNetGetResult(object result, string contentType = "application/json")
			: base(result, true, contentType)
		{
		}
	}
}