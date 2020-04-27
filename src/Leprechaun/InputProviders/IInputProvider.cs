using Leprechaun.Configuration;

namespace Leprechaun.InputProviders
{
	public interface IInputProvider
	{
		void Initialize(LeprechaunConfigurationBuilder config);
	}
}
