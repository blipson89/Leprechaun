using System;
using System.Diagnostics;
using System.IO;
using Leprechaun.Logging;
using Rainbow.Model;
using Rainbow.Storage;

namespace Leprechaun.Console
{
	public static class Watcher
	{
		public static void Watch(LeprechaunConfigurationBuilder configuration, ILogger logger, Action rebuildAction)
		{
			void RebuildFromItemChange(IItemMetadata item, string database)
			{
				logger.Info($"{item.SerializedItemId} was changed.");
				Rebuild();
			}

			void RebuildFromConfigChange(string configPath, TreeWatcher.TreeWatcherChangeType changeType)
			{
				if (changeType == TreeWatcher.TreeWatcherChangeType.Delete)
				{
					logger.Error($"Config file {configPath} was deleted. Terminating watch.");
					Environment.Exit(1);
				}

				logger.Info($"{configPath} was changed.");
				Rebuild();
			}

			void Rebuild()
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
