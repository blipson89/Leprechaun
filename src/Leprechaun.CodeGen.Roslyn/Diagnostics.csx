Log.Debug($"Emitting diagnostic templates for {ConfigurationName}...");

foreach (var template in Templates)
{
	Code.AppendLine($"// {template.FullTypeName} ({template.Path} {template.Id})");

	foreach (var field in template.OwnFields)
	{
		Code.AppendLine($"\t// {field.CodeName} ({field.Id})");
		Code.AppendLine($"\t\t// Type: {field.Type}");
		Code.AppendLine($"\t\t// Section: {field.Section}");
		Code.AppendLine($"\t\t// Sort Order: {field.SortOrder}");
		Code.AppendLine($"\t\t// Source: {field.Source}");
	}

	Code.AppendLine(string.Empty);
}