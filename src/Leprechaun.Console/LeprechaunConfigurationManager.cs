using System.Xml;
using Configy.Parsing;
using Leprechaun.Console.Variables;

namespace Leprechaun.Console
{
	public static class LeprechaunConfigurationManager
	{
		// TODO: temp hack
		// needs includes maybe?
		static LeprechaunConfigurationManager()
		{
			var config = new XmlDocument();
			config.Load("Leprechaun.config");

			var replacer = new ChainedVariablesReplacer(new ConfigurationNameVariablesReplacer(), new HelixConventionVariablesReplacer());

			Configuration = new LeprechaunConfigurationBuilder(replacer, config.DocumentElement["configurations"], config.DocumentElement["defaults"], config.DocumentElement["shared"]);
		}

		public static LeprechaunConfigurationBuilder Configuration { get; }
	}
}
