using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leprechaun.Console.Variables
{
	public class MultiTenantHelixConventionVariablesReplacer : HelixConventionVariablesReplacer
	{
		public override Dictionary<string, string> GetVariables(string name)
		{
			var pieces = name.Split('.');

			if (pieces.Length < 2) return new Dictionary<string, string>();
			if (pieces.Length == 2) return base.GetVariables(name);

			var vars = new Dictionary<string, string>()
			{
				{"tenant", pieces[0]},
				{"layer", pieces[1]},
				{"module", pieces[2]}
			};

			if (pieces.Length >= 4)
			{
				vars.Add("moduleConfigName", pieces[3]);
			}
			else
			{
				// fallback if no third level name is used but variable is defined
				vars.Add("moduleConfigName", "Dev");
			}

			return vars;
		}
	}
}
