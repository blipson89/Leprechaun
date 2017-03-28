using System;
using System.Collections.Generic;
using Leprechaun.Model;

namespace Leprechaun.Validation
{
	public class StandardArchitectureValidatorLogger : IArchitectureValidatorLogger
	{
		public virtual void MissingBaseTemplateInModel(TemplateCodeGenerationMetadata template, Guid missingTemplateId)
		{
			throw new NotImplementedException();
		}

		public virtual void DuplicateTemplateNames(IEnumerable<TemplateCodeGenerationMetadata> duplicates)
		{
			throw new NotImplementedException();
		}

		public virtual void DuplicateFieldNames(IEnumerable<TemplateFieldCodeGenerationMetadata> duplicates)
		{
			throw new NotImplementedException();
		}

		public virtual void FieldNamedSameAsTemplate(TemplateCodeGenerationMetadata template, TemplateFieldCodeGenerationMetadata field)
		{
			throw new NotImplementedException();
		}
	}
}