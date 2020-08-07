using System;
using Leprechaun.Configuration;
using Leprechaun.Execution;
using Leprechaun.Logging;

namespace Leprechaun.InputProviders.Sitecore
{
	public class SitecoreWatcher : IWatcher
	{
		public void Watch(LeprechaunConfigurationBuilder configuration, ILogger logger, Action rebuildAction)
		{
			logger.Warn("Watch is currently not supported for Sitecore Serialization.");
		}
	}
}
