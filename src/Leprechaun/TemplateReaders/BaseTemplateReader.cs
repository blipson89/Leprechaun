using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Leprechaun.Adapters;
using Leprechaun.Extensions;
using Leprechaun.Filters;
using Leprechaun.Model;

namespace Leprechaun.TemplateReaders
{
	public abstract class BaseTemplateReader : ITemplateReader
	{
		internal static readonly Guid TemplateTemplateId = new Guid("{AB86861A-6030-46C5-B394-E8F99E8B87DB}");
		internal static readonly Guid TemplateFieldTemplateId = new Guid("{455A3E98-A627-4B40-8035-E683A0331AC7}");
		internal static readonly Guid TemplateSectionTemplateId = new Guid("{E269FBB5-3750-427A-9149-7AA950B49301}");
		
		internal static readonly Guid DisplayNameFieldId = new Guid("{B5E02AD9-D56F-4C41-A065-A133DB87BDEB}");
		internal static readonly Guid TemplateFieldTitleFieldId = new Guid("{19A69332-A23E-4E70-8D16-B2640CB24CC8}");
		internal static readonly Guid HelpTextFieldId = new Guid("{577F1689-7DE4-4AD2-A15F-7FDC1759285F}");
		internal static readonly Guid FieldTypeFieldId = new Guid("{AB162CC0-DC80-4ABF-8871-998EE5D7BA32}");
		internal static readonly Guid SourceFieldId = new Guid("{1EB8AE32-E190-44A6-968D-ED904C794EBF}");
		internal static readonly Guid SortOrderFieldId = new Guid("{BA3F86A2-4A1C-4D78-B63D-91C2779C1B5E}");
		internal static readonly Guid BaseTemplateFieldId = new Guid("{12C33F3F-86C5-43A5-AEB4-5598CEC45116}");

		internal static readonly Guid StandardTemplateId = new Guid("{1930BBEB-7805-471A-A3BE-4858AC7CF696}");
		internal static readonly Guid FolderId = new Guid("{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}");

		protected BaseTemplateReader(XmlNode configNode)
		{
			ParseExcludedTemplates(configNode);
		}

		protected void ParseExcludedTemplates(XmlNode configNode)
		{
			if (configNode?.ChildNodes == null)
				return;

			IEnumerable<XmlNode> nodes = configNode.ChildNodes
				.Cast<XmlNode>()
				.Where(node => node.Name == "excludedBaseTemplate")
				.ToList();

			foreach (XmlNode excludedTemplate in nodes)
			{
				var id = excludedTemplate.GetExpectedAttribute("id");
				if (Guid.TryParse(id, out Guid templateId))
				{
					_ignoredBaseTemplateIds.Add(templateId);
				}
				else
				{
					throw new InvalidOperationException($"'{id}' is not a valid Guid in '{excludedTemplate.OuterXml}'");
				}
			}

		}

		public abstract TemplateInfo[] GetTemplates(ITemplatePredicate predicate);

		protected virtual IEnumerable<TemplateInfo> ParseTemplates(IItemDataAdapter root)
		{
			var processQueue = new Queue<IItemDataAdapter>();

			processQueue.Enqueue(root);

			while (processQueue.Count > 0)
			{
				var currentTemplate = processQueue.Dequeue();

				// if it's a template we parse it and skip adding children (nested templates not really allowed)
				if (currentTemplate.TemplateId == TemplateTemplateId)
				{
					yield return ParseTemplate(currentTemplate);
					continue;
				}

				// it's not a template (e.g. a template folder) so we want to scan its children for templates to parse
				var children = currentTemplate.GetChildren();
				foreach (var child in children)
				{
					processQueue.Enqueue(child);
				}
			}
		}

		protected virtual TemplateInfo ParseTemplate(IItemDataAdapter templateItem)
		{
			if (templateItem == null) throw new ArgumentException("Template item passed to parse was null", nameof(templateItem));
			if (templateItem.TemplateId != TemplateTemplateId) throw new ArgumentException("Template item passed to parse was not a Template item", nameof(templateItem));

			var result = new TemplateInfo
			{
				Id = templateItem.Id,
				BaseTemplateIds = ParseBaseTemplatesAndRejectIgnoredBaseTemplates(GetFieldValue(templateItem, BaseTemplateFieldId, string.Empty)),
				HelpText = GetFieldValue(templateItem, HelpTextFieldId, string.Empty),
				Name = templateItem.Name,
				OwnFields = ParseTemplateFields(templateItem),
				Path = templateItem.Path
			};

			return result;
		}

		protected virtual TemplateFieldInfo[] ParseTemplateFields(IItemDataAdapter templateItem)
		{
			var results = new List<TemplateFieldInfo>();

			var sections = templateItem.GetChildren().Where(child => child.TemplateId == TemplateSectionTemplateId);

			foreach (var section in sections)
			{
				var fields = section.GetChildren().Where(child => child.TemplateId == TemplateFieldTemplateId);

				foreach (var field in fields)
				{
					results.Add(new TemplateFieldInfo
					{
						Id = field.Id,
						DisplayName = GetFieldValue(field, TemplateFieldTitleFieldId, null) ?? GetFieldValue(field, DisplayNameFieldId, string.Empty),
						HelpText = GetFieldValue(field, HelpTextFieldId, string.Empty),
						Name = field.Name,
						Path = field.Path,
						Section = section.Name,
						SortOrder = GetFieldValueAsInt(field, SortOrderFieldId, 100),
						Source = GetFieldValue(field, SourceFieldId, string.Empty),
						Type = GetFieldValue(field, FieldTypeFieldId, string.Empty),
						AllFields = GetAllFields(field)
					});
				}
			}

			return results.ToArray();
		}

		protected virtual string GetFieldValue(IItemDataAdapter item, Guid fieldId, string defaultValue)
		{
			foreach (IItemFieldValueAdapter field in item.SharedFields)
			{
				if (field.FieldId == fieldId) return field.Value;
			}

			foreach (IItemFieldValueAdapter field in item.UnversionedFields.SelectMany(uf => uf.Fields))
			{
				if (field.FieldId == fieldId) return field.Value;
			}

			foreach (IItemFieldValueAdapter field in item.Versions.SelectMany(v => v.Fields))
			{
				if (field.FieldId == fieldId) return field.Value;
			}

			return defaultValue;
		}

		protected virtual int GetFieldValueAsInt(IItemDataAdapter itemDataAdapter, Guid fieldId, int defaultValue)
		{
			var value = GetFieldValue(itemDataAdapter, fieldId, string.Empty);

			if (int.TryParse(value, out int result)) return result;

			return defaultValue;
		}

		protected virtual Guid[] ParseBaseTemplatesAndRejectIgnoredBaseTemplates(string value)
		{
			ICollection<Guid> ignoredIds = IgnoredBaseTemplateIds;

			return ParseMultilistValue(value)
				.Where(id => !ignoredIds.Contains(id))
				.ToArray();
		}

		protected virtual Guid[] ParseMultilistValue(string value)
		{
			return value.Split('|')
				.Select(item => Guid.TryParse(item, out Guid result) ? result : Guid.Empty)
				.Where(item => item != Guid.Empty)
				.ToArray();
		}

		private readonly HashSet<Guid> _ignoredBaseTemplateIds = new HashSet<Guid>
		{
			StandardTemplateId,
			FolderId
		};

		protected virtual ICollection<Guid> IgnoredBaseTemplateIds => _ignoredBaseTemplateIds;
		

		protected virtual IDictionary<Guid, string> GetAllFields(IItemDataAdapter item)
		{
			var allFields = new Dictionary<Guid, string>();

			foreach (IItemFieldValueAdapter field in item.SharedFields)
			{
				allFields[field.FieldId] = field.Value;
			}

			foreach (IItemFieldValueAdapter field in item.UnversionedFields.SelectMany(f => f.Fields))
			{
				allFields[field.FieldId] = field.Value;
			}

			foreach (IItemFieldValueAdapter field in item.Versions.SelectMany(v => v.Fields))
			{
				allFields[field.FieldId] = field.Value;
			}

			return allFields;
		}
	}
}
