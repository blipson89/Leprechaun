﻿using System.Xml;
using Configy.Containers;
using Leprechaun.Filters;
using Leprechaun.InputProviders.Sitecore.Configuration;

namespace Leprechaun.InputProviders.Sitecore.Filters
{
	public class SitecoreTemplatePredicate : BaseTemplatePredicate
	{
		private readonly IModuleConfigurationReader _moduleConfigurationReader;
		private readonly LeprechaunModuleFactory _leprechaunModuleFactory;
		private LeprechaunModuleConfiguration _leprechaunModule;

		public SitecoreTemplatePredicate(XmlNode configNode, IContainer configuration, IModuleConfigurationReader moduleConfigurationReader, LeprechaunModuleFactory leprechaunModuleFactory, string rootNamespace)
		: base(configNode, configuration, rootNamespace)
		{
			_moduleConfigurationReader = moduleConfigurationReader;
			_leprechaunModuleFactory = leprechaunModuleFactory;

			LoadModule();

		}
		public void LoadModule()
		{
			if (_moduleConfigurationReader.GetModules().ContainsKey(_configuration.Name))
			{
				var serializationModule = _moduleConfigurationReader.GetModules()[_configuration.Name];
				_leprechaunModule = _leprechaunModuleFactory.GetModuleSync(serializationModule);
			}
		}

		public LeprechaunModuleConfiguration GetModule()
		{
			return _leprechaunModule;
		}
	}
}
