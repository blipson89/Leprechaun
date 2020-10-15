using System;

namespace Leprechaun.Model
{
	public interface IItemInfo
	{
		string Path { get; set; }
		Guid Id { get; set; }
		string Name { get; set; }
	}
}