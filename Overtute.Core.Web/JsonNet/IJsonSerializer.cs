namespace Overtute.Core.Web.JsonNet
{
	public interface IJsonSerializer
	{
		string Serialize(object model);
		string SerializeEncoded(object model);
		T Deserialize<T>(string json);
	}
}