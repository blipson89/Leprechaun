using System.Collections.Generic;
using System.Linq;
using Leprechaun.Adapters;
using Leprechaun.TemplateReaders;
using Rainbow.Model;

namespace Leprechaun.InputProviders.Rainbow.Adapters
{
	public class RainbowItemVersionAdapter : IItemVersionAdapter
	{
		private readonly IItemVersion _itemVersion;

		public RainbowItemVersionAdapter(IItemVersion itemVersion)
		{
			_itemVersion = itemVersion;
		}

		public IEnumerable<IItemFieldValueAdapter> Fields => _itemVersion.Fields.Select(f => new RainbowItemFieldValueAdapter(f));
	}
}