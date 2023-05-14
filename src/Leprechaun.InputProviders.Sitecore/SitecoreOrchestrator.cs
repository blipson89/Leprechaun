﻿using System.Collections.Generic;
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
			var templateReader = configuration.Resolve<ITemplateReader>();
			var templatePredicate = configuration.Resolve<ITemplatePredicate>();
			Assert.IsNotNull(templateReader, "templateReader != null");
			Assert.IsNotNull(templatePredicate, "templatePredicate != null");

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
