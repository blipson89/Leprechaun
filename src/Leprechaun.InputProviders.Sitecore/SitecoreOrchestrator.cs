using System.Collections.Generic;
using System.Linq;
using Configy.Containers;
using Leprechaun.Filters;
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

		public SitecoreOrchestrator(ITemplateMetadataGenerator metadataGenerator, 
			IArchitectureValidator architectureValidator)
		{
			_metadataGenerator = metadataGenerator;
			_architectureValidator = architectureValidator;
		}

		public virtual IReadOnlyList<ConfigurationCodeGenerationMetadata> GenerateMetadata(params IContainer[] configurations)
		{
			var templates = GetAllTemplates(configurations).ToArray();

			FilterIgnoredFields(templates);
			var metadata = _metadataGenerator.Generate(templates);
			var allTemplatesMetadata = metadata.SelectMany(config => config.Metadata).ToArray();

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
			var templateReader = configuration.Resolve<ITemplateReader>();
			var templatePredicate = configuration.Resolve<ITemplatePredicate>();
			
			Assert.IsNotNull(templateReader, "templateReader != null");
			Assert.IsNotNull(templatePredicate, "templatePredicate != null");

			return templateReader.GetTemplates(templatePredicate);
		}

		protected virtual void FilterIgnoredFields(IEnumerable<TemplateConfiguration> configurations)
		{
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
