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

			var orchestrator = LeprechaunConfigurationManager.Configuration.Shared.Resolve<Orchestrator>();

			var metadata = orchestrator.GenerateMetadata(LeprechaunConfigurationManager.Configuration.Configurations);

			System.Console.ReadKey();
		}
	}
}
