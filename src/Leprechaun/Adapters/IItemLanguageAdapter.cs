using System.Collections.Generic;

namespace Leprechaun.Adapters
{
	public interface IItemLanguageAdapter
	{
		IEnumerable<IItemFieldValueAdapter> Fields { get; }
	}
}