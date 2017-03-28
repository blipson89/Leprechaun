using System.Xml;
using Configy.Parsing;

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

			Configuration = new LeprechaunConfigurationBuilder(new ContainerDefinitionVariablesReplacer(), config.DocumentElement["configurations"], config.DocumentElement["defaults"], config.DocumentElement["shared"]);
		}

		public static LeprechaunConfigurationBuilder Configuration { get; }
	}
}
