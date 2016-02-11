namespace Overture.T4.Helper.WebApiControllers
{
	public class ApiAction
	{
		public ApiAction(string name, string method)
		{
			Name = name;
			Method = method;
		}

		public string Name { get; private set; }
		public string Method { get; private set; }
	}
}