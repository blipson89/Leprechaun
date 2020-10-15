using System;
using System.Diagnostics;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{Path} ({Id})")]
	public class ItemInfo : IItemInfo
	{
		public string Path { get; set; }
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}