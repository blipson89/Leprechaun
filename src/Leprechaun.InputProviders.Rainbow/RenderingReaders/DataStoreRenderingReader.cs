using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Leprechaun.Adapters;
using Leprechaun.Filters;
using Leprechaun.InputProviders.Rainbow.Adapters;
using Leprechaun.InputProviders.Rainbow.Filters;
using Leprechaun.Model;
using Leprechaun.RenderingReaders;
using Leprechaun.TemplateReaders;
using Rainbow.Storage;

namespace Leprechaun.InputProviders.Rainbow.RenderingReaders
{
	public class DataStoreRenderingReader : IRenderingReader
	{
		internal static readonly Guid ControllerRenderingTemplateId = new Guid("{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}");
		internal static readonly Guid ViewRenderingTemplateId = new Guid("{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}");

		private IDataStore _dataStore;

		public DataStoreRenderingReader(XmlNode configNode, IDataStore dataStore) 
		{ 
			_dataStore = dataStore;
		}

		public RenderingInfo[] GetRenderings(ITemplatePredicate predicate)
		{
			if (predicate is StandardTemplatePredicate standardPredicate)
			{
				return GetRenderings(standardPredicate.GetRootPaths());
			}
			return new RenderingInfo[0];
		}

		public RenderingInfo[] GetRenderings(params TreeRoot[] rootPaths)
		{
			return rootPaths
				.AsParallel()
				.SelectMany(root =>
				{
					var rootItem = _dataStore.GetByPath(root.Path, root.DatabaseName)?.Select(itemData => new RainbowItemDataAdapter(itemData));

					if (rootItem == null) return Enumerable.Empty<RenderingInfo>();

					// because a path could match more than one item we have to SelectMany again
					return rootItem.SelectMany(ParseRenderings);
				})
				.ToArray();
		}

		protected virtual IEnumerable<RenderingInfo> ParseRenderings(IItemDataAdapter root)
		{
			var processQueue = new Queue<IItemDataAdapter>();

			processQueue.Enqueue(root);

			while (processQueue.Count > 0)
			{
				var currentTemplate = processQueue.Dequeue();

				// if it's a viewrendering or controller rendering we parse it and skip adding children (nested templates not really allowed)
				if (currentTemplate.TemplateId == ControllerRenderingTemplateId || currentTemplate.TemplateId == ViewRenderingTemplateId)
				{
					yield return ParseRendering(currentTemplate);
					continue;
				}

				// it's not a template (e.g. a template folder) so we want to scan its children for templates to parse
				var children = currentTemplate.GetChildren();
				foreach (var child in children)
				{
					processQueue.Enqueue(child);
				}
			}
		}

		protected virtual RenderingInfo ParseRendering(IItemDataAdapter templateItem)
		{
			if (templateItem == null) throw new ArgumentException("Template item passed to parse was null", nameof(templateItem));
			if (templateItem.TemplateId != ControllerRenderingTemplateId) throw new ArgumentException("Template item passed to parse was not a Template item", nameof(templateItem));

			var result = new RenderingInfo()
			{
				Id = templateItem.Id,
				Name = templateItem.Name,
				Path = templateItem.Path
			};

			return result;
		}

	}
}