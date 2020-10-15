using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using Configy.Containers;
using Leprechaun.Filters;
using Leprechaun.MetadataGeneration;
using Leprechaun.Model;
using Leprechaun.RenderingReaders;
using Leprechaun.TemplateReaders;
using Leprechaun.Validation;

namespace Leprechaun
{
	public class Orchestrator : IOrchestrator
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
				var processingConfig = new TemplateConfiguration(config)
				{
					Templates = GetTemplates(config), 
					Renderings = GetRenderings(config)
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

		private IEnumerable<RenderingInfo> GetRenderings(IContainer configuration)
		{
			var renderingReader = configuration.Resolve<IRenderingReader>();
			var templatePredicate = configuration.Resolve<ITemplatePredicate>();

			Assert.IsNotNull(renderingReader, "renderingReader != null");
			Assert.IsNotNull(templatePredicate, "templatePredicate != null");

			return renderingReader.GetRenderings(templatePredicate);
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
