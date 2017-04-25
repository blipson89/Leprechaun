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
			// PARSE ARGS
			var argsParser = new CommandLineParser.CommandLineParser();
			var parsedArgs = new ConsoleArgs();

			argsParser.ExtractArgumentAttributes(parsedArgs);
			argsParser.ParseCommandLine(args);

			if (parsedArgs.Help)
			{
				argsParser.ShowUsage();
				Environment.Exit(-1);
			}

			// RUN LEPRECHAUN
			var timer = new Stopwatch();
			timer.Start();

			var configuration = BuildConfiguration(parsedArgs);

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
				System.Console.WriteLine();
				System.Console.ForegroundColor = ConsoleColor.Cyan;
				System.Console.WriteLine($"> Generating {meta.Configuration.Name} ({meta.Metadata.Count} templates)");
				System.Console.ResetColor();

				var codeGen = meta.Configuration.Resolve<ICodeGenerator>();
				codeGen.GenerateCode(meta);
			}

			timer.Stop();
			System.Console.ForegroundColor = ConsoleColor.Green;
			System.Console.WriteLine($"Leprechaun has completed in {timer.ElapsedMilliseconds}ms.");
			System.Console.ResetColor();

			if (parsedArgs.NoExit)
			{
				System.Console.ReadKey();
			}
		}

		private static LeprechaunConfigurationBuilder BuildConfiguration(ConsoleArgs args)
		{
			if (args.Watch)
			{
				System.Console.WriteLine("Sorry, watch is not yet implemented.");
				Environment.Exit(1);
			}

			var config = new XmlDocument();
			config.Load(args.ConfigFilePath);

			var replacer = new ChainedVariablesReplacer(new ConfigurationNameVariablesReplacer(), new HelixConventionVariablesReplacer());

			return new LeprechaunConfigurationBuilder(replacer, config.DocumentElement["configurations"], config.DocumentElement["defaults"], config.DocumentElement["shared"], args.ConfigFilePath, new ConfigurationImportPathResolver(new ConsoleLogger()));
		}
	}
}
