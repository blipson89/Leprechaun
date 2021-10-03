namespace Leprechaun.Execution
{
	public interface IRuntimeArgs
	{
		string ConfigFilePath { get; set; }

		bool Watch { get; set; }

		bool Help { get; set; }

		bool NoExit { get; set; }

		bool NoSplash { get; set; }
		bool Rainbow { get; set; }
		string PluginPath { get; set; }
		bool Debug { get; set; }
	}
}
