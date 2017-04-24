using CommandLineParser.Arguments;

namespace Leprechaun.Console
{
	class ConsoleArgs
	{
		[ValueArgument(typeof(string), 'c', "config", Description = "Path to the config file to use", DefaultValue = "Leprechaun.config")]
		public string ConfigFilePath { get; set; }

		[SwitchArgument('w', "watch", false, Description = "If set, source files are watched for changes and automatically rebuilt.")]
		public bool Watch { get; set; }

		[SwitchArgument('h', "help", false, Description = "Prints this message")]
		public bool Help { get; set; }

		[SwitchArgument('n', "noexit", false, Description = "If set, the program will not exit after completion. Mostly for debugging purposes.")]
		public bool NoExit { get; set; }
	}
}
