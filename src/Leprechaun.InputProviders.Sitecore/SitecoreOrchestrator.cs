using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using Configy.Containers;
using Leprechaun.Filters;
using Leprechaun.Logging;
using Leprechaun.MetadataGeneration;
using Leprechaun.Model;
using Leprechaun.TemplateReaders;
using Leprechaun.Validation;

namespace Leprechaun.InputProviders.Sitecore
{
	public class SitecoreOrchestrator : IOrchestrator
	{
		private readonly ITemplateMetadataGenerator _metadataGenerator;
		private readonly IArchitectureValidator _architectureValidator;
		private readonly ILogger _logger;

		public SitecoreOrchestrator(ITemplateMetadataGenerator metadataGenerator, 
			IArchitectureValidator architectureValidator,
			ILogger logger)
		{
			_metadataGenerator = metadataGenerator;
			_architectureValidator = architectureValidator;
			_logger = logger;
		}

		public virtual IReadOnlyList<ConfigurationCodeGenerationMetadata> GenerateMetadata(params IContainer[] configurations)
		{
			_logger?.Debug("[SitecoreOrchestrator] GenerateMetadata - A");
			var templates = GetAllTemplates(configurations).ToArray();
			_logger?.Debug("[SitecoreOrchestrator] GenerateMetadata - B");

			FilterIgnoredFields(templates);
			_logger?.Debug("[SitecoreOrchestrator] GenerateMetadata - C");
			var metadata = _metadataGenerator.Generate(templates);
			_logger?.Debug("[SitecoreOrchestrator] GenerateMetadata - D");
			var allTemplatesMetadata = metadata.SelectMany(config => config.Metadata).ToArray();
			_logger?.Debug("[SitecoreOrchestrator] GenerateMetadata - E");
			_architectureValidator.Validate(allTemplatesMetadata);

			return metadata;
		}

		protected virtual IEnumerable<TemplateConfiguration> GetAllTemplates(IEnumerable<IContainer> configurations)
		{
			var results = new List<TemplateConfiguration>();

			foreach (IContainer configuration in configurations)
			{
				var processingConfig = new TemplateConfiguration(configuration)
				{
					Templates = GetTemplates(configuration)
				};
				results.Add(processingConfig);
			}
			return results.ToArray();
		}

		protected virtual IEnumerable<TemplateInfo> GetTemplates(IContainer configuration)
		{
			_logger?.Debug($"[SitecoreOrchestrator] GetTemplates - {configuration.Name}");
			try
			{
				var singletonsField = configuration.GetType().GetField("_singletons", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				var singletons = ((ConcurrentDictionary<Type, Lazy<object>>)singletonsField.GetValue(configuration)).Select(x => x.Key.FullName);
				_logger.Debug("====== DI Registration ======");
				foreach (string s in singletons)
				{
					_logger.Debug($"- {s}");
				}
				_logger.Debug("=====  END DI Registration ======");
			}
			catch (Exception) { }

			var templateReader = configuration.Resolve<ITemplateReader>();
			_logger?.Debug($"[SitecoreOrchestrator] GetTemplates - Template Reader Resolved Successfully!");
			Assert.IsNotNull(templateReader, "templateReader != null");
			_logger?.Debug($"[SitecoreOrchestrator] GetTemplates - Template Reader is not null! Type: {templateReader.GetType().FullName}");
			var templatePredicate = configuration.Resolve<ITemplatePredicate>();
			_logger?.Debug($"[SitecoreOrchestrator] GetTemplates - Template Predicate Resolved Successfully!");
			Assert.IsNotNull(templatePredicate, "templatePredicate != null");
			_logger?.Debug($"[SitecoreOrchestrator] GetTemplates - Template Predicate Is not null! Type: {templatePredicate.GetType().FullName}");

			return templateReader.GetTemplates(templatePredicate);
		}

		protected virtual void FilterIgnoredFields(IEnumerable<TemplateConfiguration> configurations)
		{
			_logger?.Debug("[SitecoreOrchestrator] FilterIgnoredFields");
			foreach (var configuration in configurations)
			{
				var filter = configuration.Configuration.Resolve<IFieldFilter>();

				Assert.IsNotNull(filter, "filter != null");

				foreach (var template in configuration.Templates)
				{
					template.OwnFields = template.OwnFields.Where(field => filter.Includes(field)).ToArray();
				}
			}
		}
	}

}
