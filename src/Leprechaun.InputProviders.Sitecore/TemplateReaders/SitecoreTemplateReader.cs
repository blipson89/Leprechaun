using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Leprechaun.Filters;
using Leprechaun.InputProviders.Sitecore.Adapters;
using Leprechaun.InputProviders.Sitecore.Configuration;
using Leprechaun.InputProviders.Sitecore.Extensions;
using Leprechaun.InputProviders.Sitecore.Filters;
using Leprechaun.Model;
using Leprechaun.TemplateReaders;
using Sitecore.DevEx.Serialization;
using Sitecore.DevEx.Serialization.Client.Query;


namespace Leprechaun.InputProviders.Sitecore.TemplateReaders
{
	public class SitecoreTemplateReader : BaseTemplateReader, ITemplateReader
	{
		public SitecoreTemplateReader(XmlNode configNode) : base(configNode)
		{
		}

		public override TemplateInfo[] GetTemplates(ITemplatePredicate predicate)
		{
			if (predicate is SitecoreTemplatePredicate scPredicate)
			{
				return GetTemplates(scPredicate).GetAwaiter().GetResult().ToArray();
			}
			return new TemplateInfo[0];
		}
		
		public async Task<IEnumerable<TemplateInfo>> GetTemplates(SitecoreTemplatePredicate predicate)
		{
			var module = predicate.GetModule();
			await module.DataStore.Reinitialize(null); // ensure the datastore is up to date
			var tasks = new List<Task<IEnumerable<TemplateInfo>>>();
			foreach (var fstree in predicate.GetTreeSpecs())
			{
				if (fstree.Scope == TreeScope.DescendantsOnly)
				{
					var templates = (await module.DataStore
						.GetChildren(fstree.Path))
						.Select(child => ConvertTreeToTemplates(module, child));

					tasks.AddRange(templates);
				}
				else
				{
					tasks.Add(ConvertTreeToTemplates(module, await module.DataStore.GetTreeNode(fstree.Path)));
				}
			}

			return (await Task.WhenAll(tasks)).SelectMany(x => x);
		}
		
		private async Task<IEnumerable<TemplateInfo>> ConvertTreeToTemplates(LeprechaunModuleConfiguration module, IItemTreeNode tn)
		{
			var templateItemData = await module.DataStore.GetItemData(tn);
			var itemAdapter = new SitecoreItemDataAdapter(templateItemData, module.DataStore);
			return ParseTemplates(itemAdapter);
		}

		protected override Guid[] ParseMultilistValue(string value)
		{
			return value.Split(new []{@"\r",@"\n","|", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
				.Select(item => Guid.TryParse(item, out Guid result) ? result : Guid.Empty)
				.Where(item => item != Guid.Empty)
				.ToArray();
		}
	}
}
