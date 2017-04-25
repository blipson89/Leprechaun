using System;
using System.Collections.Generic;
using Configy.Parsing;

namespace Leprechaun.Console.Variables
{
	public class ConfigPathVariableReplacer : ContainerDefinitionVariablesReplacer
	{
		private readonly string _configDirectory;

		public ConfigPathVariableReplacer(string configDirectory)
		{
			_configDirectory = configDirectory;
		}

		public override void ReplaceVariables(ContainerDefinition definition)
		{
			ApplyVariables(definition.Definition, new Dictionary<string, string> { { "configDirectory", _configDirectory } });
		}
	}
}
