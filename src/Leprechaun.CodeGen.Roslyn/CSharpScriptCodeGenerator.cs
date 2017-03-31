using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Leprechaun.Logging;
using Leprechaun.Model;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Leprechaun.CodeGen.Roslyn
{
	public class CSharpScriptCodeGenerator : ICodeGenerator
	{
		private readonly string _outputFile;
		private readonly ILogger _logger;
		private readonly string[] _scripts;

		public CSharpScriptCodeGenerator(string scripts, string outputFile, ILogger logger)
		{
			_outputFile = outputFile;
			_logger = logger;

			// ReSharper disable once VirtualMemberCallInConstructor
			_scripts = ProcessScripts(scripts);
		}

		public void GenerateCode(ConfigurationCodeGenerationMetadata metadata)
		{
			var tasks = _scripts
				.Select(script =>
				{
					var codegenContext = new CSharpScriptCodeGeneratorContext(metadata, _logger, _outputFile);

					return GetScript(script)
						.RunAsync(codegenContext)
						.ContinueWith(task => EmitCode(codegenContext));
				})
				.ToArray<Task>();

			Task.WaitAll(tasks);
		}

		protected virtual async Task EmitCode(CSharpScriptCodeGeneratorContext state)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(state.OutputFileName));

			using (var file = File.Open(state.OutputFileName, FileMode.Create, FileAccess.Write))
			{
				using (var writer = new StreamWriter(file))
				{
					await writer.WriteAsync(state.Code.ToString());
				}
			}
		}

		protected virtual Script GetScript(string scriptFileName)
		{
			return CSharpScriptCache.GetScript(scriptFileName);
		}

		protected virtual string[] ProcessScripts(string scripts)
		{
			return scripts
				.Split(',')
				.Select(script =>
				{
					var trimmed = script.Trim();

					if(!File.Exists(trimmed)) throw new FileNotFoundException($"Code generation script '{trimmed}' was not found on disk.");

					return trimmed;
				})
				.ToArray();
		}
	}
}
