using System.Linq;
using System.Xml;
using Configy;
using Configy.Containers;
using Configy.Parsing;
using Leprechaun.Filters;
using Leprechaun.Logging;
using Leprechaun.MetadataGeneration;
using Leprechaun.TemplateReaders;
using Leprechaun.Validation;
using Rainbow.Storage;
using Sitecore.Diagnostics;

namespace Leprechaun
{
	public class LeprechaunConfigurationBuilder : XmlContainerBuilder
	{
		private readonly XmlElement _configsElement;
		private readonly XmlElement _baseConfigElement;
		private readonly XmlElement _sharedConfigElement;

		private IContainer _sharedConfig;
		private IContainer[] _configurations;

		public LeprechaunConfigurationBuilder(IContainerDefinitionVariablesReplacer variablesReplacer, XmlElement configsElement, XmlElement baseConfigElement, XmlElement sharedConfigElement) : base(variablesReplacer)
		{
			Assert.ArgumentNotNull(variablesReplacer, nameof(variablesReplacer));
			Assert.ArgumentNotNull(configsElement, nameof(configsElement));
			Assert.ArgumentNotNull(baseConfigElement, nameof(baseConfigElement));
			Assert.ArgumentNotNull(sharedConfigElement, nameof(sharedConfigElement));

			_configsElement = configsElement;
			_baseConfigElement = baseConfigElement;
			_sharedConfigElement = sharedConfigElement;
		}

		public virtual IContainer Shared
		{
			get
			{
				if(_sharedConfig == null) LoadSharedConfiguration();
				return _sharedConfig;
			}
		}

		public virtual IContainer[] Configurations
		{
			get
			{
				if (_configurations == null) LoadConfigurations();
				return _configurations;
			}
		}

		protected virtual void LoadConfigurations()
		{
			var parser = new XmlContainerParser(_configsElement, _baseConfigElement, new XmlInheritanceEngine());

			var definitions = parser.GetContainers();

			var configurations = GetContainers(definitions).ToArray();

			foreach (var configuration in configurations)
			{
				// Assert that expected dependencies exist - and in the case of data stores are specifically singletons (WEIRD things happen otherwise)
				configuration.AssertSingleton(typeof(IDataStore));
				configuration.AssertSingleton(typeof(IFieldFilter));
				configuration.AssertSingleton(typeof(ITemplateFilter));
				configuration.AssertSingleton(typeof(ITypeNameGenerator));
				configuration.AssertSingleton(typeof(ITemplateReader));
				

				// register the container with itself. how meta!
				configuration.Register(typeof(IContainer), () => configuration, true);
			}

			_configurations = configurations.ToArray();
		}

		protected virtual void LoadSharedConfiguration()
		{
			var definition = new ContainerDefinition(_sharedConfigElement);

			var sharedConfiguration = GetContainer(definition);

			// Assert that expected dependencies exist - and in the case of data stores are specifically singletons (WEIRD things happen otherwise)
			sharedConfiguration.AssertSingleton(typeof(ITemplateMetadataGenerator));
			sharedConfiguration.AssertSingleton(typeof(IArchitectureValidator));
			sharedConfiguration.Assert(typeof(ILogger));

			_sharedConfig = sharedConfiguration;
		}
	}
}
