using Leprechaun.Model;

namespace Leprechaun.Validation
{
	public interface IArchitectureValidator
	{
		void Validate(TemplateCodeGenerationMetadata[] allTemplates);
	}
}