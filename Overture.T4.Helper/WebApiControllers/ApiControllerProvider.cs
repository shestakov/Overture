using System.Linq;
using Overture.T4.Helper.DesignTimeCodeModel;

namespace Overture.T4.Helper.WebApiControllers
{
	public class ApiControllerProvider
	{
		private readonly DesignTimeSolutionCodeModelProvider helper;

		public ApiControllerProvider(DesignTimeSolutionCodeModelProvider helper)
		{
			this.helper = helper;
		}

		public ApiController[] GetControllers(string attributeTypeName)
		{
			var codeModel = helper.GetCodeModel();
			var classes = codeModel.Classes.Values;
			return classes
				.Where(c => IsApiController(c, attributeTypeName))
				.Select(
					c =>
						new ApiController(GetControllerName(c), c.Source,
							c.Methods
								.Where(IsApiMethod)
								.Select(m => new ApiAction(m.Name, GetActionMethod(m)))
								.ToArray()))
				.ToArray();
		}

		private static string GetControllerName(ClassDefinition classDefinition)
		{
			var className = classDefinition.Name;
			const string conventionKeyword = "Controller";
			return className.EndsWith(conventionKeyword)
				? className.Substring(0, className.Length - conventionKeyword.Length)
				: className;
		}

		private static string GetActionMethod(MethodDefinition methodDefinition)
		{
			if (methodDefinition.Attributes.Any(a => a.ClassName.EndsWith("HttpPostAttribute")))
				return "POST";
			if (methodDefinition.Attributes.Any(a => a.ClassName.EndsWith("HttpGetAttribute")))
				return "GET";
			return null;
		}

		private static bool IsApiMethod(MethodDefinition methodDefinition)
		{
			return
				methodDefinition.Attributes.Any(
					a => a.ClassName.EndsWith("HttpPostAttribute") || a.ClassName.EndsWith("HttpGetAttribute"));
		}

		private static bool IsApiController(ClassDefinition classDefinition, string attributeTypeName)
		{
			return classDefinition.Attributes.Any(a => a.ClassName.EndsWith(attributeTypeName));
		}
	}
}