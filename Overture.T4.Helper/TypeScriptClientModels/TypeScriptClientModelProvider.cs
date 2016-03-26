using System.Collections.Generic;
using System.Linq;
using Overture.T4.Helper.DesignTimeCodeModel;

namespace Overture.T4.Helper.TypeScriptClientModels
{
	public class TypeScriptClientModelProvider
	{
		public TypeScriptClientModelClass[] GetClientModelClasses(string attributeTypeName)
		{
			return classes.Values
				.Where(c => IsClientModel(c, attributeTypeName))
				.Select(
					c =>
						new TypeScriptClientModelClass(ShortName(c.OriginalFullName), c.OriginalFullName, c.Source,
							c.Properties.Select(
								p => new TypeScriptClientModelProperty(GetTypeScriptPropertyName(p), GetOriginalPropertyName(p), GetMemberTypeScriptType(p)))))
				.ToArray();
		}

		public TypeScriptClientModelEnum[] GetClientModelEnums(string attributeTypeName)
		{
			return enums.Values
				.Where(c => IsClientModel(c, attributeTypeName))
				.Select(
					e =>
						new TypeScriptClientModelEnum(e.Name, e.OriginalFullName, e.Source,
							e.Members.Select(m => new TypeScriptClientModelEnumMember(m.Text)).ToArray()))
				.ToArray();
		}

		private readonly Dictionary<string, EnumDefinition> enums;
		private readonly Dictionary<string, ClassDefinition> classes;

		public TypeScriptClientModelProvider(DesignTimeSolutionCodeModelProvider helper)
		{
			var codeModel = helper.GetCodeModel();
			enums = codeModel.Enums;
			classes = codeModel.Classes;
		}

		private string GetTypeScriptTypeName(string normalizedType)
		{
			if (numberTypes.Contains(normalizedType))
				return "number";

			if (stringTypes.Contains(normalizedType))
				return "string";

			if (normalizedType == "bool")
				return "boolean";

			if (enums.ContainsKey(normalizedType))
				return ShortName(normalizedType);

			if (classes.ContainsKey(normalizedType))
				return ShortName(normalizedType);

			return string.Format("/** {0} **/ any", normalizedType);
		}

		private static string GetTypeScriptPropertyName(PropertyDefinition property)
		{
			var name = property.Name;
			var result = name.StartsWith("@") ? name.Substring(1) : name;
			return result.Substring(0, 1).ToLowerInvariant() + result.Substring(1);
		}

		private static string GetOriginalPropertyName(PropertyDefinition property)
		{
			var name = property.Name;
			var result = name.StartsWith("@") ? name.Substring(1) : name;
			return result;
		}

		private static bool IsArray(string type)
		{
			return type.StartsWith("System.Collections.Generic.IEnumerable<") ||
				   type.StartsWith("System.Collections.Generic.List<") ||
				   type.StartsWith("System.Collections.Generic.IList<") ||
				   type.EndsWith("[]");

		}

		private static bool IsStringDictionary(string type)
		{
			return type.StartsWith("System.Collections.Generic.Dictionary<string, ");
		}

		private readonly string[] numberTypes = { "long", "int", "short", "decimal", "float", "double" };

		private readonly string[] stringTypes = { "string", "System.Guid" };
		
		private bool IsClientModel(ClassDefinition classDefinition, string attributeTypeName)
		{
			return classDefinition.Attributes.Any(a => a.ClassName.EndsWith(attributeTypeName));
		}

		private bool IsClientModel(EnumDefinition classDefinition, string attributeTypeName)
		{
			return classDefinition.Attributes.Any(a => a.ClassName.EndsWith(attributeTypeName));
		}

		private TypeScriptClientModelPropertyType GetMemberTypeScriptType(PropertyDefinition codeProperty)
		{
			var normalizedType = SimplifyType(codeProperty.TypeName);

			if (numberTypes.Contains(normalizedType))
				return new TypeScriptClientModelPropertyType("number", false, false, false);

			if (stringTypes.Contains(normalizedType))
				return new TypeScriptClientModelPropertyType("string", false, false, false);

			if (normalizedType == "bool")
				return new TypeScriptClientModelPropertyType("boolean", false, false, false);

			if (enums.ContainsKey(normalizedType))
				return new TypeScriptClientModelPropertyType(ShortName(normalizedType), false, false, false);

			if (classes.ContainsKey(normalizedType))
				return new TypeScriptClientModelPropertyType(ShortName(normalizedType), false, true, false);

			if (IsArray(normalizedType))
				return new TypeScriptClientModelPropertyType(GetTypeScriptTypeName(GetArrayBaseType(normalizedType)), true, false, false);

			if (IsStringDictionary(normalizedType))
				return new TypeScriptClientModelPropertyType(GetTypeScriptTypeName(GetStringDictionaryType(normalizedType)), false, false, true);

			return new TypeScriptClientModelPropertyType(string.Format("/** {0} **/ any", normalizedType), false, false, false);
		}

		private static string ShortName(string type)
		{
			int di = type.LastIndexOf('.');
			if (di > -1)
				type = type.Substring(di + 1);
			return type;
		}

		private static string SimplifyType(string type)
		{
			return type.EndsWith("?") ? type.Substring(0, type.Length - 1) : type;
		}

		private string GetStringDictionaryType(string type)
		{
			if (type.StartsWith("System.Collections.Generic.Dictionary<string, "))
			{
				var l = "System.Collections.Generic.Dictionary<string, ".Length;
				return type.Substring(l, type.Length - l - 1);
			}

			return null;
		}


		private string GetArrayBaseType(string type)
		{
			if (type == "System.Collections.IEnumerable")
				return "any";

			if (type.StartsWith("System.Collections.Generic.IEnumerable<"))
			{
				var l = "System.Collections.Generic.IEnumerable<".Length;
				return type.Substring(l, type.Length - l - 1);
			}

			if (type.StartsWith("System.Collections.Generic.List<"))
			{
				var l = "System.Collections.Generic.List<".Length;
				return type.Substring(l, type.Length - l - 1);
			}

			if (type.StartsWith("System.Collections.Generic.IList<"))
			{
				var l = "System.Collections.Generic.IList<".Length;
				return type.Substring(l, type.Length - l - 1);
			}

			if (type.EndsWith("[]"))
			{
				return type.Substring(0, type.Length - 2);
			}

			return null;
		}
	}
}