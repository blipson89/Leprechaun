using System.Collections.Generic;

namespace Leprechaun.Adapters
{
	public interface IItemVersionAdapter
	{
		IEnumerable<IItemFieldValueAdapter> Fields { get; }
	}
}