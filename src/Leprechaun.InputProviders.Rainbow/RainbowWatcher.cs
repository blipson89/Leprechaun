using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Leprechaun.Configuration;
using Leprechaun.Execution;
using Leprechaun.Logging;
using Rainbow.Model;
using Rainbow.Storage;

namespace Leprechaun.InputProviders.Rainbow
{
	public class RainbowWatcher : IWatcher
	{
		public void Watch(LeprechaunConfigurationBuilder configuration, ILogger logger, Action rebuildAction)
		{
			const int debounceInMs = 500;
			Timer debouncer = new Timer(Rebuild);

			void RebuildFromItemChange(IItemMetadata item, string database)
			{
				logger.Info($"{item?.SerializedItemId ?? "Unknown"} was altered.");
				debouncer.Change(debounceInMs, Timeout.Infinite);
			}

			void RebuildFromConfigChange(string configPath, TreeWatcher.TreeWatcherChangeType changeType)
			{
				if (changeType == TreeWatcher.TreeWatcherChangeType.Delete)
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
				config.Resolve<IDataStore>().RegisterForChanges(RebuildFromItemChange);
			}

			foreach (var configPath in configuration.ImportedConfigFilePaths)
			{
				new TreeWatcher(Path.GetDirectoryName(configPath), Path.GetFileName(configPath), RebuildFromConfigChange);
			}
		}
	}
}
