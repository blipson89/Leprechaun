using System;

namespace Leprechaun.Model
{
	public class TemplateInfo : ItemInfo
	{
		public string HelpText { get; set; }

		public Guid[] BaseTemplateIds { get; set; }

		public TemplateFieldInfo[] OwnFields { get; set; }
	}
}
