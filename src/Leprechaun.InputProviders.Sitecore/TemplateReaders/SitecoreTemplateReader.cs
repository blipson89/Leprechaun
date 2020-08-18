using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Leprechaun.Filters;
using Leprechaun.InputProviders.Sitecore.Adapters;
using Leprechaun.InputProviders.Sitecore.Configuration;
using Leprechaun.InputProviders.Sitecore.Extensions;
using Leprechaun.InputProviders.Sitecore.Filters;
using Leprechaun.Model;
using Leprechaun.TemplateReaders;


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
				return GetTemplates(scPredicate.GetModule()).ToArray();
			}
			return new TemplateInfo[0];
		}

		public IEnumerable<TemplateInfo> GetTemplates(LeprechaunModuleConfiguration module)
		{
			return module.SerializationModule.Items.Includes
				.AsParallel()
				.SelectMany(fsTreeSpec =>
				{
					var templateItemData = module.DataStore.GetItemDataSync(module.DataStore.GetTreeNodeSync(fsTreeSpec.Path));
					var itemAdapter = new SitecoreItemDataAdapter(templateItemData, module.DataStore);
					return ParseTemplates(itemAdapter);
				})
				.ToArray();
		}

		protected override Guid[] ParseMultilistValue(string value)
		{
			return value.Split(new []{'\r','\n','|'}, StringSplitOptions.RemoveEmptyEntries)
				.Select(item => Guid.TryParse(item, out Guid result) ? result : Guid.Empty)
				.Where(item => item != Guid.Empty)
				.ToArray();
		}
	}
}
