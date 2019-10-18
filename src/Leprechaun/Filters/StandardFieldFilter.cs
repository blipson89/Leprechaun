using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Leprechaun.Model;

namespace Leprechaun.Filters
{
	public class StandardFieldFilter : IFieldFilter
	{
		protected readonly HashSet<Guid> ExcludedFieldIds = new HashSet<Guid>();
		protected readonly List<string> ExcludedFieldSpecs = new List<string>();

		public StandardFieldFilter(XmlNode configNode)
		{
			Assert.IsNotNull(configNode, "configNode");

			foreach (var element in configNode.ChildNodes.OfType<XmlElement>())
			{
				var idAttribute = element.Attributes["fieldId"];
				if (idAttribute != null && Guid.TryParse(idAttribute.InnerText, out Guid candidate))
				{
					ExcludedFieldIds.Add(candidate);
				}

				var nameAttribute = element.Attributes["name"];
				if (nameAttribute != null)
				{
					ExcludedFieldSpecs.Add(nameAttribute.InnerText);
				}
			}
		}

		protected StandardFieldFilter()
		{
			// available for testing purposes without needing XML
		}

		public virtual bool Includes(TemplateFieldInfo field)
		{
			if (ExcludedFieldIds.Contains(field.Id)) return false;

			foreach (var spec in ExcludedFieldSpecs)
			{
				if (IsWildcardMatch(field.Name, spec)) return false;
			}

			return true;
		}

		protected virtual bool IsWildcardMatch(string input, string wildcards)
		{
			if (wildcards.IndexOf('*') < 0) return wildcards.Equals(input, StringComparison.OrdinalIgnoreCase);

			return Regex.IsMatch(input, "^" + Regex.Escape(wildcards).Replace("\\*", ".*").Replace("\\?", ".") + "$", RegexOptions.IgnoreCase);
		}
	}
}
