using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Overture.T4.Helper.DesignTimeCodeModel
{
	[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
	public class DesignTimeSolutionCodeModelProvider : IDesignTimeSolutionCodeModelProvider
	{
		public DesignTimeSolutionCodeModel GetCodeModel()
		{
			return new DesignTimeSolutionCodeModel(enumDictionary, classDictionary);
		}

		private readonly Dictionary<string, EnumDefinition> enumDictionary = new Dictionary<string, EnumDefinition>();
		private readonly Dictionary<string, ClassDefinition> classDictionary;

		public DesignTimeSolutionCodeModelProvider(object host)
		{
			var dte = (DTE2) ((IServiceProvider) host).GetService(typeof (DTE));
			var codeClasses = new Dictionary<string, CodeClass>();

			foreach (var item in GetAllProjectItems(dte))
			{
				var code = item.FileCodeModel;
				if (code == null)
					continue;
				foreach (var e in EnumerateCodeElement(code))
				{
					switch (e.Kind)
					{
						case vsCMElement.vsCMElementClass:
							if(!codeClasses.ContainsKey(e.FullName)) //Partial classes cause key duplication exceptions
								codeClasses.Add(e.FullName, (CodeClass)e);
							break;

						case vsCMElement.vsCMElementEnum:
							if (!enumDictionary.ContainsKey(e.FullName)) //Partial classes cause key duplication exceptions
								enumDictionary.Add(e.FullName, new EnumDefinition(
									e.Name,
									e.FullName,
									e.GetCodeElementSourceFileNameAndLineNumber(),
									e.GetEnumMembers(),
									e.GetAttributes()));
							break;
					}
				}
			}
			classDictionary = GetClassDefinitions(codeClasses).ToDictionary(c => c.OriginalFullName, c => c);
		}

		private static IEnumerable<ClassDefinition> GetClassDefinitions(Dictionary<string, CodeClass> classes)
		{
			return classes.Values
				.Select(
					c =>
						new ClassDefinition(
							c.Name,
							c.FullName,
							c.GetClassSourceFileNameAndLineNumber(),
							GetHierarchy(classes, c).Union(new[] {c}).GetProperties(),
							c.GetAttributes(),
							c.GetMethods()));
		}
		
		private static IEnumerable<CodeClass> GetHierarchy(IReadOnlyDictionary<string, CodeClass> codeClasses, CodeClass c)
		{
			var parents = c.Bases
				.OfType<CodeClass>()
				.Where(x => codeClasses.ContainsKey(x.FullName)) // only classes declared in this solution
				.ToArray();
				
			var hierarchy = parents.SelectMany(c2  => GetHierarchy(codeClasses, c2));
			return parents.Union(hierarchy);
		}
		
		private static IEnumerable<CodeElement> EnumerateCodeElement(FileCodeModel code)
		{
			var codeElements = code.CodeElements;
			return codeElements.Cast<CodeElement>().SelectMany(EnumerateCodeElement);
		}

		private static IEnumerable<CodeElement> EnumerateCodeElement(CodeElement element)
		{
			yield return element;
			foreach (var sub2 in element.Children.Cast<CodeElement>().SelectMany(EnumerateCodeElement))
				yield return sub2;
		}
		
		private static IEnumerable<ProjectItem> GetAllProjectItems(_DTE dte)
		{
			var projects =
				dte.Solution.Projects
					.OfType<Project>()
					.Where(p => p.Kind != Constants.vsProjectKindUnmodeled && p.FullName.EndsWith(".csproj"))
					.ToArray();

			return projects.SelectMany(project => project.ProjectItems.OfType<ProjectItem>().SelectMany(EnumerateProjectItem));
		}

		private IEnumerable<Project> GetProjects(Project project)
		{
			if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
			{
				return project.ProjectItems
					.Cast<ProjectItem>()
					.Select(x => x.SubProject)
					.Where(x => x != null)
					.SelectMany(GetProjects);
			}
			return new[] { project };
		}

		private static IEnumerable<ProjectItem> EnumerateProjectItem(ProjectItem p)
		{
			yield return p;
			foreach (var sub2 in p.ProjectItems.OfType<ProjectItem>().SelectMany(EnumerateProjectItem))
				yield return sub2;
		}
	}
}