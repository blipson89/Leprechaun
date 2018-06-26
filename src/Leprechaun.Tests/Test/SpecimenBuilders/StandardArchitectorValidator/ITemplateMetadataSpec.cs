using AutoFixture;
using AutoFixture.Kernel;

namespace Leprechaun.Tests.Test.SpecimenBuilders.StandardArchitectorValidator
{
	public interface ITemplateMetadataSpec : ISpecimenBuilder
	{
		void Init(IFixture fixture);
	}
}