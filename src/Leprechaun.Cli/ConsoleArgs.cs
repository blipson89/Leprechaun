using CommandLineParser.Arguments;
using Leprechaun.Execution;

namespace Leprechaun.Cli
{
	class ConsoleArgs : IRuntimeArgs
	{
		[ValueArgument(typeof(string), 'c', "config", Description = "Path to the config file to use. Defaults to Leprechaun.config.")]
		public string ConfigFilePath { get; set; }

		[SwitchArgument('w', "watch", false, Description = "If set, source files are watched for changes and automatically rebuilt.")]
		public bool Watch { get; set; }

		[SwitchArgument('h', "help", false, Description = "Prints this message")]
		public bool Help { get; set; }

		[SwitchArgument('n', "noexit", false, Description = "If set, the program will not exit after completion until a key is pressed. Mostly for debugging purposes.")]
		public bool NoExit { get; set; }

		[SwitchArgument('g', "grumpycat", false, Description = "Disables the ASCII art splash screen. You hate fun.")]
		public bool NoSplash { get; set; }

		[SwitchArgument('r', "rainbow", false, Description = "Use legacy console app. Required for input providers that don't support .NET Core (like Rainbow)")]
		public bool Rainbow { get; set; }

		[ValueArgument(typeof(string), 'p', "plugins", Description = "Path to the folder where plugins will be located. ./Leprechaun")]
		public string PluginPath { get; set; }
		
		[SwitchArgument('d', "debug", false, Description = "Prints out full stack traces for exceptions")]
		public bool Debug { get; set; }
	}
}
