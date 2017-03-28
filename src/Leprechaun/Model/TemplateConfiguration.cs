using System.Collections.Generic;
using Configy.Containers;

namespace Leprechaun.Model
{
	public class TemplateConfiguration
	{
		public TemplateConfiguration(IContainer configuration)
		{
			Configuration = configuration;
		}

		public IContainer Configuration { get; }
		public IEnumerable<TemplateInfo> Templates { get; set; }
	}
}
