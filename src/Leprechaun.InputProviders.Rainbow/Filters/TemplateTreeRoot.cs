using System.Collections.Generic;
using Leprechaun.Filters;
using Rainbow.Storage;

namespace Leprechaun.InputProviders.Rainbow.Filters
{
	public class TemplateTreeRoot : TreeRoot, ITemplateTreeRoot
	{
		public TemplateTreeRoot(string name, string path) : base(name, path, "master")
		{
			Exclusions = new List<IPresetTreeExclusion>();
		}
		public TemplateTreeRoot(TreeRoot root) : base(root.Name, root.Path, root.DatabaseName)
		{
			FieldValueManipulator = root.FieldValueManipulator;
		}

		public IList<IPresetTreeExclusion> Exclusions { get; set; }
	}
}
