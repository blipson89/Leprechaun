using System;
using System.Collections.Generic;
using Leprechaun.Model;

namespace Leprechaun.Validation
{
	public interface IArchitectureValidatorLogger
	{
		void MissingBaseTemplateInModel(TemplateCodeGenerationMetadata template, Guid missingTemplateId);
		void DuplicateTemplateNames(IEnumerable<TemplateCodeGenerationMetadata> duplicates);
		void DuplicateFieldNames(IEnumerable<TemplateFieldCodeGenerationMetadata> duplicates);
		void FieldNamedSameAsTemplate(TemplateCodeGenerationMetadata template, TemplateFieldCodeGenerationMetadata field);
		void TemplateInMultipleModules(IEnumerable<(string resultingNamespace, TemplateCodeGenerationMetadata dupe)> duplicates);
	}
}
