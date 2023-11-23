﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Leprechaun.Model
{
	[DebuggerDisplay("{FullTypeName} ({Id})")]
	public class TemplateCodeGenerationMetadata : BaseCodeGenerationMetadata
	{
		
		public TemplateCodeGenerationMetadata(TemplateInfo templateInfo, string fullTypeName, string rootNamespace, IEnumerable<TemplateFieldCodeGenerationMetadata> ownFields):
			base(fullTypeName,rootNamespace)
		{
			TemplateInfo = templateInfo;
			OwnFields = ownFields.ToArray();
		}

		public virtual TemplateInfo TemplateInfo { get; }
		
		public virtual string Path => TemplateInfo.Path;

		public virtual Guid Id => TemplateInfo.Id;

		public virtual string Name => TemplateInfo.Name;

		public virtual string HelpText => string.IsNullOrWhiteSpace(TemplateInfo.HelpText) ? $"Represents the {TemplateInfo.Name} field ({Id})." : TemplateInfo.HelpText;

		
		/// <summary>
		/// Gets a namespace-formatted relative path from the root template path to this template
		/// e.g. if root is /Foo, and this template's path is /Foo/Bar/Baz/Quux, this would be "Bar.Baz"
		/// </summary>
		public virtual string RelativeNamespace => _fullTypeName.Contains(".") ? _fullTypeName.Substring(0, _fullTypeName.LastIndexOf('.')) : string.Empty;

		/// <summary>
		/// Gets the full namespace for the template (e.g. RootNamespace.RelativeNamespace)
		/// </summary>
		public virtual string Namespace
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(RelativeNamespace))
				{
					return $"{RootNamespace}.{RelativeNamespace}";
				}
				return RootNamespace;
			}
		}
		
		/// <summary>
		/// The template's fields that should get passed to code generation
		/// </summary>
		public virtual TemplateFieldCodeGenerationMetadata[] OwnFields { get; }

		/// <summary>
		/// All known immediate templates implemented by this type (transitive inheritance is not included eg a -> b -> c, will have b but not c for a)
		/// </summary>
		public virtual IList<TemplateCodeGenerationMetadata> BaseTemplates { get; } = new List<TemplateCodeGenerationMetadata>();

		/// <summary>
		/// All fields that make up this template, including all base templates' fields
		/// </summary>
		public virtual IEnumerable<TemplateFieldCodeGenerationMetadata> AllFields
		{
			get
			{
				var templates = new Queue<TemplateCodeGenerationMetadata>();
				templates.Enqueue(this);

				var knownFields = new HashSet<Guid>();

				while (templates.Count > 0)
				{
					var current = templates.Dequeue();
					foreach (var field in current.OwnFields)
					{
						if(knownFields.Contains(field.Id)) continue;

						knownFields.Add(field.Id);

						yield return field;
					}

					foreach (var baseTemplate in current.BaseTemplates)
					{
						templates.Enqueue(baseTemplate);
					}
				}
			}
		}
	}
}
