using System.Collections.Generic;
using System.Linq;
using Leprechaun.Adapters;
using Leprechaun.TemplateReaders;
using Sitecore.DevEx.Serialization.Models;

namespace Leprechaun.InputProviders.Sitecore.Adapters
{
	public class SitecoreItemVersionAdapter : IItemVersionAdapter
	{
		private readonly IItemVersion _itemVersion;

		public SitecoreItemVersionAdapter(IItemVersion itemVersion)
		{
			_itemVersion = itemVersion;
		}

		public IEnumerable<IItemFieldValueAdapter> Fields => _itemVersion.Fields.Select(f => new SitecoreItemFieldValueAdapter(f));
	}
}