using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Configy.Containers;
using Leprechaun.Filters;
using Leprechaun.InputProviders.Sitecore.Configuration;
using Leprechaun.Logging;
using Sitecore.DevEx.Serialization.Client.Datasources.Filesystem.Configuration;

namespace Leprechaun.InputProviders.Sitecore.Filters
{
	public class SitecoreTemplatePredicate : BaseTemplatePredicate
	{
		private readonly IModuleConfigurationReader _moduleConfigurationReader;
		private readonly LeprechaunModuleFactory _leprechaunModuleFactory;
		private readonly ILogger _logger;
		private LeprechaunModuleConfiguration _leprechaunModule;

		public SitecoreTemplatePredicate(XmlNode configNode, IContainer configuration, IModuleConfigurationReader moduleConfigurationReader, LeprechaunModuleFactory leprechaunModuleFactory, ILogger logger, string rootNamespace)
		: base(configNode, configuration, rootNamespace)
		{
			_moduleConfigurationReader = moduleConfigurationReader;
			_leprechaunModuleFactory = leprechaunModuleFactory;
			_logger = logger;
			LoadModule();

		}
		public void LoadModule()
		{
			_logger.Debug("[SitecoreTemplatePredicate] LoadModules - A");
			if (_moduleConfigurationReader.GetModules().ContainsKey(_configuration.Name))
			{
				_logger.Debug("[SitecoreTemplatePredicate] LoadModules - B");
				var serializationModule = _moduleConfigurationReader.GetModules()[_configuration.Name];
				_logger.Debug("[SitecoreTemplatePredicate] LoadModules - C");
				_leprechaunModule = _leprechaunModuleFactory.GetModuleSync(serializationModule);
				_logger.Debug("[SitecoreTemplatePredicate] LoadModules - D");
			}
		}

		public LeprechaunModuleConfiguration GetModule()
		{
			return _leprechaunModule;
		}

		public IEnumerable<FilesystemTreeSpec> GetTreeSpecs()
		{
			_logger.Debug("[SitecoreTemplatePredicate] GetTreeSpecs - A");
			foreach (var treeNode in _leprechaunModule.SerializationModule.Items.Includes)
			{
				_logger.Debug($"[SitecoreTemplatePredicate] GetTreeSpecs - B {treeNode.Name}");
				if (_includeNames.Contains(treeNode.Name.ToLowerInvariant()))
				{
					yield return treeNode;
				}
			}
		}

		public IEnumerable<string> GetWatchPaths()
		{
			return GetTreeSpecs().Select(x => x.PhysicalPath);
		}
	}
}
