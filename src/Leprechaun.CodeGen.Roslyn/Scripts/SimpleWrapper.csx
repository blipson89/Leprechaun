
Log.Debug($"Emitting templates for {ConfigurationName}...");

public string RenderTemplates()
{
	var localCode = new System.Text.StringBuilder();

	foreach (var template in Templates)
	{
		var baseInterfaces = GetBaseInterfaces(template);
		string baseInterfaceStr = "";
		if(baseInterfaces.Length > 0)
		{
			baseInterfaceStr = $" : {baseInterfaces}";
		}
		localCode.AppendLine($@"
namespace {template.Namespace}
{{
	[GeneratedCode(""Leprechaun"", ""2.0.0.0"")]
	public interface I{template.CodeName}Item{baseInterfaceStr}
	{{{RenderInterfaceFields(template)}
	}}
	[GeneratedCode(""Leprechaun"", ""2.0.0.0"")]
	public class {template.CodeName} : CustomItem, I{template.CodeName}Item
	{{
		public {template.CodeName}(Item innerItem)
			:base(innerItem)
		{{
		}}
		public static string TemplateName => ""{template.Name}"";
		public static ID ItemTemplateId => new ID(""{template.Id.ToString("B").ToUpper()}"");
		{RenderFields(template)}
		public static implicit operator {template.CodeName}(Item item) => item != null ? new {template.CodeName}(item) : null;
		public static implicit operator Item({template.CodeName} customItem) => customItem?.InnerItem;
		public struct FieldConstants
		{{
			{RenderFieldConstants(template)}
		}}
	}}
}}"
		);
	}

	return localCode.ToString();
}

Code.AppendLine($@"// ReSharper Disable all
using global::Sitecore.Data;
using global::Sitecore.Data.Fields;
using global::Sitecore.Data.Items;
using global::System.CodeDom.Compiler;
{RenderTemplates()}
");

public string GetBaseInterfaces(TemplateCodeGenerationMetadata template)
{
	var bases = new System.Collections.Generic.List<string>(template.BaseTemplates.Count + 1);

	foreach(var baseTemplate in template.BaseTemplates) 
	{
		bases.Add($"global::{baseTemplate.Namespace}.I{baseTemplate.CodeName}Item");
	}

	return string.Join(", ", bases);
}

public string RenderInterfaceFields(TemplateCodeGenerationMetadata template)
{
	var localCode = new System.Text.StringBuilder();

	foreach (var field in template.OwnFields)
	{
		localCode.Append($@"
		{GetFieldType(field)} {field.CodeName}Field {{ get; }}");
	}

	return localCode.ToString();
}

public string RenderFields(TemplateCodeGenerationMetadata template)
{
	var localCode = new System.Text.StringBuilder();

	foreach (var field in template.AllFields)
	{
		localCode.Append($@"
		public {GetFieldType(field)} {field.CodeName}Field => new {GetFieldType(field)}(InnerItem.Fields[FieldConstants.{field.CodeName}.Id]{(GetFieldType(field) == "DelimitedField" ? ", '|'" : string.Empty)});");
	}

	return localCode.ToString();
}

public string RenderFieldConstants(TemplateCodeGenerationMetadata template)
{
    var localCode = new System.Text.StringBuilder();

    foreach (var field in template.AllFields)
    {
        localCode.Append($@"public struct {field.CodeName}
            {{
		        public const string FieldName = ""{field.Name}"";
		        public static readonly ID Id = new ID(""{field.Id.ToString("B").ToUpper()}"");
            }}
            ");
    }

    return localCode.ToString();
}

public string GetFieldType(TemplateFieldCodeGenerationMetadata field)
{
	switch(field.Type) {
		// Simple Types
		case "Checkbox": return "CheckboxField";
		case "Date": return "DateField";
		case "Datetime": return "DateField";
		case "File": return "FileField";
		case "Image": return "ImageField";
		case "Integer": return "TextField";
		case "Number": return "TextField";
		case "Rich Text": return "TextField";
		case "Multi-Line Text":
		case "Password":
		case "Single-Line Text": return "TextField";

		// List Types
		case "Checklist":
		case "Grouped Droplink": return "ReferenceField";
		case "Droplist":
		case "Grouped Droplist": return "TextField";
		case "Multilist":
		case "Treelist":
		case "TreelistEx": return "DelimitedField";
		case "Name Value List": return "NameValueListField";

		// Link Types
		case "Droplink":
		case "Droptree": return "ReferenceField";
		case "General Link": return "LinkField";

		// System Types
		case "Internal Link": return "InternalLinkField";

		// Developer Types
		case "Tristate": return "TristateField";

		// Deprecated Types
		case "text": return "TextField";
		case "memo": return "TextField";
		case "lookup":
		case "reference":
		case "tree": return "ReferenceField";
		case "tree list": return "DelimitedField";
		case "html": return "TextField";
		case "link": return "LinkField";
		default: return "TextField";
	}
}