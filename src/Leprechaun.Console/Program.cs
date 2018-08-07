using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Leprechaun.CodeGen;
using Leprechaun.Console.Variables;
using Leprechaun.Model;
using Rainbow.Settings;

namespace Leprechaun.Console
{
	public class Program
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
			if (!parsedArgs.NoSplash) Ascii.Leprechaun();

			var appRunTimer = new Stopwatch();
			appRunTimer.Start();

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

			var metadata = GenerateMetadata(orchestrator, configuration);

			// make sure we're done preloading the compiled codegen templates
			preload.Wait();

			System.Console.ForegroundColor = ConsoleColor.Green;
			System.Console.WriteLine($"Code generator has loaded in {appRunTimer.ElapsedMilliseconds}ms.");
			System.Console.ResetColor();

			GenerateCode(metadata);

			if (parsedArgs.Watch)
			{
				System.Console.WriteLine();
				System.Console.WriteLine("Leprechaun is now watching for file changes and rebuilding at need.");
				System.Console.WriteLine("Press Ctrl-C to exit.");
				Watcher.Watch(configuration, new ConsoleLogger(), () => GenerateWatch(orchestrator, configuration));
				var exit = new ManualResetEvent(false);
				exit.WaitOne();
			}

			appRunTimer.Stop();
			System.Console.WriteLine();
			System.Console.ForegroundColor = ConsoleColor.Green;
			System.Console.WriteLine($"Leprechaun has completed in {appRunTimer.ElapsedMilliseconds}ms.");
			System.Console.ResetColor();

			if (parsedArgs.NoExit)
			{
				System.Console.ReadKey();
			}
		}

		private static void GenerateWatch(Orchestrator orchestrator, LeprechaunConfigurationBuilder configuration)
		{
			try
			{
				var metadata = GenerateMetadata(orchestrator, configuration);
				GenerateCode(metadata);
			}
			catch (Exception ex)
			{
				// during watch we don't want errors to terminate the application
				System.Console.ForegroundColor = ConsoleColor.Red;
				System.Console.WriteLine(ex.Message);
				System.Console.ForegroundColor = ConsoleColor.Gray;
				System.Console.WriteLine(ex.StackTrace);
				System.Console.ResetColor();
			}
		}

		private static void GenerateCode(IReadOnlyList<ConfigurationCodeGenerationMetadata> metadata)
		{
			// emit actual code using the codegens for each config
			foreach (var meta in metadata)
			{
				System.Console.WriteLine();
				System.Console.ForegroundColor = ConsoleColor.Cyan;
				var word = meta.Metadata.Count == 1 ? "template" : "templates";
				System.Console.WriteLine($"> Generating {meta.Configuration.Name} ({meta.Metadata.Count} {word})");
				System.Console.ResetColor();

				var codeGen = meta.Configuration.Resolve<ICodeGenerator>();
				codeGen.GenerateCode(meta);
			}
		}

		private static IReadOnlyList<ConfigurationCodeGenerationMetadata> GenerateMetadata(Orchestrator orchestrator, LeprechaunConfigurationBuilder configuration)
		{
			var metadataTimer = new Stopwatch();
			metadataTimer.Start();

			// we generate template data that will feed code generation
			var metadata = orchestrator.GenerateMetadata(configuration.Configurations);

			metadataTimer.Stop();
			System.Console.ForegroundColor = ConsoleColor.Green;
			System.Console.WriteLine(
				$"Loaded metadata for {metadata.Count} configurations ({metadata.Sum(m => m.Metadata.Count)} total templates) in {metadataTimer.ElapsedMilliseconds}ms.");
			System.Console.ResetColor();

			return metadata;
		}

		private static LeprechaunConfigurationBuilder BuildConfiguration(ConsoleArgs args)
		{
			var config = new XmlDocument();

			args.ConfigFilePath = EnsureAbsoluteConfigPath(args.ConfigFilePath);

			config.Load(args.ConfigFilePath);

			var replacer = new ChainedVariablesReplacer(
				new ConfigurationNameVariablesReplacer(),
				new HelixConventionVariablesReplacer(),
				new ConfigPathVariableReplacer(Path.GetDirectoryName(args.ConfigFilePath)));

			var configObject = new LeprechaunConfigurationBuilder(replacer, config.DocumentElement["configurations"], config.DocumentElement["defaults"], config.DocumentElement["shared"], args.ConfigFilePath, new ConfigurationImportPathResolver(new ConsoleLogger()));

			// configure Rainbow
			RainbowSettings.Current = (RainbowSettings)configObject.Shared.Resolve<ILeprechaunRainbowSettings>();

			return configObject;
		}

		internal static string EnsureAbsoluteConfigPath(string path)
		{
			// if the config file isn't specified, return the app-relative Leprechaun.config file
			if (string.IsNullOrEmpty(path))
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Leprechaun.config");

			// If it's a relative path, merge the application root with the provided config file path
			if (!Path.IsPathRooted(path))
				return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));

			// If it's a rooted path, return it
			return path;
		}
	}
}
