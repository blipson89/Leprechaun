using System;
using System.Text.RegularExpressions;

namespace Leprechaun.MetadataGeneration
{
	/// <summary>
	/// Generates class names and relative namespaces for code generation types
	/// </summary>
	public class StandardTypeNameGenerator : ITypeNameGenerator
	{
		private readonly string _namespaceRoot;
		private readonly bool _keepLeadingUnderscores;

		public StandardTypeNameGenerator(string namespaceRootPath, bool keepLeadingUnderscores)
		{
			if(namespaceRootPath == "/") throw new NotSupportedException("Namespace root cannot be /, please use a sub-path e.g. /sitecore/templates");
			_keepLeadingUnderscores = keepLeadingUnderscores;

			_namespaceRoot = namespaceRootPath;
		}

		/// <summary>
		/// Calculates a relative namespace and type name for a template based on its relative path from the root namespace path
		/// </summary>
		public virtual string GetFullTypeName(string fullPath)
		{
			string name = fullPath.Trim('/');
			if (fullPath.StartsWith(_namespaceRoot, StringComparison.OrdinalIgnoreCase))
			{
				name = name.Substring(_namespaceRoot.Length);
			}

			var nameParts = name.Split('/');

			// Check for namespace elements that begin with a number (invalid for C#) and replace with a leading underscore
			for (int cnt = 0; cnt < nameParts.Length; cnt++)
			{
				string namePart = nameParts[cnt];
				int v;
				if (int.TryParse(namePart.Substring(0, 1), out v))
				{
					namePart = "_" + namePart;
				}

				nameParts[cnt] = namePart;
			}

			name = string.Join(".", nameParts);

			if (name.Contains("."))
			{
				string typeName = name.Substring(name.LastIndexOf('.') + 1);

				if (!_keepLeadingUnderscores)
					typeName = typeName.TrimStart('_');

				string namespaceName = ConvertToIdentifier(name.Substring(0, name.LastIndexOf('.')));

				name = ConvertToIdentifier(string.Concat(namespaceName, ".", typeName));
			}
			else
			{
				name = ConvertToIdentifier(name);
			}

			return name;
		}

		/// <summary>
		/// Converts a string into a valid .NET identifier
		/// </summary>
		public virtual string ConvertToIdentifier(string name)
		{
			// Desnakeify case if it exists (e.g. foo_bar -> "foo bar")
			name = Regex.Replace(name, @"(\w)_(\w)", "$1 $2");

			if (!_keepLeadingUnderscores)
				name = name.TrimStart('_');

			// Uppercase any non-capitalized words (e.g. 'lord flowers' -> 'Lord Flowers')
			// this makes identifiers Pascal Case as .NET expects
			name = Regex.Replace(name, "^([a-z])", match => match.Value.ToUpperInvariant()); // first letter
			name = Regex.Replace(name, " ([a-z])", match => match.Value.ToUpperInvariant()); // subsequent words

			// Normalize three or more letter acronyms to C#ean casing (e.g. XML -> Xml)
			name = Regex.Replace(name, "([A-Z]{3,})", match => match.Value[0] + match.Value.Substring(1).ToLowerInvariant());

			// allow for fields that start with a number (this is not allowed as an identifier)
			if (char.IsDigit(name[0]))
				name = "_" + name;

			// replace invalid chars for an identifier with nothing (removes spaces, etc)
			return Regex.Replace(name, "[^a-zA-Z0-9_\\.]+", string.Empty);
		}
	}
}
