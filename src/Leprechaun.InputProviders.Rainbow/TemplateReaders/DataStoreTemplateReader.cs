using System.Linq;
using System.Xml;
using Leprechaun.Filters;
using Leprechaun.InputProviders.Rainbow.Adapters;
using Leprechaun.InputProviders.Rainbow.Filters;
using Leprechaun.Model;
using Leprechaun.TemplateReaders;
using Rainbow.Storage;

namespace Leprechaun.InputProviders.Rainbow.TemplateReaders
{
	public class DataStoreTemplateReader : BaseTemplateReader
	{
		private readonly IDataStore _dataStore;

		public DataStoreTemplateReader(XmlNode configNode, IDataStore dataStore) : base(configNode)
		{
			_dataStore = dataStore;
		}


		public override TemplateInfo[] GetTemplates(ITemplatePredicate predicate)
		{
			if (predicate is StandardTemplatePredicate standardPredicate)
			{
				return GetTemplates(standardPredicate.GetRootPaths());
			}
			return new TemplateInfo[0];
		}

		public TemplateInfo[] GetTemplates(params TreeRoot[] rootPaths)
		{
			return rootPaths
				.AsParallel()
				.SelectMany(root =>
				{
					var rootItem = _dataStore.GetByPath(root.Path, root.DatabaseName)?.Select(itemData => new RainbowItemDataAdapter(itemData));

					if (rootItem == null) return Enumerable.Empty<TemplateInfo>();

					// because a path could match more than one item we have to SelectMany again
					return rootItem.SelectMany(ParseTemplates);
				})
				.ToArray();
		}
	}
}
