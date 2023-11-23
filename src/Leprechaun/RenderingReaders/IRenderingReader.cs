using Leprechaun.Filters;
using Leprechaun.Model;

namespace Leprechaun.RenderingReaders
{
	public interface IRenderingReader
	{
		RenderingInfo[] GetRenderings(ITemplatePredicate predicate);
	}
}
