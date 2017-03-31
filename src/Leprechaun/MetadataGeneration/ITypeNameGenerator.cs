namespace Leprechaun.MetadataGeneration
{
	public interface ITypeNameGenerator
	{
		string GetFullTypeName(string fullPath);
		string ConvertToIdentifier(string name);
	}
}