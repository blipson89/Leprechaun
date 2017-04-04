using System;
using System.Collections.Generic;
using System.Linq;
using Leprechaun.Filters;
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
				var predicate = configuration.Configuration.Resolve<ITemplatePredicate>();

				var templates = configuration.Templates
					.Select(template => CreateTemplate(nameGenerator, predicate, template))
					.OrderBy(template => template.Name, StringComparer.Ordinal)
					.ToArray();

				results.Add(new ConfigurationCodeGenerationMetadata(configuration.Configuration, templates));
			}

			ApplyBaseTemplates(results);

			results.Sort((a, b) => string.Compare(a.Configuration.Name, b.Configuration.Name, StringComparison.Ordinal));

			return results;
		}

		protected virtual TemplateCodeGenerationMetadata CreateTemplate(ITypeNameGenerator nameGenerator, ITemplatePredicate predicate, TemplateInfo template)
		{
			var fullName = nameGenerator.GetFullTypeName(template.Path);

			var fields = CreateTemplateFields(template, nameGenerator);

			return new TemplateCodeGenerationMetadata(template, fullName, predicate.GetRootNamespace(template), fields);
		}

		protected virtual IEnumerable<TemplateFieldCodeGenerationMetadata> CreateTemplateFields(TemplateInfo template, ITypeNameGenerator nameGenerator)
		{
			var fields = new List<TemplateFieldCodeGenerationMetadata>(template.OwnFields.Length);

			foreach (var field in template.OwnFields)
			{
				var currentField = new TemplateFieldCodeGenerationMetadata(field, nameGenerator.ConvertToIdentifier(field.Name));

				fields.Add(currentField);
			}

			fields.Sort((a,b) => string.Compare(a.CodeName, b.CodeName, StringComparison.Ordinal));

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
