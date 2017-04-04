using Leprechaun.Model;
using Rainbow.Storage;

namespace Leprechaun.Filters
{
	public interface ITemplatePredicate
	{
		bool Includes(TemplateInfo template);

		TreeRoot[] GetRootPaths();

		/// <param name="template">Template to get a root NS for. MAY BE NULL, in which case a general root NS should be returned.</param>
		string GetRootNamespace(TemplateInfo template);
	}
}
