using System;
using System.Collections.Generic;
using System.Linq;
using Leprechaun.Adapters;
using Leprechaun.TemplateReaders;
using Rainbow.Model;

namespace Leprechaun.InputProviders.Rainbow.Adapters
{
	public class RainbowItemDataAdapter : IItemDataAdapter
	{
		private readonly IItemData _itemData;

		public RainbowItemDataAdapter(IItemData itemData)
		{
			_itemData = itemData;
		}

		public Guid TemplateId => _itemData.TemplateId;
		public Guid Id => _itemData.Id;
		public string Name => _itemData.Name;
		public string Path => _itemData.Path;
		public IEnumerable<IItemFieldValueAdapter> SharedFields => _itemData.SharedFields.Select(sf => new RainbowItemFieldValueAdapter(sf));
		public IEnumerable<IItemLanguageAdapter> UnversionedFields => _itemData.UnversionedFields.Select(id => new RainbowItemLanguageAdapter(id));
		public IEnumerable<IItemVersionAdapter> Versions => _itemData.Versions.Select(x => new RainbowItemVersionAdapter(x));
		public IEnumerable<IItemDataAdapter> GetChildren()
		{
			return _itemData.GetChildren().Select(c => new RainbowItemDataAdapter(c));
		}
	}
}