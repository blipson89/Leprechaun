// Generates Sitecore Habitat-style constants classes

Log.Debug($"Emitting constants templates for {ConfigurationName}...");

public string RenderFields(TemplateCodeGenerationMetadata template)
{
	if (template.OwnFields.Length == 0)
	{
		return string.Empty;
	}

	var localCode = new System.Text.StringBuilder();

	localCode.Append($@"
			public struct Fields
			{{");

	foreach (var field in template.OwnFields)
	{
		localCode.AppendLine($@"
				public static readonly ID {field.CodeName} = new ID(""{field.Id}"");
				public const string {field.CodeName}_FieldName = ""{field.Name}"";");
	}

	localCode.Append(@"
			}");

	return localCode.ToString();
}
public string RenderTemplates()
{
	var localCode = new System.Text.StringBuilder();
	
	foreach(var template in Templates)
	{
		localCode.AppendLine($@"
		public struct {template.CodeName}
		{{
			public static readonly ID ID = new ID(""{template.Id}"");

			{RenderFields(template)}
		}}");
	}

	return localCode.ToString();
}

Code.AppendLine($@"
namespace {GenericRootNamespace}
{{
	using global::Sitecore.Data;

	public struct Templates
	{{
		{RenderTemplates()}
	}}
}}");