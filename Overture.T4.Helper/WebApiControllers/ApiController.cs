namespace Overture.T4.Helper.WebApiControllers
{
	public class ApiController
	{
		public ApiController(string controllerName, string source, ApiAction[] actions)
		{
			ControllerName = controllerName;
			Source = source;
			Actions = actions;
		}

		public string ControllerName { get; private set; }
		public string Source { get; private set; }
		public ApiAction[] Actions { get; private set; }
	}
}