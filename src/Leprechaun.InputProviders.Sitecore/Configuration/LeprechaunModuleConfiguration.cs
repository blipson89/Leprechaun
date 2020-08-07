using Sitecore.DevEx.Serialization.Client.Configuration;
using Sitecore.DevEx.Serialization.Client.Datasources.Filesystem.Query;

namespace Leprechaun.InputProviders.Sitecore.Configuration
{
	public class LeprechaunModuleConfiguration
	{
		public SerializationModuleConfiguration SerializationModule { get; }
		public FilesystemTreeDataStore DataStore { get; }

		public LeprechaunModuleConfiguration(
			SerializationModuleConfiguration serializationModule,
			FilesystemTreeDataStore dataStore)
		{
			SerializationModule = serializationModule;
			DataStore = dataStore;
		}
	}
}