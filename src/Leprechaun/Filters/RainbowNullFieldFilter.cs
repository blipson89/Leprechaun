using System;

namespace Leprechaun.Filters
{
	// when we read templates we do not care about filtering their input in Rainbow; we can assume that whatever is written is legit.
	public class RainbowNullFieldFilter : Rainbow.Filtering.IFieldFilter
	{
		public bool Includes(Guid fieldId)
		{
			return true;
		}
	}
}
