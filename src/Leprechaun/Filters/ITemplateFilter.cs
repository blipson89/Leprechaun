using Leprechaun.Model;
using Rainbow.Storage;

namespace Leprechaun.Filters
{
	public interface ITemplateFilter
	{
		bool Includes(TemplateInfo template);

		TreeRoot[] GetRootPaths();
	}
}
