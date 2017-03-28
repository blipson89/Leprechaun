using Leprechaun.Model;

namespace Leprechaun.Filters
{
	public interface IFieldFilter
	{
		bool Includes(TemplateFieldInfo field);
	}
}
