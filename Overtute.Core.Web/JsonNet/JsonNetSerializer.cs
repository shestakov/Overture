using System.Web.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Overtute.Core.Web.JsonNet
{
	public class JsonNetSerializer : IJsonSerializer
	{
		public string Serialize(object model)
		{
			var serializerSettings = new JsonSerializerSettings
			{
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DefaultValueHandling = DefaultValueHandling.Include,
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			//serializerSettings.Converters.Add(new StringEnumConverter());
			
			var serializedObject = JsonConvert.SerializeObject(model, Formatting.None, serializerSettings);
			return serializedObject;
		}

		public string SerializeEncoded(object model)
		{
			return Json.Encode(Serialize(model));
		}

		public T Deserialize<T>(string json)
		{
			if (string.IsNullOrEmpty(json))
				return default(T);

			var serializerSettings = new JsonSerializerSettings
			{
				DateTimeZoneHandling = DateTimeZoneHandling.Utc,
				NullValueHandling = NullValueHandling.Include,
			};
	
			return JsonConvert.DeserializeObject<T>(json, serializerSettings);
		}
	}
}