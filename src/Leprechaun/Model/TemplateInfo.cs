using System;
using System.Diagnostics;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{Path} ({Id})")]
	public class TemplateInfo
	{
		public string Path { get; set; }

		public Guid Id { get; set; }

		public string Name { get; set; }

		public string HelpText { get; set; }

		public Guid[] BaseTemplateIds { get; set; }

		public TemplateFieldInfo[] OwnFields { get; set; }
	}
}
