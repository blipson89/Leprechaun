using AutoFixture;
using Leprechaun.Model;

namespace Leprechaun.Tests.Test.SpecimenBuilders.StandardArchitectorValidator
{
	public class TemplateMetadataDuplicateFieldSpec : TemplateMetadataSpec
	{
		protected override void InnerInit(IFixture fixture)
		{
			base.InnerInit(fixture);
			var metadataWithDuplicateField =
				fixture.Build<TemplateCodeGenerationMetadata>()
					.FromFactory(() => new TemplateCodeGenerationMetadata
					(fixture.Create<TemplateInfo>(),
						fixture.Create<string>(),
						fixture.Create<string>(),
						Metadata.OwnFields))
					.WithAutoProperties()
					.Create();
			Metadata.BaseTemplates.Add(metadataWithDuplicateField);
			Metadatas.Add(metadataWithDuplicateField);
		}
	}
}