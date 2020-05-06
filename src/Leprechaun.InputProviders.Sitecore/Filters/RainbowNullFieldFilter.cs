using System;
using Rainbow.Filtering;

namespace Leprechaun.InputProviders.Sitecore.Filters
{
	// when we read templates we do not care about filtering their input in Rainbow; we can assume that whatever is written is legit.
	public class RainbowNullFieldFilter : IFieldFilter
	{
		public bool Includes(Guid fieldId)
		{
			return true;
		}
	}
}
