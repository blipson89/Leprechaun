using System.Diagnostics;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{FullTypeName} ({Id})")]
	public class BaseCodeGenerationMetadata
	{
		protected readonly string _fullTypeName;

		protected BaseCodeGenerationMetadata(string fullTypeName, string rootNamespace)
		{
			RootNamespace = rootNamespace;
			_fullTypeName = fullTypeName;
		}

		/// <summary>
		/// A unique name for this template, usable as a name for a C# class. e.g. for "Foo Bar" this would be "FooBar"
		/// </summary>
		public virtual string CodeName => _fullTypeName.Contains(".") ? _fullTypeName.Substring(_fullTypeName.LastIndexOf('.') + 1) : _fullTypeName;

		/// <summary>
		/// Gets the full type name, qualified by namespace, of this template
		/// e.g. RootNamespace.RelativeNamespace.CodeName
		/// </summary>
		public virtual string FullTypeName => $"{RootNamespace}.{_fullTypeName}";

		public virtual string RootNamespace { get; }
	}

}
