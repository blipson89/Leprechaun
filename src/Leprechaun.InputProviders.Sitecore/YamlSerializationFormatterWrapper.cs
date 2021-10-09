namespace Leprechaun.InputProviders.Sitecore
{
	/// <summary>
	/// Wraps the YamlSerializationFormatter because Configy doesn't like classes with multiple constructors
	/// </summary>
	public class YamlSerializationFormatterWrapper : global::Sitecore.DevEx.Serialization.Client.Datasources.Filesystem.Formatting.Yaml.YamlSerializationFormatter
	{
		public YamlSerializationFormatterWrapper()
		{

		}
	}
}
