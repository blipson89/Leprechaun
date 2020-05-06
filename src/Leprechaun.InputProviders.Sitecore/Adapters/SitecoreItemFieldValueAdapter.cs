using System;
using Leprechaun.Adapters;
using Leprechaun.TemplateReaders;
using Sitecore.DevEx.Serialization.Models;

namespace Leprechaun.InputProviders.Sitecore.Adapters
{
	public class SitecoreItemFieldValueAdapter : IItemFieldValueAdapter
	{
		private readonly IItemFieldValue _itemFieldValue;

		public SitecoreItemFieldValueAdapter(IItemFieldValue itemFieldValue)
		{
			_itemFieldValue = itemFieldValue;
		}

		public string Value => _itemFieldValue.Value;
		public Guid FieldId => _itemFieldValue.FieldId;
	}
}