using System;

namespace Overtute.Core.Web.Attributes
{
	public class AjaxControllerIncludeAttribute : Attribute
	{
		public Type[] ControllerTypes { get; private set; }

		public AjaxControllerIncludeAttribute(params Type[] controllerTypes)
		{
			/*if(controllerTypes.Any(type => !type.IsSubclassOf(typeof(Controller))))
				throw new ArgumentException("Only System.Web.Mvc.Controller descendants can be includede as AJAX controllers", "controllerTypes");*/
			ControllerTypes = controllerTypes;
		}
	}
}