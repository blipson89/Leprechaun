using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Configy.Containers;
using Leprechaun.Filters;
using Rainbow.Storage;

namespace Leprechaun.InputProviders.Rainbow.Filters
{
	public class StandardTemplatePredicate : BaseTemplatePredicate, ITemplatePredicate, ITreeRootFactory
	{
		public StandardTemplatePredicate(XmlNode configNode, IContainer configuration, string rootNamespace) : base(configNode, configuration, rootNamespace)
		{
			
		}

		public virtual TreeRoot[] GetRootPaths()
		{
			return _includeEntries.Select(i => i as TreeRoot).ToArray();
		}


		protected override ITemplateTreeRoot CreateTreeRoot(string name, string path) => new TemplateTreeRoot(name, path);

		IEnumerable<TreeRoot> ITreeRootFactory.CreateTreeRoots()
		{
			return _includeEntries.Select(i => i as TreeRoot);
		}
	}
}
