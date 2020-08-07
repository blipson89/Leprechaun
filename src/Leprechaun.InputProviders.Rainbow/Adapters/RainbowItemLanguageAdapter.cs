using System.Collections.Generic;
using System.Linq;
using Leprechaun.Adapters;
using Leprechaun.TemplateReaders;
using Rainbow.Model;

namespace Leprechaun.InputProviders.Rainbow.Adapters
{
	public class RainbowItemLanguageAdapter : IItemLanguageAdapter
	{
		private readonly IItemLanguage _itemLanguage;

		public RainbowItemLanguageAdapter(IItemLanguage itemLanguage)
		{
			_itemLanguage = itemLanguage;
		}

		public IEnumerable<IItemFieldValueAdapter> Fields => _itemLanguage.Fields.Select(f => new RainbowItemFieldValueAdapter(f));
	}
}