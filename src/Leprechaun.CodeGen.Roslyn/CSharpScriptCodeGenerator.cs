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

		protected virtual void EmitCode(IEnumerable<CSharpScriptCodeGeneratorContext> states)
		{
			var code = string.Join(Environment.NewLine, states.Select(state => state.Code.ToString()));

			var files = _outputFile.Split(',');
			var tasks = files.Select(fi => Task.Run(() => WriteFile(fi.Trim(), code)));
			Task.WhenAll(tasks).Wait();
		}

		protected virtual void WriteFile(string filePath, string contents)
		{
			if (File.Exists(filePath))
			{
				var existingCode = File.ReadAllText(filePath);

				if (existingCode.Equals(contents, StringComparison.Ordinal))
				{
					_logger.Debug($"Skipped up to date {filePath}.");
					return;
				}
			}

			Directory.CreateDirectory(Path.GetDirectoryName(filePath));

			using (var file = File.Open(filePath, FileMode.Create, FileAccess.Write))
			using (var writer = new StreamWriter(file))
			{
				writer.Write(contents);
			}

			_logger.Info($"Wrote {filePath}.");
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

					ResolveScriptPath(scriptPath);

					return GetScript(scriptPath);
				})
				.ToArray();
		}

		protected virtual string ResolveScriptPath(string path)
		{
			if (File.Exists(path))
				return path;
			var directoryRelativePath = "";
			var exeRelativePath = "";

			if (!Path.IsPathRooted(path))
			{
				directoryRelativePath = Path.Combine(Directory.GetCurrentDirectory(), path);
				if (File.Exists(directoryRelativePath))
					return directoryRelativePath;

				exeRelativePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
				if (File.Exists(exeRelativePath))
					return exeRelativePath;
			}
			throw new FileNotFoundException($"Code generation script '{path}' was not found on disk.\n\nLooked in '{directoryRelativePath}' and '{exeRelativePath}'");
		}
	}
}
