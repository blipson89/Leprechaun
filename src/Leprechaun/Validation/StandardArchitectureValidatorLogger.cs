using System;
using System.Collections.Generic;
using Leprechaun.Logging;
using Leprechaun.Model;

namespace Leprechaun.Validation
{
	public class StandardArchitectureValidatorLogger : IArchitectureValidatorLogger
	{
		private readonly ILogger _logger;

		public StandardArchitectureValidatorLogger(ILogger logger)
		{
			_logger = logger;
		}

		public virtual void MissingBaseTemplateInModel(TemplateCodeGenerationMetadata template, Guid missingTemplateId)
		{
			_logger.Warn($"{template.Path} ({template.Id})");
			_logger.Warn($"Unable to resolve base template {missingTemplateId}. No fields from this template will exist.");
		}

		public virtual void DuplicateTemplateNames(IEnumerable<TemplateCodeGenerationMetadata> duplicates)
		{
			_logger.Error("Duplicate template names in the same namespace were found; fix this architecture smell by renaming one.");
			foreach (var dupe in duplicates)
			{
				_logger.Error($"> {dupe.Path} ({dupe.Id})");
			}
		}

		public virtual void DuplicateFieldNames(IEnumerable<TemplateFieldCodeGenerationMetadata> duplicates)
		{
			_logger.Error("Duplicate template field names were found; fix this architecture smell by renaming one.");
			_logger.Error("Note: this can also occur when an inheriting template redefines a field on the base template.");
			foreach (var dupe in duplicates)
			{
				_logger.Error($"> {dupe.Path} ({dupe.Id})");
			}
		}

		public virtual void FieldNamedSameAsTemplate(TemplateCodeGenerationMetadata template, TemplateFieldCodeGenerationMetadata field)
		{
			_logger.Error($"{template.Path} ({template.Id})");
			_logger.Error($"The field {field.Name} has the same name as its template. This is an architecture smell, please rename it.");
		}

		public void TemplateInMultipleModules(IEnumerable<(string resultingNamespace, TemplateCodeGenerationMetadata dupe)> duplicates)
		{
			_logger.Warn("The following templates were found in multiple modules. All base template references will use the first namespace.");
			_logger.Warn("Note: you can suppress this warning by setting 'allowTemplatesInMultipleModules' to 'true' on the architectureValidator.");
			_logger.Warn("");
			foreach (var duplicate in duplicates)
			{
				_logger.Warn($"'{duplicate.dupe.Name}' will use the namespace '{duplicate.resultingNamespace}'");
			}
			
		}
	}
}