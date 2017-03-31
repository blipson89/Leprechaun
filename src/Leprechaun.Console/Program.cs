using System.Xml;
using Leprechaun.Console.Variables;

namespace Leprechaun.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			// args:
			// -watch
			// -config=c:\foo.config
			// ??

			var configuration = BuildConfiguration();

			var orchestrator = configuration.Shared.Resolve<Orchestrator>();

			var metadata = orchestrator.GenerateMetadata(configuration.Configurations);

			System.Console.ReadKey();
		}

		private static LeprechaunConfigurationBuilder BuildConfiguration()
		{
			var config = new XmlDocument();
			config.Load("Leprechaun.config");

			var replacer = new ChainedVariablesReplacer(new ConfigurationNameVariablesReplacer(), new HelixConventionVariablesReplacer());

			return new LeprechaunConfigurationBuilder(replacer, config.DocumentElement["configurations"], config.DocumentElement["defaults"], config.DocumentElement["shared"]);
		}
	}
}
