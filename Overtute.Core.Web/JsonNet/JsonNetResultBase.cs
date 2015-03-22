using System;
using System.Text;
using System.Web.Mvc;

namespace Overtute.Core.Web.JsonNet
{
	public abstract class JsonNetResultBase : ActionResult
	{
		private readonly object result;
		private readonly IJsonSerializer serializer;
		private readonly bool allowGet;
		private readonly string contentType;

		protected JsonNetResultBase(Object result, bool allowGet, string contentType)
		{
			serializer = new JsonNetSerializer();
			this.result = result;
			this.allowGet = allowGet;
			this.contentType = contentType;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			if (!allowGet && String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("GET method not allowed");
			}

			var response = context.HttpContext.Response;
			response.ContentType = contentType;
			response.ContentEncoding = Encoding.UTF8;

			if (result != null)
			{
				response.Write(serializer.Serialize(result));
			}
		}
	}
}