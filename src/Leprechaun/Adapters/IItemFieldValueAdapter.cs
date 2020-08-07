using System;

namespace Leprechaun.Adapters
{
	public interface IItemFieldValueAdapter
	{
		string Value { get; }
		Guid FieldId { get; }
	}
}