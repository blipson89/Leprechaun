using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{RelativeNamespace}.{CodeName} ({Id})")]
	public class TemplateCodeGenerationMetadata
	{
		private readonly string _fullTypeName;

		public TemplateCodeGenerationMetadata(TemplateInfo templateInfo, string fullTypeName, IEnumerable<TemplateFieldCodeGenerationMetadata> ownFields)
		{
			TemplateInfo = templateInfo;
			_fullTypeName = fullTypeName;
			OwnFields = ownFields.ToArray();
		}

		public virtual TemplateInfo TemplateInfo { get; }

		public virtual string Path => TemplateInfo.Path;

		public virtual Guid Id => TemplateInfo.Id;

		public virtual string Name => TemplateInfo.Name;

		public virtual string HelpText => TemplateInfo.HelpText;

		/// <summary>
		/// A unique name for this template, usable as a name for a C# class. e.g. for "Foo Bar" this would be "FooBar"
		/// </summary>
		public virtual string CodeName => _fullTypeName.Contains(".") ? _fullTypeName.Substring(_fullTypeName.LastIndexOf('.') + 1) : _fullTypeName;

		/// <summary>
		/// Gets a namespace-formatted relative path from the root template path to this template
		/// e.g. if root is /Foo, and this template's path is /Foo/Bar/Baz/Quux, this would be "Bar.Baz"
		/// </summary>
		public virtual string RelativeNamespace => _fullTypeName.Contains(".") ? _fullTypeName.Substring(0, _fullTypeName.LastIndexOf('.')) : string.Empty;

		/// <summary>
		/// The template's fields that should get passed to code generation
		/// </summary>
		public virtual TemplateFieldCodeGenerationMetadata[] OwnFields { get; }

		/// <summary>
		/// All known immediate templates implemented by this type (transitive inheritance is not included eg a -> b -> c, will have b but not c for a)
		/// </summary>
		public virtual IList<TemplateCodeGenerationMetadata> BaseTemplates { get; } = new List<TemplateCodeGenerationMetadata>();
	}
}
