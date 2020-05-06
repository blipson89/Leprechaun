using System;
using System.Collections.Generic;

namespace Leprechaun.Adapters
{
	public interface IItemDataAdapter
	{
		Guid TemplateId { get; }
		Guid Id { get; }
		string Name { get; }
		string Path { get; }
		IEnumerable<IItemFieldValueAdapter> SharedFields { get; }
		IEnumerable<IItemLanguageAdapter> UnversionedFields { get; }
		IEnumerable<IItemVersionAdapter> Versions { get; }
		IEnumerable<IItemDataAdapter> GetChildren();
	}
}