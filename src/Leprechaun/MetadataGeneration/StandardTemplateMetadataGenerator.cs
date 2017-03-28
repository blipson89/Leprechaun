using System.Collections.Generic;
using System.Linq;
using Leprechaun.Model;

namespace Leprechaun.MetadataGeneration
{
	public class StandardTemplateMetadataGenerator : ITemplateMetadataGenerator
	{
		public virtual IReadOnlyList<ConfigurationCodeGenerationMetadata> Generate(params TemplateConfiguration[] configurations)
		{
			var results = new List<ConfigurationCodeGenerationMetadata>(configurations.Length);

			foreach (var configuration in configurations)
			{
				var nameGenerator = configuration.Configuration.Resolve<ITypeNameGenerator>();

				var templates = configuration.Templates.Select(template => CreateTemplate(nameGenerator, template));

				results.Add(new ConfigurationCodeGenerationMetadata(configuration.Configuration, templates));
			}

			ApplyBaseTemplates(results);

			return results;
		}

		protected virtual TemplateCodeGenerationMetadata CreateTemplate(ITypeNameGenerator nameGenerator, TemplateInfo template)
		{
			var fullName = nameGenerator.GetFullTypeName(template.Name, template.Path);

			var fields = CreateTemplateFields(template, nameGenerator);

			return new TemplateCodeGenerationMetadata(template, fullName, fields);
		}

		protected virtual IEnumerable<TemplateFieldCodeGenerationMetadata> CreateTemplateFields(TemplateInfo template, ITypeNameGenerator nameGenerator)
		{
			var fields = new List<TemplateFieldCodeGenerationMetadata>(template.OwnFields.Length);

			foreach (var field in template.OwnFields)
			{
				var currentField = new TemplateFieldCodeGenerationMetadata(field, nameGenerator.ConvertToIdentifier(field.Name));

				fields.Add(currentField);
			}

			return fields;
		}

		protected virtual void ApplyBaseTemplates(IList<ConfigurationCodeGenerationMetadata> generatedMetadata)
		{
			// index all templates' metadata (so we can resolve base template IDs)
			var allTemplatesIndex = generatedMetadata
				.SelectMany(config => config.Metadata)
				.ToDictionary(template => template.Id);

			foreach (var config in generatedMetadata)
			{
				foreach (var template in config.Metadata)
				{
					foreach (var baseTemplateId in template.TemplateInfo.BaseTemplateIds)
					{
						// resolve all base template objects and assign them to the collection
						// we don't worry about unresolvable base templates here; the validator checks for that

						if (!allTemplatesIndex.TryGetValue(baseTemplateId, out TemplateCodeGenerationMetadata baseTemplate))
						{
							continue;
						}

						template.BaseTemplates.Add(baseTemplate);
					}
				}
			}
		}
	}
}
