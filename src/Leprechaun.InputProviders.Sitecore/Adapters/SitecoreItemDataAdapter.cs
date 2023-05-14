using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leprechaun.Adapters;
using Leprechaun.InputProviders.Sitecore.Extensions;
using Leprechaun.Logging;
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
		private readonly ILogger _logger = null;

		public SitecoreItemDataAdapter(IItemData itemData, FilesystemTreeDataStore dataStore)
		{
			_itemData = itemData;
			_dataStore = dataStore;
		}

		public SitecoreItemDataAdapter(IItemData itemData, FilesystemTreeDataStore dataStore, ILogger logger) : this(itemData, dataStore)
		{
			_logger = logger;
		}

		public Guid TemplateId => _itemData.TemplateId;
		public Guid Id => _itemData.Id;
		public string Name => _itemData.Name;
		public string Path => _itemData.Path.ToPathString();
		public IEnumerable<IItemFieldValueAdapter> SharedFields => _itemData.SharedFields.Select(sf => new SitecoreItemFieldValueAdapter(sf));
		public IEnumerable<IItemLanguageAdapter> UnversionedFields => _itemData.UnversionedFields.Select(id => new SitecoreItemLanguageAdapter(id));
		public IEnumerable<IItemVersionAdapter> Versions => _itemData.Versions.Select(x => new SitecoreItemVersionAdapter(x));
		public IEnumerable<IItemDataAdapter> GetChildren() => GetChildrenAsync().GetAwaiter().GetResult();
		public async Task<IEnumerable<IItemDataAdapter>> GetChildrenAsync()
		{
			IItemTreeNode templateRootNode = await _dataStore.GetTreeNode(_itemData.Path);
			_logger?.Debug($"GetChildrenAsync for ${_itemData.Path}");
			return (await templateRootNode.GetChildren()).Select(n => new SitecoreItemDataAdapter(_dataStore.GetItemDataSync(n), _dataStore, _logger)).ToArray();
		}
	}
}