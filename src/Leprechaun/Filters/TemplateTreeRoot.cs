using System.Collections.Generic;
using Rainbow.Storage;

namespace Leprechaun.Filters
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
