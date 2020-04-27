using System;
using Leprechaun.Configuration;
using Leprechaun.Logging;

namespace Leprechaun.Execution
{
	public interface IWatcher
	{
		void Watch(LeprechaunConfigurationBuilder configuration, ILogger logger, Action rebuildAction);
	}
}
