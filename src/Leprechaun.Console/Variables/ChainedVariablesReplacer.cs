using Configy.Parsing;

namespace Leprechaun.Console.Variables
{
	public class ChainedVariablesReplacer : IContainerDefinitionVariablesReplacer
	{
		private readonly IContainerDefinitionVariablesReplacer[] _innerReplacers;

		public ChainedVariablesReplacer(params IContainerDefinitionVariablesReplacer[] innerReplacers)
		{
			_innerReplacers = innerReplacers;
		}

		public virtual void ReplaceVariables(ContainerDefinition definition)
		{
			foreach (var replacer in _innerReplacers)
			{
				replacer.ReplaceVariables(definition);
			}
		}
	}
}
