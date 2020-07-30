using System;
using System.Diagnostics;
using System.IO;
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

			var appRunTimer = new Stopwatch();
			appRunTimer.Start();

			var runner = new Runner();
			try
			{
				runner.Run(parsedArgs);
			}
			catch(FileNotFoundException ex)
			{
				WriteError(ex);
				Environment.Exit(1);
			}

			appRunTimer.Stop();

			if (parsedArgs.NoExit)
			{
				System.Console.ReadKey();
			}
		}

		static void WriteError(Exception ex)
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"ERROR: {ex.Message}");
			Console.WriteLine(ex.StackTrace);
			Console.ForegroundColor = oldColor;
		}
    }
}
