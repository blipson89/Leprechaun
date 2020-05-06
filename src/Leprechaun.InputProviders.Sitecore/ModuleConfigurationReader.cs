using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.DevEx.Configuration;
using Sitecore.DevEx.Serialization.Client.Configuration;
using Sitecore.DevEx.Serialization.Client.Datasources.Filesystem.Configuration;

namespace Leprechaun.InputProviders.Sitecore
{
	public interface IModuleConfigurationReader
	{
		IDictionary<string, SerializationModuleConfiguration> GetModules();
	}

	public class ModuleConfigurationReader : IModuleConfigurationReader
	{
		private readonly IRootConfigurationManager _rootConfigurationManager;
		private readonly ISerializationModuleConfigurationManager _serializationConfigurationManager;
		private readonly ILoggerFactory _loggerFactory;
		private readonly string _configRootDirectory;

		public ModuleConfigurationReader(
			IRootConfigurationManager rootConfigurationManager,
			ISerializationModuleConfigurationManager serializationConfigurationManager,
			ILoggerFactory loggerFactory,
			string configRootDirectory)
		{
			_rootConfigurationManager = rootConfigurationManager;
			_serializationConfigurationManager = serializationConfigurationManager;
			_loggerFactory = loggerFactory;
			_configRootDirectory = configRootDirectory;
		}

		private static IDictionary<string, SerializationModuleConfiguration> _modules;
		public IDictionary<string, SerializationModuleConfiguration> GetModules()
		{
			if (_modules != null) return _modules;
			var resolveRootConfiguration = Task.Run(async () => await _rootConfigurationManager.ResolveRootConfiguration(_configRootDirectory));
			resolveRootConfiguration.Wait();


			var moduleConfigurations = Task.Run(async () => await _serializationConfigurationManager.ReadSerializationConfiguration(resolveRootConfiguration.Result,
				new ModuleGlobResolver(_loggerFactory.CreateLogger<ModuleGlobResolver>()))); // TODO
			moduleConfigurations.Wait();
			
			return _modules = moduleConfigurations.Result.ToDictionary(m => m.Namespace);
		}
	}
}
