using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Leprechaun.CodeGen.Roslyn
{
	public static class CSharpScriptCache
	{
		private static readonly ConcurrentDictionary<string, Script> ScriptCache = new ConcurrentDictionary<string, Script>(StringComparer.OrdinalIgnoreCase);

		public static Script GetScript(string fileName)
		{
			// TODO: this should decache files if watching, when the templates' timestamp changes
			return ScriptCache.GetOrAdd(fileName, key =>
			{
				var scriptOptions = ScriptOptions.Default
					.WithFilePath(fileName);

				return CSharpScript.Create(File.ReadAllText(fileName), scriptOptions, typeof(CSharpScriptCodeGeneratorContext));
			});
		}
	}
}
