using System.Collections.Generic;
using Configy.Containers;
using Leprechaun.Model;

namespace Leprechaun
{
	public interface IOrchestrator
	{
		IReadOnlyList<ConfigurationCodeGenerationMetadata> GenerateMetadata(params IContainer[] configurations);
	}
}