using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;
using Leprechaun.CodeGen;
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

			var timer = new Stopwatch();
			timer.Start();

			var configuration = BuildConfiguration();

			// start pre-compiling templates (for Roslyn provider anyway)
			// this lets C# be compiling in the background while we read the files to generate from disk
			// and saves time
			var preload = Task.Run(() =>
			{
				foreach (var config in configuration.Configurations) config.Resolve<ICodeGenerator>();
			});

			// the orchestrator controls the overall codegen flow
			var orchestrator = configuration.Shared.Resolve<Orchestrator>();

			// we generate template data that will feed code generation
			var metadata = orchestrator.GenerateMetadata(configuration.Configurations);

			// make sure we're done preloading the compiled codegen templates
			preload.Wait();

			// emit actual code using the codegens for each config
			foreach (var meta in metadata)
			{
				var codeGen = meta.Configuration.Resolve<ICodeGenerator>();
				codeGen.GenerateCode(meta);
			}

			timer.Stop();
			System.Console.ForegroundColor = ConsoleColor.Green;
			System.Console.WriteLine($"Leprechaun has completed in {timer.ElapsedMilliseconds}ms.");
			System.Console.ResetColor();

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
