using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{Path} ({Id})")]
	public class TemplateFieldInfo
	{
		public TemplateFieldInfo()
		{
		}

		public Guid Id { get; set; }

		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Path { get; set; }

		public string HelpText { get; set; }

		public string Type { get; set; }

		public string Source { get; set; }

		public string Section { get; set; }

		public int SortOrder { get; set; }

		public IDictionary<Guid, string> AllFields { get; set; } = new Dictionary<Guid, string>();
	}
}
