namespace Leprechaun.MetadataGeneration
{
	public interface ITypeNameGenerator
	{
		string GetFullTypeName(string templateName, string fullPath);
		string ConvertToIdentifier(string name);
	}
}