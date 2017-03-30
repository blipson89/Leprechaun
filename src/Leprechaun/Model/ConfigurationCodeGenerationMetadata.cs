using System.Collections.Generic;
using System.Diagnostics;
using Configy.Containers;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{Configuration.Name}")]
	public class ConfigurationCodeGenerationMetadata
	{
		public ConfigurationCodeGenerationMetadata(IContainer configuration, IReadOnlyCollection<TemplateCodeGenerationMetadata> metadata)
		{
			Configuration = configuration;
			Metadata = metadata;
		}

		public IContainer Configuration { get; }

		public IReadOnlyCollection<TemplateCodeGenerationMetadata> Metadata { get; }
	}
}
