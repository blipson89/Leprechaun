﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{CodeName} ({Id})")]
	public class TemplateFieldCodeGenerationMetadata
	{
		private string _type;
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

		public virtual string HelpText => GetHelpText();
		
		public virtual string Type
		{
			get => _type ?? Field.Type;
			set => _type = value;
		}

		public virtual string Source => Field.Source;

		public virtual string Section => Field.Section;

		public virtual int SortOrder => Field.SortOrder;

		public virtual IDictionary<Guid, string> AllFields => Field.AllFields;

		/// <summary>
		/// A unique name for this template field, usable as a C# identifier (e.g. property name)
		/// </summary>
		public virtual string CodeName { get; }

		protected virtual string GetHelpText()
		{
			if(!string.IsNullOrWhiteSpace(Field.HelpText))
				return Field.HelpText;

			string fieldName = string.IsNullOrEmpty(DisplayName) ? Name : DisplayName;

			return $"Represents the {fieldName} field ({Id}).";
		}
	}
}
