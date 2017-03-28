using System.Collections.Generic;
using Configy.Containers;

namespace Leprechaun.Model
{
	public class ConfigurationCodeGenerationMetadata
	{
		public ConfigurationCodeGenerationMetadata(IContainer configuration, IEnumerable<TemplateCodeGenerationMetadata> metadata)
		{
			Configuration = configuration;
			Metadata = metadata;
		}

		public IContainer Configuration { get; }
		public IEnumerable<TemplateCodeGenerationMetadata> Metadata { get; }
	}
}
