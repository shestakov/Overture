using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EnvDTE;

namespace Overture.T4.Helper.DesignTimeCodeModel
{
	[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
	[SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
	public static class DesignTimeHelper
	{
		public static AttributeDefinition[] GetAttributes(this CodeClass codeElement)
		{
			return GetAttributes((CodeElement)codeElement);
		}

		public static AttributeDefinition[] GetAttributes(this CodeElement codeElement)
		{
			return codeElement.Children.Cast<CodeElement>()
				.Where(e => e.Kind == vsCMElement.vsCMElementAttribute)
				.Cast<CodeAttribute>()
				.Select(e => new AttributeDefinition(e.FullName))
				.ToArray();
		}

		public static MethodDefinition[] GetMethods(this CodeClass codeClass)
		{
			var methods = codeClass.Children
				.Cast<CodeElement>()
				.Where(m => m.Kind == vsCMElement.vsCMElementFunction)
				.Cast<CodeFunction>()
				.Where(f => !f.IsShared && (f.Access & vsCMAccess.vsCMAccessPublic) != 0)
				.Cast<CodeElement>();

			return methods
				.Select(m => new MethodDefinition(m.Name, m.GetAttributes()))
				.ToArray();
		}

		public static string GetClassSourceFileNameAndLineNumber(this CodeClass c)
		{
			return String.Format("{0}: {1}", c.ProjectItem.FileNames[0], c.StartPoint.Line);
		}

		public static string GetCodeElementSourceFileNameAndLineNumber(this CodeElement codeElement)
		{
			return String.Format("{0}: {1}", codeElement.ProjectItem.FileNames[0], codeElement.StartPoint.Line);
		}

		public static PropertyDefinition[] GetProperties(this IEnumerable<CodeClass> hierarchy)
		{
			return hierarchy
				.SelectMany(
					codeClass => codeClass.Children
						.Cast<CodeElement>()
						.Where(e => e.Kind == vsCMElement.vsCMElementProperty)
						.Cast<CodeProperty>()
						.Where(e => (e.Access & vsCMAccess.vsCMAccessPublic) != 0))
				.Cast<CodeElement>()
				.Select(p => new PropertyDefinition(p.Name, p.GetPropertyTypeName(), p.GetAttributes()))
				.ToArray();
		}

		public static IEnumerable<EnumMemberDefinition> GetEnumMembers(this CodeElement codeElement)
		{
			return codeElement.Children
				.Cast<CodeElement>()
				.Where(c => c.Kind == vsCMElement.vsCMElementVariable)
				.Select(c => new EnumMemberDefinition(c.GetText()));
		}

		private static string GetPropertyTypeName(this CodeElement codeElement)
		{
			var codeProperty = (CodeProperty)codeElement;
			return codeProperty.Type.AsString;
		}

		private static string GetText(this CodeElement e)
		{
			var edit = e.StartPoint.CreateEditPoint();
			return edit.GetText(e.EndPoint);
		}
	}
}