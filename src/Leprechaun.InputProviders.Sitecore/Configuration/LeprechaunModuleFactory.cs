using System.Collections.Generic;
using System.Threading.Tasks;
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

		public LeprechaunModuleFactory(ISerializationFormatter serializationFormatter, ILoggerFactory loggerFactory)
		{
			_serializationFormatter = serializationFormatter;
			_loggerFactory = loggerFactory;
		}
		public async Task<LeprechaunModuleConfiguration> GetModule(SerializationModuleConfiguration moduleConfiguration)
		{
			var ds = new FilesystemTreeDataStore((IReadOnlyList<FilesystemTreeSpec>)moduleConfiguration.Items.Includes, _serializationFormatter, _loggerFactory.CreateLogger<FilesystemTreeDataStore>(), false);
			await ds.Initialize();

			return new LeprechaunModuleConfiguration(moduleConfiguration, ds);
		}

		public LeprechaunModuleConfiguration GetModuleSync(SerializationModuleConfiguration moduleConfiguration)
		{
			return Task.Run(async () => await GetModule(moduleConfiguration)).GetAwaiter().GetResult();
		}
	}
}