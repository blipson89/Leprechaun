using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Configy.Parsing;
using Leprechaun.CodeGen;
using Leprechaun.Configuration;
using Leprechaun.Logging;
using Leprechaun.Model;
using Leprechaun.Variables;
using Newtonsoft.Json;

namespace Leprechaun.Execution
{
	public class Runner
	{
		public void Run(IRuntimeArgs parsedArgs)
		{
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
			var orchestrator = configuration.Shared.Resolve<IOrchestrator>();
			if (parsedArgs.Debug)
			{
				(configuration.Shared.Resolve<ILogger>() as ConsoleLogger)?.SetDebug(parsedArgs.Debug);
			}

			var metadata = GenerateMetadata(orchestrator, configuration);

			// make sure we're done preloading the compiled codegen templates
			preload.Wait();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Code generator has loaded in {appRunTimer.ElapsedMilliseconds}ms.");
			Console.ResetColor();

			GenerateCode(metadata);

			if (parsedArgs.Watch)
			{
				IWatcher watcher = configuration.Shared.Resolve<IWatcher>();
				if (watcher == null)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Unable to watch because no IWatcher was defined!");
				}
				else
				{
					Console.WriteLine();
					Console.WriteLine("Leprechaun is now watching for file changes and rebuilding at need.");
					Console.WriteLine("Press Ctrl-C to exit.");
					watcher.Watch(configuration, new ConsoleLogger(), () => GenerateWatch(orchestrator, configuration));
					var exit = new ManualResetEvent(false);
					exit.WaitOne();
				}
			}

			appRunTimer.Stop();
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Leprechaun has completed in {appRunTimer.ElapsedMilliseconds}ms.");
			Console.ResetColor();
		}

		private void GenerateWatch(IOrchestrator orchestrator, LeprechaunConfigurationBuilder configuration)
		{
			try
			{
				var metadata = GenerateMetadata(orchestrator, configuration);
				GenerateCode(metadata);
			}
			catch (Exception ex)
			{
				// during watch we don't want errors to terminate the application
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine(ex.StackTrace);
				Console.ResetColor();
			}
		}

		private void GenerateCode(IReadOnlyList<ConfigurationCodeGenerationMetadata> metadata)
		{
			// emit actual code using the codegens for each config
			foreach (var meta in metadata)
			{
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.Cyan;
				var word = meta.Metadata.Count == 1 ? "template" : "templates";
				Console.WriteLine($"> Generating {meta.Configuration.Name} ({meta.Metadata.Count} {word})");
				Console.ResetColor();

				var codeGen = meta.Configuration.Resolve<ICodeGenerator>();
				codeGen.GenerateCode(meta);
			}
		}

		private IReadOnlyList<ConfigurationCodeGenerationMetadata> GenerateMetadata(IOrchestrator orchestrator, LeprechaunConfigurationBuilder configuration)
		{
			var metadataTimer = new Stopwatch();
			metadataTimer.Start();

			// we generate template data that will feed code generation
			var metadata = orchestrator.GenerateMetadata(configuration.Configurations);

			metadataTimer.Stop();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(
				$"Loaded metadata for {metadata.Count} configurations ({metadata.Sum(m => m.Metadata.Count)} total templates) in {metadataTimer.ElapsedMilliseconds}ms.");
			Console.ResetColor();

			return metadata;
		}

		private IContainerDefinitionVariablesReplacer GetVariablesReplacer(IRuntimeArgs args)
		{
			return new ChainedVariablesReplacer(
				new ConfigurationNameVariablesReplacer(),
				new HelixConventionVariablesReplacer(),
				new ConfigPathVariableReplacer(Path.GetDirectoryName(args.ConfigFilePath)));
		}

		private LeprechaunConfigurationBuilder BuildConfiguration(IRuntimeArgs args)
		{
			XmlDocument config = new XmlDocument();
			args.ConfigFilePath = EnsureAbsoluteConfigPath(args.ConfigFilePath);
			if (Path.GetExtension(args.ConfigFilePath) == ".json")
				config = JsonConvert.DeserializeXmlNode(File.ReadAllText(args.ConfigFilePath));
			else
				config.Load(args.ConfigFilePath);
			var replacer = GetVariablesReplacer(args);

			XmlElement configsElement = config.DocumentElement["configurations"];
			XmlElement baseConfigElement = config.DocumentElement["defaults"];
			XmlElement sharedConfigElement = config.DocumentElement["shared"];
			var configObject = new LeprechaunConfigurationBuilder(replacer,
				configsElement, 
				baseConfigElement, 
				sharedConfigElement, 
				args.ConfigFilePath,
				args.Include,
				args.Exclude,
				new ConfigurationImportPathResolver(new ConsoleLogger()));

			return configObject;
		}

		internal static string EnsureAbsoluteConfigPath(string path)
		{
			// if the config file isn't specified, return the app-relative Leprechaun.config file
			if (string.IsNullOrEmpty(path))
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Leprechaun.config");

			// If it's a relative path, merge the application root with the provided config file path
			if (!Path.IsPathRooted(path))
			{
				string exeRelativePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
				if (File.Exists(exeRelativePath))
					return exeRelativePath;
				string dirRelativePath = Path.Combine(Directory.GetCurrentDirectory(), path);
				if (File.Exists(dirRelativePath))
					return dirRelativePath;

				throw new FileNotFoundException($"Unable to find relative config file in the following paths: '{exeRelativePath}' or '{dirRelativePath}'");
			}

			// If it's a rooted path, return it
			return path;
		}
	}
}
