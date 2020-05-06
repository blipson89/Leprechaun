using System.Collections.Generic;

namespace Leprechaun.Filters
{
	public class TemplateTreeRoot : ITemplateTreeRoot
	{
		public TemplateTreeRoot(string name, string path)
		{
			Exclusions = new List<IPresetTreeExclusion>();
			Name = name;
			Path = path;
		}

		public IList<IPresetTreeExclusion> Exclusions { get; set; }
		public string Name { get; }
		public string Path { get; }
	}
}
