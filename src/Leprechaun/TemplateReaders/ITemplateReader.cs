using Leprechaun.Filters;
using Leprechaun.Model;

namespace Leprechaun.TemplateReaders
{
	public interface ITemplateReader
	{
		TemplateInfo[] GetTemplates(ITemplatePredicate predicate);
	}
}