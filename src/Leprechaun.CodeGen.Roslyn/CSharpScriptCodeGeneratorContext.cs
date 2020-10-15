using System.Collections.Generic;
using System.Text;
using Leprechaun.Logging;
using Leprechaun.Model;

namespace Leprechaun.CodeGen.Roslyn
{
	public class CSharpScriptCodeGeneratorContext
	{
		private readonly ConfigurationCodeGenerationMetadata _metadata;

		public CSharpScriptCodeGeneratorContext(ConfigurationCodeGenerationMetadata metadata, ILogger logger)
		{
			Log = logger;
			_metadata = metadata;
		}

		public IReadOnlyCollection<TemplateCodeGenerationMetadata> Templates => _metadata.Metadata;

		public IReadOnlyCollection<RenderingCodeGenerationMetadata> Renderings  => _metadata.RenderingMetadata;

		public string ConfigurationName => _metadata.Configuration.Name;

		public string GenericRootNamespace => _metadata.RootNamespace;

		public ILogger Log { get; }

		public StringBuilder Code { get; } = new StringBuilder();
	}
}
