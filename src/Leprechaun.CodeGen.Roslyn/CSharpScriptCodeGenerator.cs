using System;
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
		private readonly ILogger _logger;
		private readonly Script[] _scripts;

		public CSharpScriptCodeGenerator(string scripts, string outputFile, ILogger logger)
		{
			_outputFile = outputFile;
			_logger = logger;

			// ReSharper disable once VirtualMemberCallInConstructor
			_scripts = ProcessScripts(scripts);
		}

		public virtual void GenerateCode(ConfigurationCodeGenerationMetadata metadata)
		{
			var tasks = _scripts
				.Select(script =>
				{
					var codegenContext = new CSharpScriptCodeGeneratorContext(metadata, _logger);

					try
					{
						return script
							.RunAsync(codegenContext, catchException: exception => throw exception)
							.ContinueWith(task => codegenContext);
					}
					catch (Exception ex)
					{
						throw new InvalidOperationException($"Error executing {script.Options.FilePath}", ex);
					}
				})
				.ToArray();

			Task.WhenAll(tasks)
				.ContinueWith(task => EmitCode(task.Result))
				.Wait();
		}

		protected virtual async Task EmitCode(IEnumerable<CSharpScriptCodeGeneratorContext> states)
		{
			var code = string.Join(Environment.NewLine, states.Select(state => state.Code.ToString()));

			if (File.Exists(_outputFile))
			{
				var existingCode = File.ReadAllText(_outputFile);

				if (existingCode.Equals(code, StringComparison.Ordinal))
				{
					_logger.Debug($"Skipped up to date {_outputFile}.");
					return;
				}
			}

			Directory.CreateDirectory(Path.GetDirectoryName(_outputFile));

			using (var file = File.Open(_outputFile, FileMode.Create, FileAccess.Write))
			{
				using (var writer = new StreamWriter(file))
				{
					await writer.WriteAsync(code);
				}
			}

			_logger.Info($"Wrote {_outputFile}.");
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
					var scriptPath = script.Trim();

					// not an absolute drive path, in which case we want to prepend the current exe path to it
					if (scriptPath[1] != ':')
					{
						scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptPath);
					}

					if(!File.Exists(scriptPath)) throw new FileNotFoundException($"Code generation script '{scriptPath}' was not found on disk.");

					return GetScript(scriptPath);
				})
				.ToArray();
		}
	}
}
