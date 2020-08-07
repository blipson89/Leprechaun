using Rainbow.Settings;

namespace Leprechaun.InputProviders.Rainbow.Modules
{
	// overrides Rainbow to get its settings from Leprechaun's settings instead of Rainbow.config
	public class RainbowSettingsModule : RainbowSettings, ILeprechaunRainbowSettings
	{
		public RainbowSettingsModule(int serializationFolderPathMaxLength, int maxItemNameLengthBeforeTruncation)
		{
			SfsSerializationFolderPathMaxLength = serializationFolderPathMaxLength;
			SfsMaxItemNameLengthBeforeTruncation = maxItemNameLengthBeforeTruncation;

			RainbowSettings.Current = this;
		}

		public override int SfsMaxItemNameLengthBeforeTruncation { get; }

		public override int SfsSerializationFolderPathMaxLength { get; }
	}

	// this interface is just here so that Configy will register the type with the container (it registers only interfaces)
	public interface ILeprechaunRainbowSettings
	{

	}
}
