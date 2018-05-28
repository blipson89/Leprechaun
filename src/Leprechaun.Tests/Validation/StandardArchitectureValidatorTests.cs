using System;
using System.Collections.Generic;
using System.Xml;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using FluentAssertions;
using Leprechaun.Model;
using Leprechaun.Tests.Test.SpecimenBuilders;
using Leprechaun.Tests.Test.SpecimenBuilders.StandardArchitectorValidator;
using Leprechaun.Validation;
using Xunit;

namespace Leprechaun.Tests.Validation
{
	public class StandardArchitectureValidatorTests
	{
		#region ValidateTemplateNamesAreNovel

		[Theory, StandardArchitectureValidatorConventions(typeof(DuplicateTemplateNamesSpec))]
		public void ValidateTemplateNamesAreNovel_WhenThereAreDuplicateTemplateNames_ReturnFalse([Greedy] TestableStandardArchitectureValidator sut, TemplateCodeGenerationMetadata[] allTemplates)
		{
			sut.Public_ValidateTemplateNamesAreNovel(allTemplates).Should().BeFalse();
		}

		[Theory, StandardArchitectureValidatorConventions]
		public void ValidateTemplateNamesAreNovel_WhenThereAreNoDuplicateTemplateNames_ReturnTrue([Greedy] TestableStandardArchitectureValidator sut, TemplateCodeGenerationMetadata[] allTemplates)
		{
			sut.Public_ValidateTemplateNamesAreNovel(allTemplates).Should().BeTrue();
		}
		

			#endregion

		#region ValidateTemplateHasNoFieldsIdenticalToTemplateName

		[Theory, StandardArchitectureValidatorConventions]
		public void ValidateTemplateHasNoFieldsIdenticalToTemplateName_WhenItDoesnt_ReturnTrue([Greedy] TestableStandardArchitectureValidator sut, TemplateCodeGenerationMetadata template)
		{
			sut.Public_ValidateTemplateHasNoFieldsIdenticalToTemplateName(template).Should().BeTrue();
		}

		[Theory, StandardArchitectureValidatorConventions]
		public void ValidateTemplateHasNoFieldsIdenticalToTemplateName_WhenItDoes_ReturnFalse([Greedy] TestableStandardArchitectureValidator sut, TemplateCodeGenerationMetadata template)
		{
			sut.ForceFieldNameToMatchTemplateName = true;
			sut.Public_ValidateTemplateHasNoFieldsIdenticalToTemplateName(template).Should().BeFalse();
		}

		#endregion

		#region ValidateTemplateFieldNamesAreNovel

		[Theory, StandardArchitectureValidatorConventions(typeof(TemplateMetadataSpec))]
		public void ValidateTemplateFieldNamesAreNovel_WhenThereAreNoFieldsWithIdenticalNames_ReturnTrue([Greedy] TestableStandardArchitectureValidator sut, TemplateCodeGenerationMetadata template, IReadOnlyDictionary<Guid, TemplateCodeGenerationMetadata> allTemplatesIndex)
		{
			sut.Public_ValidateTemplateFieldNamesAreNovel(template, allTemplatesIndex).Should().BeTrue();
		}

		[Theory, StandardArchitectureValidatorConventions(typeof(TemplateMetadataDuplicateFieldSpec))]
		public void ValidateTemplateFieldNamesAreNovel_WhenThereAreFieldsWithIdenticalNames_ReturnFalse([Greedy] TestableStandardArchitectureValidator sut, TemplateCodeGenerationMetadata template, IReadOnlyDictionary<Guid, TemplateCodeGenerationMetadata> allTemplatesIndex)
		{
			sut.Public_ValidateTemplateFieldNamesAreNovel(template, allTemplatesIndex).Should().BeFalse();
		}

		#endregion
	}

	public class TestableStandardArchitectureValidator : StandardArchitectureValidator
	{
		public bool ForceFieldNameToMatchTemplateName { get; set; } = false;
		public TestableStandardArchitectureValidator(XmlNode configNode, IArchitectureValidatorLogger logger) : base(configNode, logger)
		{
		}

		public bool Public_ValidateTemplateFieldNamesAreNovel(TemplateCodeGenerationMetadata template,
			IReadOnlyDictionary<Guid, TemplateCodeGenerationMetadata> allTemplatesIndex)
		{
			return ValidateTemplateFieldNamesAreNovel(template, allTemplatesIndex);
		}

		public bool Public_ValidateTemplateHasNoFieldsIdenticalToTemplateName(TemplateCodeGenerationMetadata template)
		{
			return ValidateTemplateHasNoFieldsIdenticalToTemplateName(template);
		}

		public bool Public_ValidateTemplateNamesAreNovel(TemplateCodeGenerationMetadata[] allTemplates)
		{
			return ValidateTemplateNamesAreNovel(allTemplates);
		}

		public bool Public_ValidateBaseTemplatesAreKnown(TemplateCodeGenerationMetadata template)
		{
			return ValidateBaseTemplatesAreKnown(template);
		}

		protected override bool FieldCodeNameEqualsTemplateCodeName(TemplateFieldCodeGenerationMetadata field, TemplateCodeGenerationMetadata template)
		{
			if (ForceFieldNameToMatchTemplateName)
				return true;
			return base.FieldCodeNameEqualsTemplateCodeName(field, template);
		}
	}

	public class StandardArchitectureValidatorConventionsAttribute : AutoDataAttribute
	{
		private static IFixture MakeFixture(Type specType)
		{
			var fixture = new Fixture();
			fixture.Customize(new AutoNSubstituteCustomization{ConfigureMembers = true});
			fixture.Customizations.Add(new XmlNodeBuilder());
			if (specType != null)
			{
				var spec = (ITemplateMetadataSpec) Activator.CreateInstance(specType);
				spec.Init(fixture);
				fixture.Customizations.Add(spec);
			}
			return fixture;
		}

		public StandardArchitectureValidatorConventionsAttribute() : base(() => MakeFixture(null))
		{
			
		}
		public StandardArchitectureValidatorConventionsAttribute(Type spec) : base(() => MakeFixture(spec))
		{
			
		}
	}
}
