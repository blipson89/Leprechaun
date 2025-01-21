using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leprechaun.InputProviders.Sitecore.Extensions;
using Microsoft.Extensions.Logging;
using Sitecore.DevEx.Configuration;
using Sitecore.DevEx.Serialization.Client.Configuration;
using Sitecore.DevEx.Serialization.Client.Datasources.Filesystem.Configuration;
using ILogger = Leprechaun.Logging.ILogger;

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
		private readonly ILogger _leprechaunLogger;
		private readonly string _configRootDirectory;

		public ModuleConfigurationReader(
			IRootConfigurationManager rootConfigurationManager,
			ISerializationModuleConfigurationManager serializationConfigurationManager,
			ILoggerFactory loggerFactory,
			ILogger leprechaunLogger,
			string configRootDirectory)
		{
			_rootConfigurationManager = rootConfigurationManager;
			_serializationConfigurationManager = serializationConfigurationManager;
			_loggerFactory = loggerFactory;
			_leprechaunLogger = leprechaunLogger;
			_configRootDirectory = configRootDirectory;
		}

		private static IDictionary<string, SerializationModuleConfiguration> _modules;
		public IDictionary<string, SerializationModuleConfiguration> GetModules()
		{
			if (_modules != null) return _modules;
			var resolveRootConfiguration = Task.Run(async () => await _rootConfigurationManager.ResolveRootConfiguration(_configRootDirectory));
			try
			{
				resolveRootConfiguration.Wait();
			}
			catch (AggregateException ex)
			{
				if (ex.Message.Contains("Couldn't resolve a root configuration"))
				{
					_leprechaunLogger.Error("[ERROR] The path to the sitecore.json file could not be resolved. Check the 'configRootDirectory' property on the 'moduleConfigReader' in your Leprechaun.config.", ex);
				}
				else if (ex.Message.Contains("was missing Authority"))
				{
					_leprechaunLogger.Error("[ERROR] Sitecore CLI failed validation. The nature of this error indicates you are likely using XM Cloud." +
						"\n\nPlease check your .sitecore/user.json file and ensure the \"authority\" property is present for every endpoint. The value should be \"https://auth.sitecorecloud.io/\" for XM Cloud.", ex);
				}
				else
				{
					_leprechaunLogger.Error("[ERROR] Sitecore CLI threw an error while attempting to gather the module. This likely indicates an issue with your Sitecore CLI configuration.", ex);
				}

				Environment.Exit(1);
			}
   
			var moduleConfigurations = Task.Run(async () => await _serializationConfigurationManager.ReadSerializationConfiguration(resolveRootConfiguration.Result, new ModuleGlobResolver(_loggerFactory.CreateLogger<ModuleGlobResolver>(), new ExternalPackageResolver(_loggerFactory)))); // TODO
			moduleConfigurations.Wait(); 
			var result = moduleConfigurations.Result;
			foreach(var configuration in result)
			{
				List<string> modules = configuration.GetLeprechaunModuleName();
				foreach(string module in modules)
				{
					_modules.Add(module, configuration);
				}
			}
			return _modules;
		}
	}
}
