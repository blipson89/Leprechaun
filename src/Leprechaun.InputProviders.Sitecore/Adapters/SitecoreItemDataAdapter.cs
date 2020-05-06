using System;
using System.Collections.Generic;
using System.Linq;
using Leprechaun.Adapters;
using Leprechaun.InputProviders.Sitecore.Extensions;
using Leprechaun.TemplateReaders;
using Sitecore.DevEx.Serialization.Client.Datasources.Filesystem.Query;
using Sitecore.DevEx.Serialization.Client.Query;
using Sitecore.DevEx.Serialization.Models;

namespace Leprechaun.InputProviders.Sitecore.Adapters
{
	public class SitecoreItemDataAdapter : IItemDataAdapter
	{
		private readonly IItemData _itemData;
		private readonly FilesystemTreeDataStore _dataStore;

		public SitecoreItemDataAdapter(IItemData itemData, FilesystemTreeDataStore dataStore)
		{
			_itemData = itemData;
			_dataStore = dataStore;
		}

		public Guid TemplateId => _itemData.TemplateId;
		public Guid Id => _itemData.Id;
		public string Name => _itemData.Name;
		public string Path => _itemData.Path.ToPathString();
		public IEnumerable<IItemFieldValueAdapter> SharedFields => _itemData.SharedFields.Select(sf => new SitecoreItemFieldValueAdapter(sf));
		public IEnumerable<IItemLanguageAdapter> UnversionedFields => _itemData.UnversionedFields.Select(id => new SitecoreItemLanguageAdapter(id));
		public IEnumerable<IItemVersionAdapter> Versions => _itemData.Versions.Select(x => new SitecoreItemVersionAdapter(x));
		public IEnumerable<IItemDataAdapter> GetChildren()
		{
			IItemTreeNode templateRootNode = _dataStore.GetTreeNodeSync(_itemData.Path);
			return templateRootNode.GetChildrenSync().Select(n => new SitecoreItemDataAdapter(_dataStore.GetItemDataSync(n), _dataStore)).ToArray();
		}
	}
}