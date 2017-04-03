using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Leprechaun.Logging;
using Leprechaun.Model;
using Microsoft.CodeAnalysis.Scripting;

namespace Leprechaun.CodeGen.Roslyn
{
	public class CSharpScriptCodeGenerator : ICodeGenerator
	{
		private readonly string _outputFile;
		private readonly string _rootNamespace;
		private readonly ILogger _logger;
		private readonly Script[] _scripts;

		public CSharpScriptCodeGenerator(string scripts, string outputFile, string rootNamespace, ILogger logger)
		{
			_outputFile = outputFile;
			_rootNamespace = rootNamespace;
			_logger = logger;

			// ReSharper disable once VirtualMemberCallInConstructor
			_scripts = ProcessScripts(scripts);
		}

		public void GenerateCode(ConfigurationCodeGenerationMetadata metadata)
		{
			var tasks = _scripts
				.Select(script =>
				{
					var codegenContext = new CSharpScriptCodeGeneratorContext(metadata, _logger, _rootNamespace);

					return script
						.RunAsync(codegenContext)
						.ContinueWith(task => codegenContext);
				})
				.ToArray();

			Task.WhenAll(tasks)
				.ContinueWith(task => EmitCode(task.Result))
				.Wait();
		}

		protected virtual async Task EmitCode(IEnumerable<CSharpScriptCodeGeneratorContext> states)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(_outputFile));

			using (var file = File.Open(_outputFile, FileMode.Create, FileAccess.Write))
			{
				using (var writer = new StreamWriter(file))
				{
					foreach (var state in states)
					{
						await writer.WriteAsync(state.Code.ToString());
					}
				}
			}
		}

		protected virtual Script GetScript(string scriptFileName)
		{
			return CSharpScriptCache.GetScript(scriptFileName);
		}

		protected virtual Script[] ProcessScripts(string scripts)
		{
			return scripts
				.Split(',')
				.AsParallel()
				.Select(script =>
				{
					var trimmed = script.Trim();

					if(!File.Exists(trimmed)) throw new FileNotFoundException($"Code generation script '{trimmed}' was not found on disk.");

					return GetScript(trimmed);
				})
				.ToArray();
		}
	}
}
