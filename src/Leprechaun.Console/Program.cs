using System;
using System.Diagnostics;
using Leprechaun.Execution;

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

			var appRunTimer = new Stopwatch();
			appRunTimer.Start();

			var runner = new Runner();
			runner.Run(parsedArgs);

			appRunTimer.Stop();

			if (parsedArgs.NoExit)
			{
				System.Console.ReadKey();
			}
		}
	}
}
