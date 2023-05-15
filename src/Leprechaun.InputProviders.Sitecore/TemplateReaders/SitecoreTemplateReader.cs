using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Leprechaun.Filters;
using Leprechaun.InputProviders.Sitecore.Adapters;
using Leprechaun.InputProviders.Sitecore.Configuration;
using Leprechaun.InputProviders.Sitecore.Extensions;
using Leprechaun.InputProviders.Sitecore.Filters;
using Leprechaun.Logging;
using Leprechaun.Model;
using Leprechaun.TemplateReaders;
using Sitecore.DevEx.Serialization;
using Sitecore.DevEx.Serialization.Client.Query;
using Sitecore.DevEx.Serialization.Exceptions;
using Sitecore.DevEx.Serialization.Models;


namespace Leprechaun.InputProviders.Sitecore.TemplateReaders
{
	public class SitecoreTemplateReader : BaseTemplateReader, ITemplateReader
	{
		private readonly ILogger _logger;

		public SitecoreTemplateReader(XmlNode configNode, ILogger logger) : base(configNode)
		{
			_logger = logger;
		}

		public override TemplateInfo[] GetTemplates(ITemplatePredicate predicate)
		{
			_logger.Debug("[SitecoreTemplateReader] Getting templates  based on predicates - A");
			if (predicate is SitecoreTemplatePredicate scPredicate)
			{
				_logger.Debug("[SitecoreTemplateReader] Getting templates  based on predicates - B");
				return GetTemplates(scPredicate).GetAwaiter().GetResult().ToArray();
			}
			_logger.Warn($"[SitecoreTemplateReader] Incorrect predicate type passed to method! {predicate.GetType().FullName}");
			return new TemplateInfo[0];
		}
		
		public async Task<IEnumerable<TemplateInfo>> GetTemplates(SitecoreTemplatePredicate predicate)
		{
			try
			{
				_logger.Debug("[SitecoreTemplateReader] Getting templates - A");
				var module = predicate.GetModule();
				_logger.Debug("[SitecoreTemplateReader] Getting templates - B");
				await module.DataStore.Reinitialize(null); // ensure the datastore is up to date
				_logger.Debug("[SitecoreTemplateReader] Getting templates - C");
				var tasks = new List<Task<IEnumerable<TemplateInfo>>>();
				_logger.Debug("[SitecoreTemplateReader] Getting templates - D");
				foreach (var fstree in predicate.GetTreeSpecs())
				{
					_logger.Debug($"[SitecoreTemplateReader] Getting templates - Get Tree Specs '{fstree.Name}'");
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
					_logger.Debug($"[SitecoreTemplateReader] Getting templates - Done Get Tree Specs '{fstree.Name}'");
				}

				return (await Task.WhenAll(tasks)).SelectMany(x => x);
			}
			catch (Exception ex)
			{
				_logger.Error("[SitecoreTemplateReader] Error occurred while attempting to get templates. ", ex);
				return Enumerable.Empty<TemplateInfo>();
			}
		}
		
		private async Task<IEnumerable<TemplateInfo>> ConvertTreeToTemplates(LeprechaunModuleConfiguration module, IItemTreeNode tn)
		{
			_logger.Debug("[SitecoreTemplateReader] ConvertTreeToTemplates");
			IItemData templateItemData = null;
			try
			{
				_logger.Debug("[SitecoreTemplateReader] ConvertTreeToTemplates - GetItemData");
				templateItemData = await module.DataStore.GetItemData(tn);
				_logger.Debug("[SitecoreTemplateReader] ConvertTreeToTemplates - Done GetItemData");
			}
			catch (NullReferenceException ex)
			{
				var exceptionMessage = new StringBuilder("Itemdata was null for the provided treenode.");
				exceptionMessage.AppendLine();
				exceptionMessage.AppendLine();
				exceptionMessage.AppendLine("This likely indicates the filesystem is out of sync with the module.json.");
				exceptionMessage.Append("Try running 'dotnet sitecore ser validate -f' and 'dotnet sitecore ser pull',");
				exceptionMessage.AppendLine(" then run Leprechaun again.");
				var exception = new InvalidConfigurationException(exceptionMessage.ToString(), ex);
				_logger.Error(exception);
				Environment.Exit(1);
			}

			var itemAdapter = new SitecoreItemDataAdapter(templateItemData, module.DataStore, _logger);
			return ParseTemplates(itemAdapter);
		}

		protected override Guid[] ParseMultilistValue(string value)
		{
			_logger.Debug("[SitecoreTemplateReader] ParseMultilistValue");
			return value.Split(new []{@"\r",@"\n","|", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
				.Select(item => Guid.TryParse(item, out Guid result) ? result : Guid.Empty)
				.Where(item => item != Guid.Empty)
				.ToArray();
		}
	}
}
