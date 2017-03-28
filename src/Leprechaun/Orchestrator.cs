using System.Collections.Generic;
using System.Linq;
using Configy.Containers;
using Leprechaun.Filters;
using Leprechaun.MetadataGeneration;
using Leprechaun.Model;
using Leprechaun.TemplateReaders;
using Leprechaun.Validation;
using Sitecore.Diagnostics;

namespace Leprechaun
{
	public class Orchestrator
	{
		private readonly ITemplateMetadataGenerator _metadataGenerator;
		private readonly IArchitectureValidator _architectureValidator;

		public Orchestrator(ITemplateMetadataGenerator metadataGenerator, IArchitectureValidator architectureValidator)
		{
			_metadataGenerator = metadataGenerator;
			_architectureValidator = architectureValidator;
		}

		public virtual IReadOnlyList<ConfigurationCodeGenerationMetadata> GenerateMetadata(params IContainer[] configurations)
		{
			var templates = GetAllTemplates(configurations);

			FilterIgnoredFields(templates);

			var metadata = _metadataGenerator.Generate(templates);

			var allTemplatesMetadata = metadata.SelectMany(config => config.Metadata).ToArray();

			_architectureValidator.Validate(allTemplatesMetadata);

			return metadata;
		}

		protected virtual TemplateConfiguration[] GetAllTemplates(IEnumerable<IContainer> configurations)
		{
			var results = new List<TemplateConfiguration>();

			foreach (var config in configurations)
			{
				var processingConfig = new TemplateConfiguration(config);
				processingConfig.Templates = GetTemplates(config);
				results.Add(processingConfig);
			}

			return results.ToArray();
		}

		protected virtual IEnumerable<TemplateInfo> GetTemplates(IContainer configuration)
		{
			var templateReader = configuration.Resolve<ITemplateReader>();
			var templatePredicate = configuration.Resolve<ITemplateFilter>();

			Assert.IsNotNull(templateReader, "templateReader != null");
			Assert.IsNotNull(templatePredicate, "templatePredicate != null");

			var roots = templatePredicate.GetRootPaths();

			return templateReader.GetTemplates(roots);
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
