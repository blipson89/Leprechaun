using System;
using System.Diagnostics;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{CodeName} ({Id})")]
	public class TemplateFieldCodeGenerationMetadata
	{
		public TemplateFieldCodeGenerationMetadata(TemplateFieldInfo field, string codeName)
		{
			Field = field;
			CodeName = codeName;
		}

		public virtual TemplateFieldInfo Field { get; }

		public virtual Guid Id => Field.Id;

		public virtual string Name => Field.Name;

		public virtual string DisplayName => Field.DisplayName;

		public virtual string Path => Field.Path;

		public virtual string HelpText => string.IsNullOrWhiteSpace(Field.HelpText) ? $"Represents the {DisplayName} field ({Id})." : Field.HelpText;

		public virtual string Type => Field.Type;

		public virtual string Source => Field.Source;

		public virtual string Section => Field.Section;

		public virtual int SortOrder => Field.SortOrder;

		/// <summary>
		/// A unique name for this template field, usable as a C# identifier (e.g. property name)
		/// </summary>
		public virtual string CodeName { get; }
	}
}
