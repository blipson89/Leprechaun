using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Configy.Containers;
using Leprechaun.Extensions;
using Leprechaun.Filters.Exclusions;
using Leprechaun.Model;

namespace Leprechaun.Filters
{
	public abstract class BaseTemplatePredicate : ITemplatePredicate
	{
		protected readonly IList<ITemplateTreeRoot> _includeEntries;
		protected readonly IContainer _configuration;
		protected readonly string _rootNamespace;

		protected BaseTemplatePredicate(XmlNode configNode, IContainer configuration, string rootNamespace)
		{
			Assert.ArgumentNotNull(configNode, nameof(configNode));
			Assert.ArgumentNotNull(rootNamespace, nameof(rootNamespace));

			_configuration = configuration;
			_rootNamespace = rootNamespace;

			_includeEntries = ParsePreset(configNode);
			EnsureEntriesExist(configuration?.Name ?? "Unknown");
		}
		/// <summary>
		/// Checks if a preset includes a given item
		/// </summary>
		protected virtual bool Includes(ITemplateTreeRoot entry, TemplateInfo itemData)
		{
			// check for path match
			var unescapedPath = entry.Path.Replace(@"\*", "*");
			if (!itemData.Path.StartsWith(unescapedPath + "/", StringComparison.OrdinalIgnoreCase) && !itemData.Path.Equals(unescapedPath, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			// check excludes
			return ExcludeMatches(entry, itemData);
		}
		protected virtual bool ExcludeMatches(ITemplateTreeRoot entry, TemplateInfo itemData)
		{
			foreach (var exclude in entry.Exclusions)
			{
				var result = exclude.Evaluate(itemData.Path);

				if (!result) return false;
			}

			return true;
		}
		protected IList<ITemplateTreeRoot> ParsePreset(XmlNode configuration)
		{

			List<ITemplateTreeRoot> presets = configuration.ChildNodes
				.Cast<XmlNode>()
				.Where(node => node.Name == "include")
				.Select(CreateIncludeEntry)
				.ToList();

			var names = new HashSet<string>();
			foreach (ITemplateTreeRoot preset in presets)
			{
				if (!names.Contains(preset.Name))
				{
					names.Add(preset.Name);
					continue;
				}

				throw new InvalidOperationException($"Multiple predicate include nodes had the same name '{preset.Name}'. This is not allowed. Note that this can occur if you did not specify the name attribute and two include entries end in an item with the same name. Use the name attribute on the include tag to give a unique name.");
			}

			return presets;
		}

		public virtual bool Includes(TemplateInfo template)
		{
			Assert.ArgumentNotNull(template, nameof(template));

			var result = true;

			foreach (var entry in _includeEntries)
			{
				result = Includes(entry, template);

				if (result)
				{
					return true;
				}
			}

			return result;
		}

		/// <param name="template">Template to get a root NS for. MAY BE NULL, in which case a general root NS should be returned.</param>
		public virtual string GetRootNamespace(TemplateInfo template)
		{
			return _rootNamespace;
		}


		protected virtual ITemplateTreeRoot CreateIncludeEntry(XmlNode configuration)
		{
			string path = configuration.GetExpectedAttribute("path");

			// ReSharper disable once PossibleNullReferenceException
			XmlAttribute name = configuration.Attributes["name"];
			string nameValue = name == null ? path.Substring(path.LastIndexOf('/') + 1) : name.Value;

			var root = CreateTreeRoot(nameValue, path);

			root.Exclusions = configuration.ChildNodes
				.OfType<XmlElement>()
				.Where(element => element.Name.Equals("exclude"))
				.Select(excludeNode => CreateExcludeEntry(excludeNode, root))
				.ToList();

			return root;
		}

		protected virtual ITemplateTreeRoot CreateTreeRoot(string name, string path) => new TemplateTreeRoot(name, path);

		protected virtual IPresetTreeExclusion CreateExcludeEntry(XmlElement excludeNode, ITemplateTreeRoot root)
		{
			if (excludeNode.HasAttribute("path"))
			{
				return new PathBasedPresetTreeExclusion(excludeNode.GetExpectedAttribute("path"), root.Path);
			}

			var exclusions = excludeNode.ChildNodes
				.OfType<XmlElement>()
				.Where(element => element.Name.Equals("except") && element.HasAttribute("name"))
				.Select(element => element.GetExpectedAttribute("name"))
				.ToArray();

			if (excludeNode.HasAttribute("children"))
			{
				return new ChildrenOfPathBasedPresetTreeExclusion(root.Path, exclusions, root.Path);
			}

			if (excludeNode.HasAttribute("childrenOfPath"))
			{
				return new ChildrenOfPathBasedPresetTreeExclusion(excludeNode.GetExpectedAttribute("childrenOfPath"), exclusions, root.Path);
			}

			throw new InvalidOperationException($"Unable to parse invalid exclusion value: {excludeNode.OuterXml}");
		}
		protected virtual void EnsureEntriesExist(string configurationName)
		{
			// no entries = throw!
			if (_includeEntries.Count == 0) throw new InvalidOperationException($"No include entries were present on the predicate for the {configurationName} Leprechaun configuration. You must explicitly specify the templates you want to include.");
		}
	}
}