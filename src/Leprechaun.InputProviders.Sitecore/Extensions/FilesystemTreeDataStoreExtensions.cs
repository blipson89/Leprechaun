using System.Linq;
using System.Threading.Tasks;
using Sitecore.DevEx.Serialization.Client.Datasources.Filesystem.Query;
using Sitecore.DevEx.Serialization.Client.Query;
using Sitecore.DevEx.Serialization.Models;

namespace Leprechaun.InputProviders.Sitecore.Extensions
{
	public static class FilesystemTreeDataStoreExtensions
	{
		public static async Task<IItemTreeNode> GetTreeNode(this FilesystemTreeDataStore dataStore, ItemPath itemPath)
		{
			return (await dataStore.GetTreeNodes(new[] {new ItemSpec(itemPath)})).FirstOrDefault();
		}

		public static IItemTreeNode GetTreeNodeSync(this FilesystemTreeDataStore dataStore, ItemPath itemPath)
		{
			return Task.Run(async () => await GetTreeNode(dataStore, itemPath)).GetAwaiter().GetResult();
		}

		public static async Task<IItemData> GetItemData(this FilesystemTreeDataStore dataStore, IItemTreeNode treeNode)
		{
			return (await dataStore.GetItemData(new[] {new ItemSpec(treeNode.Value.Path)})).FirstOrDefault();
		}

		public static IItemData GetItemDataSync(this FilesystemTreeDataStore dataStore, IItemTreeNode treeNode)
		{
			return dataStore.GetItemData(treeNode).GetAwaiter().GetResult();
		}
	}
}