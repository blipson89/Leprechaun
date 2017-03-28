using System.Collections.Generic;
using Leprechaun.Model;

namespace Leprechaun.MetadataGeneration
{
	public interface ITemplateMetadataGenerator
	{
		IReadOnlyList<ConfigurationCodeGenerationMetadata> Generate(params TemplateConfiguration[] configurations);
	}
}