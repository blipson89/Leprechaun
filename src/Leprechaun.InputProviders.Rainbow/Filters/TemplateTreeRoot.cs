using System.Collections.Generic;
using Leprechaun.Filters;
using Rainbow.Storage;

namespace Leprechaun.InputProviders.Rainbow.Filters
{
	public class TemplateTreeRoot : TreeRoot
	{
		public TemplateTreeRoot(string name, string path) : base(name, path, "master")
		{
			Exclusions = new List<IPresetTreeExclusion>();
		}

		public IList<IPresetTreeExclusion> Exclusions { get; set; }
	}
}
