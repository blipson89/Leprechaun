using System;
using System.Collections.Generic;
using System.Linq;
using Leprechaun.Model;
using Rainbow.Model;
using Rainbow.Storage;
using Sitecore;

namespace Leprechaun.TemplateReaders
{
	public class DataStoreTemplateReader : ITemplateReader
	{
		private static readonly Guid TemplateTemplateId = TemplateIDs.Template.Guid;
		private static readonly Guid TemplateFieldTemplateId = TemplateIDs.TemplateField.Guid;
		private static readonly Guid TemplateSectionTemplateId = TemplateIDs.TemplateSection.Guid;

		private static readonly Guid DisplayNameFieldId = FieldIDs.DisplayName.Guid;
		private static readonly Guid TemplateFieldTitleFieldId = TemplateFieldIDs.Title.Guid;
		private static readonly Guid HelpTextFieldId = TemplateFieldIDs.Description.Guid;
		private static readonly Guid FieldTypeFieldId = TemplateFieldIDs.Type.Guid;
		private static readonly Guid SourceFieldId = TemplateFieldIDs.Source.Guid;
		private static readonly Guid SortOrderFieldId = FieldIDs.Sortorder.Guid;
		private static readonly Guid BaseTemplateFieldId = FieldIDs.BaseTemplate.Guid;

		private readonly IDataStore _dataStore;

		public DataStoreTemplateReader(IDataStore dataStore)
		{
			_dataStore = dataStore;
		}

		public TemplateInfo[] GetTemplates(params TreeRoot[] rootPaths)
		{
			return rootPaths
				.AsParallel()
				.SelectMany(root =>
				{
					var rootItem = _dataStore.GetByPath(root.Path, root.DatabaseName);

					if (rootItem == null) return Enumerable.Empty<TemplateInfo>();

					// because a path could match more than one item we have to SelectMany again
					return rootItem.SelectMany(ParseTemplates);
				})
				.ToArray();
		}

		protected virtual IEnumerable<TemplateInfo> ParseTemplates(IItemData root)
		{
			var processQueue = new Queue<IItemData>();

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

		protected virtual TemplateInfo ParseTemplate(IItemData templateItem)
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

		protected virtual TemplateFieldInfo[] ParseTemplateFields(IItemData templateItem)
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

		protected virtual string GetFieldValue(IItemData item, Guid fieldId, string defaultValue)
		{
			foreach (var field in item.SharedFields)
			{
				if (field.FieldId == fieldId) return field.Value;
			}

			foreach (var language in item.UnversionedFields)
			{
				foreach (var field in language.Fields)
				{
					if (field.FieldId == fieldId) return field.Value;
				}
			}

			foreach (var version in item.Versions)
			{
				foreach (var field in version.Fields)
				{
					if (field.FieldId == fieldId) return field.Value;
				}
			}

			return defaultValue;
		}

		protected virtual int GetFieldValueAsInt(IItemData item, Guid fieldId, int defaultValue)
		{
			var value = GetFieldValue(item, fieldId, string.Empty);

			if (int.TryParse(value, out int result)) return result;

			return defaultValue;
		}

		protected virtual Guid[] ParseBaseTemplatesAndRejectIgnoredBaseTemplates(string value)
		{
			var ignoredIds = IgnoredBaseTemplateIds;

			return ParseMultilistValue(value)
				.Where(id => !ignoredIds.Contains(id))
				.ToArray();
		}

		protected virtual Guid[] ParseMultilistValue(string value)
		{
			return value.Split('|')
				.Select(item =>
				{
					if (Guid.TryParse(item, out Guid result))
					{
						return result;
					}

					return Guid.Empty;
				})
				.Where(item => item != Guid.Empty)
				.ToArray();
		}

		protected virtual ICollection<Guid> IgnoredBaseTemplateIds => new HashSet<Guid> { TemplateIDs.StandardTemplate.Guid, TemplateIDs.Folder.Guid };

        protected virtual Dictionary<Guid, string> GetAllFields(IItemData item)
        {
			var allFields = new Dictionary<Guid, string>();

            foreach (var field in item.SharedFields)
            {
                if (!allFields.ContainsKey(field.FieldId))
                {
                    allFields.Add(field.FieldId, field.Value);
                }
            }

            foreach (var language in item.UnversionedFields)
            {
                foreach (var field in language.Fields)
                {
					if (!allFields.ContainsKey(field.FieldId))
                    {
                        allFields.Add(field.FieldId, field.Value);
                    }
				}
            }

            foreach (var version in item.Versions)
            {
                foreach (var field in version.Fields)
                {
					if (!allFields.ContainsKey(field.FieldId))
                    {
                        allFields.Add(field.FieldId, field.Value);
                    }
				}
            }

            return allFields;
        }
	}
}
