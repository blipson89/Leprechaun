using System.Collections.Generic;
using Rainbow.Storage;

namespace Leprechaun.Filters
{
	public class TemplateTreeRoot : TreeRoot
	{
		public TemplateTreeRoot(string path) : base(null, path, "master")
		{
			Exclusions = new List<IPresetTreeExclusion>();

			Name = path.Substring(path.LastIndexOf('/') + 1);
		}

		public IList<IPresetTreeExclusion> Exclusions { get; set; }
	}
}
