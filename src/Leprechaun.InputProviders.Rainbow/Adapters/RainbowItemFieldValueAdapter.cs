using System;
using Leprechaun.Adapters;
using Leprechaun.TemplateReaders;
using Rainbow.Model;

namespace Leprechaun.InputProviders.Rainbow.Adapters
{
	public class RainbowItemFieldValueAdapter : IItemFieldValueAdapter
	{
		private readonly IItemFieldValue _itemFieldValue;

		public RainbowItemFieldValueAdapter(IItemFieldValue itemFieldValue)
		{
			_itemFieldValue = itemFieldValue;
		}

		public string Value => _itemFieldValue.Value;
		public Guid FieldId => _itemFieldValue.FieldId;
	}
}