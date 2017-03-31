using Leprechaun.Model;

namespace Leprechaun.CodeGen
{
	public interface ICodeGenerator
	{
		void GenerateCode(ConfigurationCodeGenerationMetadata metadata);
	}
}
