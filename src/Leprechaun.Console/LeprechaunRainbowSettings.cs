using Rainbow.Settings;

namespace Leprechaun.Console
{
	// overrides Rainbow to get its settings from Leprechaun's settings instead of Rainbow.config
	public class LeprechaunRainbowSettings : RainbowSettings, ILeprechaunRainbowSettings
	{
		public LeprechaunRainbowSettings(int serializationFolderPathMaxLength, int maxItemNameLengthBeforeTruncation)
		{
			SfsSerializationFolderPathMaxLength = serializationFolderPathMaxLength;
			SfsMaxItemNameLengthBeforeTruncation = maxItemNameLengthBeforeTruncation;
		}

		public override int SfsMaxItemNameLengthBeforeTruncation { get; }

		public override int SfsSerializationFolderPathMaxLength { get; }
	}

	// this interface is just here so that Configy will register the type with the container (it registers only interfaces)
	public interface ILeprechaunRainbowSettings
	{

	}
}
