// Generates Glass Constants File

Log.Debug($"Emitting Rendering constants for {ConfigurationName}...");

public string RenderRenderings()
{
	var localCode = new System.Text.StringBuilder();

	localCode.Append($@"
	public struct Renderings
	{{
");

	foreach (var rendering in Renderings)
	{
		localCode.AppendLine($@"		public static readonly ID {rendering.CodeName} = new ID(""{rendering.Id}"");");
	}

	localCode.Append(@"
	}");

	return localCode.ToString();
}

Code.AppendLine($@"
namespace {GenericRootNamespace}
{{
	using global::Sitecore.Data;
	{RenderRenderings()}
	
}}");