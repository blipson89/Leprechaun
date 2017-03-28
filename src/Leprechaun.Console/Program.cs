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

			orchestrator.GenerateMetadata(LeprechaunConfigurationManager.Configuration.Configurations);
		}
	}
}
