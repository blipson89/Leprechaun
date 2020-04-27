using Leprechaun.Configuration;
using Leprechaun.InputProviders.Rainbow.Modules;
using Rainbow.Settings;

namespace Leprechaun.InputProviders.Rainbow
{
	public class RainbowInputProvider : IInputProvider
	{
		public void Initialize(LeprechaunConfigurationBuilder config)
		{
			// configure Rainbow
			RainbowSettings.Current = (RainbowSettings)config.Shared.Resolve<ILeprechaunRainbowSettings>();
		}
	}
}
