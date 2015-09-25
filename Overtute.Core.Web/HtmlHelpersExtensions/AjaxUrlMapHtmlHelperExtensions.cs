using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Overtute.Core.Web.Attributes;

namespace Overtute.Core.Web.HtmlHelpersExtensions
{
	public static class AjaxUrlMapHtmlHelperExtensions
	{
		public static MvcHtmlString RenderAjaxUrlMap(this HtmlHelper html, UrlHelper urlHelper, ControllerBase controller)
		{
			var includeAttrbiutes =
				GetIncludeAttributes<AjaxControllerIncludeAttribute>(controller.GetType());

			var includedControllers =
				includeAttrbiutes.SelectMany(a => a.ControllerTypes)
					.Union(new[] { controller.GetType() });

			var builder = new StringBuilder();
			builder.AppendLine("AjaxUrlMap = {};");

			foreach (var controllerDescriptor in includedControllers)
			{
				var controllerName = controllerDescriptor.Name.Substring(0, controllerDescriptor.Name.Length - "Controller".Length);
				var actions = controllerDescriptor.GetMethods().Where(a => GetActionMethod(a) != "").Select(a =>
				{
					var actionName = a.Name;
					return new
					{
						Name = actionName,
						Url = urlHelper.Action(actionName, controllerName),
						Method = GetActionMethod(a)
					};
				}).ToList();

				builder.AppendLine(string.Format("\t\tAjaxUrlMap.{0} = {{", controllerName));
				foreach (var action in actions)
				{
					builder.AppendLine(string.Format("\t\t\t{0}: '{1}',", action.Name, action.Url));
				}
				builder.AppendLine("\t\t}");
			}

			return new MvcHtmlString(builder.ToString());
		}

		private static string GetActionMethod(ICustomAttributeProvider a)
		{
			var post = a.GetCustomAttributes(typeof (HttpPostAttribute), true).Any();
			var get = a.GetCustomAttributes(typeof (HttpGetAttribute), true).Any();
			return post ? "POST" : get ? "GET" : "";
		}

		private static IEnumerable<T> GetIncludeAttributes<T>(Type type) where T: Attribute
		{
			var attributeType = typeof (AjaxControllerIncludeAttribute);
			var currentType = type;
			while (currentType != null)
			{
				var attributes = Attribute.GetCustomAttributes(currentType, attributeType, false);
				foreach (var attribute in attributes)
				{
					yield return (T)attribute;
				}

				currentType = currentType.BaseType;
			}
		}
	}
}