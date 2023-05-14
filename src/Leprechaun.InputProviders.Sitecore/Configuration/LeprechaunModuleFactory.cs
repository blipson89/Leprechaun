using System.Collections.Generic;
using System.Threading.Tasks;
using Leprechaun.Execution;
using Microsoft.Extensions.Logging;
using Sitecore.DevEx.Serialization.Client.Configuration;
using Sitecore.DevEx.Serialization.Client.Datasources.Filesystem.Configuration;
using Sitecore.DevEx.Serialization.Client.Datasources.Filesystem.Formatting;
using Sitecore.DevEx.Serialization.Client.Datasources.Filesystem.Query;

namespace Leprechaun.InputProviders.Sitecore.Configuration
{
	public class LeprechaunModuleFactory
	{
		private readonly ISerializationFormatter _serializationFormatter;
		private readonly ILoggerFactory _loggerFactory;
		private readonly Logging.ILogger _leprechaunLogger;

		public LeprechaunModuleFactory(ISerializationFormatter serializationFormatter, ILoggerFactory loggerFactory, Leprechaun.Logging.ILogger leprechaunLogger)
		{
			_serializationFormatter = serializationFormatter;
			_loggerFactory = loggerFactory;
			_leprechaunLogger = leprechaunLogger;
		}
		public async Task<LeprechaunModuleConfiguration> GetModule(SerializationModuleConfiguration moduleConfiguration)
		{
			var ds = new FilesystemTreeDataStore((IReadOnlyList<FilesystemTreeSpec>)moduleConfiguration.Items.Includes, _serializationFormatter, _loggerFactory.CreateLogger<FilesystemTreeDataStore>(), false);
			await ds.Initialize();
			_leprechaunLogger.Debug($"[LeprechaunModuleFactory] GetModule executed");

			return new LeprechaunModuleConfiguration(moduleConfiguration, ds);
		}

		public LeprechaunModuleConfiguration GetModuleSync(SerializationModuleConfiguration moduleConfiguration)
		{
			_leprechaunLogger.Debug($"[LeprechaunModuleFactory] GetModuleSync executed");
			return Task.Run(async () => await GetModule(moduleConfiguration)).GetAwaiter().GetResult();
		}
	}
}