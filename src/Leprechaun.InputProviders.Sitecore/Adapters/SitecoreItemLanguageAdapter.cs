using System.Collections.Generic;
using System.Linq;
using Leprechaun.Adapters;
using Leprechaun.TemplateReaders;
using Sitecore.DevEx.Serialization.Models;

namespace Leprechaun.InputProviders.Sitecore.Adapters
{
	public class SitecoreItemLanguageAdapter : IItemLanguageAdapter
	{
		private readonly IItemLanguage _itemLanguage;

		public SitecoreItemLanguageAdapter(IItemLanguage itemLanguage)
		{
			_itemLanguage = itemLanguage;
		}

		public IEnumerable<IItemFieldValueAdapter> Fields => _itemLanguage.Fields.Select(f => new SitecoreItemFieldValueAdapter(f));
	}
}