using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Leprechaun.Configuration;
using Leprechaun.Execution;
using Leprechaun.Filters;
using Leprechaun.InputProviders.Sitecore.Filters;
using Leprechaun.Logging;
using Sitecore.DevEx.Serialization.Client.Datasources.Filesystem;

namespace Leprechaun.InputProviders.Sitecore
{
	public class SitecoreWatcher : IWatcher
	{
		public void Watch(LeprechaunConfigurationBuilder configuration, ILogger logger, Action rebuildAction)
		{
			const int debounceInMs = 500;
			var debouncer = new Timer(Rebuild);

			void RebuildFromItemChange(string configPath, TreeWatcherChangeType changeType)
			{
				logger.Info($"{configPath} was altered.");
				debouncer.Change(debounceInMs, Timeout.Infinite);
			}

			void RebuildFromConfigChange(string configPath, TreeWatcherChangeType changeType)
			{
				if (changeType == TreeWatcherChangeType.Delete)
				{
					logger.Error($"Config file {configPath} was deleted. Terminating watch.");
					Environment.Exit(1);
				}

				logger.Info($"{configPath} was changed.");
				debouncer.Change(debounceInMs, Timeout.Infinite);
			}

			void Rebuild(object ignored)
			{
				var timer = new Stopwatch();
				timer.Start();

				rebuildAction();

				timer.Stop();
				logger.Info(string.Empty);
				logger.Info($"Regeneration complete in {timer.ElapsedMilliseconds}ms.");
			}

			foreach (var config in configuration.Configurations)
			{
				var predicate = config.Resolve<ITemplatePredicate>() as SitecoreTemplatePredicate;
				if (predicate == null)
					continue;
				foreach (var watchPath in predicate.GetWatchPaths())
				{
					var _ = new TreeWatcher(watchPath, "*.yml", RebuildFromItemChange);
				}
			}

			foreach (var configPath in configuration.ImportedConfigFilePaths)
			{
				var _ = new TreeWatcher(Path.GetDirectoryName(configPath), Path.GetFileName(configPath), RebuildFromConfigChange);
			}
		}
	}
}
