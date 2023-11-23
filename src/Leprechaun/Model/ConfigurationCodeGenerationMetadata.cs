using System.Collections.Generic;
using System.Diagnostics;
using Configy.Containers;
using Leprechaun.Filters;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{Configuration.Name}")]
	public class ConfigurationCodeGenerationMetadata
	{
		public ConfigurationCodeGenerationMetadata(IContainer configuration, IReadOnlyCollection<TemplateCodeGenerationMetadata> metadata, RenderingCodeGenerationMetadata[] renderings)
		{
			Configuration = configuration;
			Metadata = metadata;
			RenderingMetadata = renderings;
		}

		public IContainer Configuration { get; }

		public string RootNamespace => Configuration.Resolve<ITemplatePredicate>().GetRootNamespace(null);

		public IReadOnlyCollection<TemplateCodeGenerationMetadata> Metadata { get; }

		public IReadOnlyCollection<RenderingCodeGenerationMetadata> RenderingMetadata { get; }
	}
}
