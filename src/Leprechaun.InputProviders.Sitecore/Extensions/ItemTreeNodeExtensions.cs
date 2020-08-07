using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.DevEx.Serialization.Client.Query;

namespace Leprechaun.InputProviders.Sitecore.Extensions
{
	public static class ItemTreeNodeExtensions
	{
		public static IEnumerable<IItemTreeNode> GetChildrenSync(this IItemTreeNode treeNode)
		{
			return Task.Run(async () => await treeNode.GetChildren()).GetAwaiter().GetResult();
		}
	}
}