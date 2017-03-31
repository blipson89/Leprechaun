using System.Collections.Generic;
using System.Text;
using Leprechaun.Logging;
using Leprechaun.Model;

namespace Leprechaun.CodeGen.Roslyn
{
	public class CSharpScriptCodeGeneratorContext
	{
		private readonly ConfigurationCodeGenerationMetadata _metadata;

		public CSharpScriptCodeGeneratorContext(ConfigurationCodeGenerationMetadata metadata, ILogger logger, string rootNamespace)
		{
			Log = logger;
			RootNamespace = rootNamespace;
			_metadata = metadata;
		}

		public IReadOnlyCollection<TemplateCodeGenerationMetadata> Templates => _metadata.Metadata;

		public string ConfigurationName => _metadata.Configuration.Name;

		public ILogger Log { get; }

		public string RootNamespace { get; }

		public StringBuilder Code { get; } = new StringBuilder();
	}
}
