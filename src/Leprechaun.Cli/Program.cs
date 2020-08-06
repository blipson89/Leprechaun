using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Loader;
using Leprechaun.Execution;

namespace Leprechaun.Cli
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

			AssemblyLoadContext.Default.Resolving += (context, name) =>
			{
				var pluginDir = ResolvePluginPath(parsedArgs.PluginPath);
				// Try to load from the plugins directory. Else, fall back to the app directory
				if (File.Exists(Path.Combine(pluginDir, $"{name.Name}.dll")))
					return context.LoadFromAssemblyPath(Path.Combine(pluginDir, $"{name.Name}.dll"));
				return context.LoadFromAssemblyPath(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{name.Name}.dll")));
			};

			var appRunTimer = new Stopwatch();
			appRunTimer.Start();

			var runner = new Runner();
			try
			{
				runner.Run(parsedArgs);
			}
			catch (FileNotFoundException ex)
			{
				new ConsoleLogger().Error(ex);
				Environment.Exit(1);
			}

			appRunTimer.Stop();

			if (parsedArgs.NoExit)
			{
				Console.ReadKey();
			}
		}
	
		public static string ResolvePluginPath(string path)
		{
			if (string.IsNullOrEmpty(path))
				return AppDomain.CurrentDomain.BaseDirectory;

			if (!Path.IsPathRooted(path))
			{
				path = Path.Combine(Directory.GetCurrentDirectory(), path);
			}

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return path;
		}
	}
}
