using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Configy;
using Configy.Containers;
using Configy.Parsing;
using Leprechaun.CodeGen;
using Leprechaun.Execution;
using Leprechaun.Filters;
using Leprechaun.InputProviders;
using Leprechaun.Logging;
using Leprechaun.MetadataGeneration;
using Leprechaun.RenderingReaders;
using Leprechaun.TemplateReaders;
using Leprechaun.Validation;
using Newtonsoft.Json;

namespace Leprechaun.Configuration
{
	public class LeprechaunConfigurationBuilder : XmlContainerBuilder
	{
		private readonly XmlElement _configsElement;
		private readonly XmlElement _baseConfigElement;
		private readonly XmlElement _sharedConfigElement;
		private readonly string _configFilePath;
		private readonly ConfigurationImportPathResolver _configImportResolver;

		private IContainer _sharedConfig;
		private IContainer[] _configurations;

		public LeprechaunConfigurationBuilder(IContainerDefinitionVariablesReplacer variablesReplacer, XmlElement configsElement, XmlElement baseConfigElement, XmlElement sharedConfigElement, string configFilePath, ConfigurationImportPathResolver configImportResolver) : base(variablesReplacer)
		{
			Assert.ArgumentNotNull(variablesReplacer, nameof(variablesReplacer));
			Assert.ArgumentNotNull(configsElement, nameof(configsElement));
			Assert.ArgumentNotNull(baseConfigElement, nameof(baseConfigElement));
			Assert.ArgumentNotNull(sharedConfigElement, nameof(sharedConfigElement));

			_configsElement = configsElement;
			_baseConfigElement = baseConfigElement;
			_sharedConfigElement = sharedConfigElement;
			_configFilePath = configFilePath;
			_configImportResolver = configImportResolver;

			ProcessImports();
			InitializeInputProvider();
		}

		public virtual IContainer Shared
		{
			get
			{
				if (_sharedConfig == null) LoadSharedConfiguration();
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

		public string[] ImportedConfigFilePaths { get; protected set; }

		protected virtual void LoadConfigurations()
		{
			var parser = new XmlContainerParser(_configsElement, _baseConfigElement, new XmlInheritanceEngine());

			var definitions = parser.GetContainers();

			var configurations = GetContainers(definitions).ToArray();

			foreach (var configuration in configurations)
			{
				// Assert that expected dependencies exist - and in the case of data stores are specifically singletons (WEIRD things happen otherwise)
				configuration.AssertSingleton(typeof(IFieldFilter));
				configuration.AssertSingleton(typeof(ITemplatePredicate));
				configuration.AssertSingleton(typeof(ITypeNameGenerator));
				configuration.AssertSingleton(typeof(ITemplateReader));
				configuration.AssertSingleton(typeof(IRenderingReader));
				configuration.Assert(typeof(ICodeGenerator));

				// register the container with itself. how meta!
				configuration.Register(typeof(IContainer), () => configuration, true);
			}

			_configurations = configurations.ToArray();
		}

		protected virtual void LoadSharedConfiguration()
		{
			var definition = new ContainerDefinition(_sharedConfigElement);

			try
			{
				var sharedConfiguration = GetContainer(definition);

				// Assert that expected dependencies exist - and in the case of data stores are specifically singletons (WEIRD things happen otherwise)
				sharedConfiguration.AssertSingleton(typeof(ITemplateMetadataGenerator));
				sharedConfiguration.AssertSingleton(typeof(IArchitectureValidator));
				sharedConfiguration.AssertSingleton(typeof(IInputProvider));
				sharedConfiguration.Assert(typeof(ILogger));

				_sharedConfig = sharedConfiguration;
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.Contains("Leprechaun.InputProviders.Rainbow"))
				{
					new ConsoleLogger().Error("Unable to resolve dependency. If you are using Rainbow, ensure you have the /r switch in the command line arguments.", ex);
				}
				else
				{
					new ConsoleLogger().Error($"{ex.Message}.\n\nCheck your configuration files to make sure it is typed correctly. " +
											$"If so, ensure that the dlls are in the correct location. " +
											$"You may need to use the /p switch to specify this location", ex);
				}
				Environment.Exit(1);
			}
		}

		public void InitializeInputProvider()
		{
			Shared.Resolve<IInputProvider>().Initialize(this);
		}

		protected virtual void ProcessImports()
		{
			var imports = _configsElement.Attributes["import"]?.InnerText;

			if (imports == null) return;

			var allImportsGlobs = imports.Split(';');

			var allImportsRepathedGlobs = allImportsGlobs.Select(glob =>
			{
				// fix issues if "; " is used as a separator
				glob = glob.Trim();

				// absolute path with drive letter, so use the path raw
				if (glob[0] == ':') return glob;

				// relative path (absolutize with root config file path as base)
				return Path.Combine(Path.GetDirectoryName(_configFilePath), glob);
			});

			var allImportsFiles = allImportsRepathedGlobs
				.SelectMany(glob => _configImportResolver.ResolveImportPaths(glob))
				.Concat(new[] {_configFilePath})
				.ToList();
			Queue<string> configsToProcess = new Queue<string>(allImportsFiles);
			while(configsToProcess.Count > 0)
			{
				var import = configsToProcess.Dequeue();
				var xml = new XmlDocument();
				XmlElement nodeToImport;
				if (Path.GetExtension(import) == ".json")
				{
					var xmlConfig = JsonConvert.DeserializeXmlNode(File.ReadAllText(import), "module");
					nodeToImport = (XmlElement)xmlConfig.SelectSingleNode("/module/leprechaun")?.FirstChild;
					if (nodeToImport == null)
					{
						allImportsFiles.Remove(import);
						continue;
					}

					if (!nodeToImport.HasAttribute("name"))
					{
						var namespaceUri = xmlConfig.SelectSingleNode("/module/namespace")?.InnerText;
						if (string.IsNullOrEmpty(namespaceUri))
						{
							throw new InvalidOperationException($"module does not have namespace: '{import}'");
						}
						nodeToImport.SetAttribute("name", namespaceUri);
					}
				}
				else
				{
					xml.Load(import);
					nodeToImport = xml.DocumentElement;
				}

				var importedXml = _baseConfigElement.OwnerDocument.ImportNode(nodeToImport, true);

				_configsElement.AppendChild(importedXml);

			}

			// we'll use this to watch imports for changes later
			ImportedConfigFilePaths = allImportsFiles.ToArray();
		}
	}
}
