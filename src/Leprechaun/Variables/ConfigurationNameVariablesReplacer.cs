using System;
using System.Collections.Generic;
using Configy.Parsing;

namespace Leprechaun.Variables
{
	/// <summary>
	/// Enables the use of $(configurationName) in configuration vars
	/// </summary>
	public class ConfigurationNameVariablesReplacer : ContainerDefinitionVariablesReplacer
	{
		public override void ReplaceVariables(ContainerDefinition definition)
		{
			if (definition.Name == null) throw new ArgumentException("Configuration without a name was used. Add a name attribute to all configurations.", nameof(definition));

			ApplyVariables(definition.Definition, new Dictionary<string, string> { { "configurationName", definition.Name } });
		}
	}
}
