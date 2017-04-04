using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Configy.Containers;
using Leprechaun.Filters.Exclusions;
using Leprechaun.Model;
using Rainbow.Storage;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;

namespace Leprechaun.Filters
{
	public class StandardTemplatePredicate : ITemplatePredicate, ITreeRootFactory
	{
		private readonly string _rootNamespace;
		private readonly IList<TemplateTreeRoot> _includeEntries;

		public StandardTemplatePredicate(XmlNode configNode, IContainer configuration, string rootNamespace)
		{
			_rootNamespace = rootNamespace;
			Assert.ArgumentNotNull(configNode, nameof(configNode));
			Assert.ArgumentNotNull(configuration, nameof(configuration));
			Assert.ArgumentNotNull(rootNamespace, nameof(rootNamespace));

			_includeEntries = ParsePreset(configNode);

			EnsureEntriesExist(configuration?.Name ?? "Unknown");
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

		public virtual TreeRoot[] GetRootPaths()
		{
			return _includeEntries.ToArray<TreeRoot>();
		}

		/// <param name="template">Template to get a root NS for. MAY BE NULL, in which case a general root NS should be returned.</param>
		public virtual string GetRootNamespace(TemplateInfo template)
		{
			return _rootNamespace;
		}

		/// <summary>
		/// Checks if a preset includes a given item
		/// </summary>
		protected virtual bool Includes(TemplateTreeRoot entry, TemplateInfo itemData)
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

		protected virtual bool ExcludeMatches(TemplateTreeRoot entry, TemplateInfo itemData)
		{
			foreach (var exclude in entry.Exclusions)
			{
				var result = exclude.Evaluate(itemData.Path);

				if (!result) return false;
			}

			return true;
		}

		protected virtual IList<TemplateTreeRoot> ParsePreset(XmlNode configuration)
		{
			var presets = configuration.ChildNodes
				.Cast<XmlNode>()
				.Where(node => node.Name == "include")
				.Select(CreateIncludeEntry)
				.ToList();

			var names = new HashSet<string>();
			foreach (var preset in presets)
			{
				if (!names.Contains(preset.Name))
				{
					names.Add(preset.Name);
					continue;
				}

				throw new InvalidOperationException("Multiple predicate include nodes had the same name '{0}'. This is not allowed. Note that this can occur if you did not specify the name attribute and two include entries end in an item with the same name. Use the name attribute on the include tag to give a unique name.".FormatWith(preset.Name));
			}

			return presets;
		}

		protected virtual void EnsureEntriesExist(string configurationName)
		{
			// no entries = throw!
			if (_includeEntries.Count == 0) throw new InvalidOperationException($"No include entries were present on the predicate for the {configurationName} Leprechaun configuration. You must explicitly specify the templates you want to include.");
		}

		protected virtual TemplateTreeRoot CreateIncludeEntry(XmlNode configuration)
		{
			string path = GetExpectedAttribute(configuration, "path");

			// ReSharper disable once PossibleNullReferenceException
			var name = configuration.Attributes["name"];
			string nameValue = name == null ? path.Substring(path.LastIndexOf('/') + 1) : name.Value;

			var root = new TemplateTreeRoot(nameValue, path);

			root.Exclusions = configuration.ChildNodes
				.OfType<XmlElement>()
				.Where(element => element.Name.Equals("exclude"))
				.Select(excludeNode => CreateExcludeEntry(excludeNode, root))
				.ToList();

			return root;
		}

		protected virtual IPresetTreeExclusion CreateExcludeEntry(XmlElement excludeNode, TemplateTreeRoot root)
		{
			if (excludeNode.HasAttribute("path"))
			{
				return new PathBasedPresetTreeExclusion(GetExpectedAttribute(excludeNode, "path"), root);
			}

			var exclusions = excludeNode.ChildNodes
				.OfType<XmlElement>()
				.Where(element => element.Name.Equals("except") && element.HasAttribute("name"))
				.Select(element => GetExpectedAttribute(element, "name"))
				.ToArray();

			if (excludeNode.HasAttribute("children"))
			{
				return new ChildrenOfPathBasedPresetTreeExclusion(root.Path, exclusions, root);
			}

			if (excludeNode.HasAttribute("childrenOfPath"))
			{
				return new ChildrenOfPathBasedPresetTreeExclusion(GetExpectedAttribute(excludeNode, "childrenOfPath"), exclusions, root);
			}

			throw new InvalidOperationException($"Unable to parse invalid exclusion value: {excludeNode.OuterXml}");
		}

		protected static string GetExpectedAttribute(XmlNode node, string attributeName)
		{
			// ReSharper disable once PossibleNullReferenceException
			var attribute = node.Attributes[attributeName];

			if (attribute == null) throw new InvalidOperationException("Missing expected '{0}' attribute on '{1}' node while processing predicate: {2}".FormatWith(attributeName, node.Name, node.OuterXml));

			return attribute.Value;
		}

		IEnumerable<TreeRoot> ITreeRootFactory.CreateTreeRoots()
		{
			return _includeEntries;
		}
	}
}
