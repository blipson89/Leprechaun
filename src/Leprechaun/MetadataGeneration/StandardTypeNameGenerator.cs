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

		public StandardTypeNameGenerator(string namespaceRootPath)
		{
			if(namespaceRootPath == "/") throw new NotSupportedException("Namespace root cannot be /, please use a sub-path e.g. /sitecore/templates");

			_namespaceRoot = namespaceRootPath;
		}

		/// <summary>
		/// Calculates a relative namespace and type name for a template based on its relative path from the root namespace path
		/// </summary>
		public virtual string GetFullTypeName(string fullPath)
		{
			string name = fullPath.Replace(_namespaceRoot, string.Empty).Trim('/').Replace('/', '.');

			var nameParts = name.Split('/');

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
				string namespaceName = ConvertToIdentifier(name.Substring(0, name.LastIndexOf('.')));

				name = ConvertToIdentifier(string.Concat(namespaceName, ".", typeName));
			}

			return name;
		}

		/// <summary>
		/// Converts a string into a valid .NET identifier
		/// </summary>
		public virtual string ConvertToIdentifier(string name)
		{
			// allow for fields that start with a number
			if (char.IsDigit(name[0]))
				name = "_" + name;

			return Regex.Replace(name, "[^a-zA-Z0-9_\\.]+", string.Empty);
		}
	}
}
