using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Leprechaun.Model;

namespace Leprechaun.Validation
{
	public class StandardArchitectureValidator : IArchitectureValidator
	{
		private readonly IArchitectureValidatorLogger _logger;
		private readonly bool _allowFieldNamesIdenticalToTemplateName;
		private readonly bool _allowNovelFieldNames;

		public StandardArchitectureValidator(XmlNode configNode, IArchitectureValidatorLogger logger)
		{
			if (configNode.Attributes != null)
			{
				_allowNovelFieldNames = configNode.Attributes["allowNovelFieldNames"]?.Value == "true";
				_allowFieldNamesIdenticalToTemplateName = configNode.Attributes["allowFieldNamesIdenticalToTemplateName"]?.Value == "true";
			}
			_logger = logger;
		}

		public void Validate(TemplateCodeGenerationMetadata[] allTemplates)
		{
			var allTemplatesIndex = allTemplates.ToDictionary(config => config.Id);

			// ReSharper disable once ReplaceWithSingleAssignment.False
			bool errors = false;

			// ReSharper disable once ConvertIfToOrExpression
			if (!ValidateTemplateNamesAreNovel(allTemplates))
			{
				errors = true;
			}

			foreach (var template in allTemplates)
			{
				ValidateBaseTemplatesAreKnown(template);

				if (!_allowFieldNamesIdenticalToTemplateName && !ValidateTemplateHasNoFieldsIdenticalToTemplateName(template))
				{
					errors = true;
				}

				if (!_allowNovelFieldNames && !ValidateTemplateFieldNamesAreNovel(template, allTemplatesIndex))
				{
					errors = true;
				}
			}

			if(errors) throw new ArchitectureValidationException("At least one error occurred validating your template architecture.");
		}

		protected virtual bool ValidateBaseTemplatesAreKnown(TemplateCodeGenerationMetadata template)
		{
			var unresolvedBaseTemplates = template.TemplateInfo.BaseTemplateIds
				.Where(baseId => template.BaseTemplates.All(baseTemplate => baseTemplate.Id != baseId))
				.ToArray();

			if (unresolvedBaseTemplates.Length == 0) return true;

			foreach (var unresolved in unresolvedBaseTemplates)
			{
				_logger.MissingBaseTemplateInModel(template, unresolved);
			}

			return false;
		}

		protected virtual bool ValidateTemplateNamesAreNovel(TemplateCodeGenerationMetadata[] allTemplates)
		{
			var groups = allTemplates
				.GroupBy(template => template.FullTypeName, StringComparer.OrdinalIgnoreCase)
				.Where(group => group.Count() > 1)
				.ToArray();

			if (groups.Length == 0) return true;

			foreach (var duplicate in groups)
			{
				_logger.DuplicateTemplateNames(duplicate);
			}

			return false;
		}

		protected virtual bool ValidateTemplateHasNoFieldsIdenticalToTemplateName(TemplateCodeGenerationMetadata template)
		{
			// check for a field named the same as its template; this is bad because you cannot have a property named the same as its enclosing class
			var fieldsWithSameNameAsEnclosingTemplate = template.OwnFields
				.Where(field => FieldCodeNameEqualsTemplateCodeName(field, template))
				.ToArray();

			if (fieldsWithSameNameAsEnclosingTemplate.Length == 0) return true;

			foreach (var childDuplicate in fieldsWithSameNameAsEnclosingTemplate)
			{
				_logger.FieldNamedSameAsTemplate(template, childDuplicate);
			}

			return false;
		}

		protected virtual bool FieldCodeNameEqualsTemplateCodeName(TemplateFieldCodeGenerationMetadata field, TemplateCodeGenerationMetadata template)
		{
			return field.CodeName.Equals(template.CodeName);
		}

		protected virtual bool ValidateTemplateFieldNamesAreNovel(TemplateCodeGenerationMetadata template, IReadOnlyDictionary<Guid, TemplateCodeGenerationMetadata> allTemplatesIndex)
		{
			// look for fields with identical names in the whole current template's inheritance tree
			var fieldsWithIdenticalNames = GetAllBaseTemplates(template, allTemplatesIndex)
				.SelectMany(currentTemplate => currentTemplate.OwnFields)
				.Concat(template.OwnFields)
				.GroupBy(field => field.CodeName, StringComparer.OrdinalIgnoreCase)
				.Where(field => field.Count() > 1)
				.ToArray();

			if (fieldsWithIdenticalNames.Length == 0) return true;

			foreach (var duplicate in fieldsWithIdenticalNames)
			{
				_logger.DuplicateFieldNames(duplicate);
			}

			return false;
		}

		protected virtual IEnumerable<TemplateCodeGenerationMetadata> GetAllBaseTemplates(TemplateCodeGenerationMetadata template, IReadOnlyDictionary<Guid, TemplateCodeGenerationMetadata> allTemplatesIndex)
		{
			var parentBases = new Queue<TemplateCodeGenerationMetadata>(template.BaseTemplates);
			var bases = new Dictionary<Guid, TemplateCodeGenerationMetadata>();

			while (parentBases.Count > 0)
			{
				var currentBase = parentBases.Dequeue();

				// already processed this template; skip it (e.g. a template cycle), or if it's the parent template
				if (bases.ContainsKey(currentBase.Id) || currentBase.Id.Equals(template.Id)) continue;

				// add grandparent base templates to processing queue
				var newBases = currentBase.BaseTemplates;

				foreach (var newBase in newBases) parentBases.Enqueue(newBase);

				// add parent base template to bases
				bases.Add(currentBase.Id, currentBase);
			}

			return bases.Values.ToList();
		}
	}
}
