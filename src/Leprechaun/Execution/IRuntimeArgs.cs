namespace Leprechaun.Execution
{
	public interface IRuntimeArgs
	{
		string ConfigFilePath { get; set; }

		bool Watch { get; set; }

		bool Help { get; set; }

		bool NoExit { get; set; }

		bool NoSplash { get; set; }
		bool Legacy { get; set; }
		string PluginPath { get; set; }
	}
}
