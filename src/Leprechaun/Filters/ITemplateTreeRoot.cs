using System.Collections.Generic;

namespace Leprechaun.Filters
{
	public interface ITemplateTreeRoot
	{
		IList<IPresetTreeExclusion> Exclusions { get; set; }
		string Name { get; }
		string Path { get; }
	}
}