namespace Overtute.Core.Web.JsonNet
{
	public class
		JsonNetPostResult : JsonNetResultBase
	{
		public JsonNetPostResult(object result)
			: base(result, false, "application/json")
		{
		}

		public JsonNetPostResult()
			: base(new {}, false, "application/json")
		{
		}

		public JsonNetPostResult(object result, string contentType = "application/json")
			: base(result, false, contentType)
		{
		}
	}
}