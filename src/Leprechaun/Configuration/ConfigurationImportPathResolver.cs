using System;
using System.Collections.Generic;
using System.IO;
using Leprechaun.Logging;

namespace Leprechaun.Configuration
{
	/// <summary>
	/// Resolves imported config files' paths.
	/// This amounts to an efficient glob implementation, which somehow seems to not exist for C#
	/// </summary>
	public class ConfigurationImportPathResolver
	{
		private readonly ILogger _logger;

		public ConfigurationImportPathResolver(ILogger logger)
		{
			_logger = logger;
		}

		public virtual string[] ResolveImportPaths(string inputPath)
		{
			// normalize input path 
			inputPath = inputPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).Trim();

			// check for wildcards
			if (inputPath.IndexOf('*') < 0)
			{
				// non-wildcard path is taken literally (e.g. ../foo.config or c:\foo.config)
				if(!File.Exists(inputPath)) return new string[0];

				return new[] { inputPath };
			}

			// for wildcard parsing we have to get fancier

			var wildcardIndex = inputPath.IndexOf('*');

			// get the highest non-wildcard path segment; we'll iterate for the filter under there
			string rootDir = inputPath.Substring(0, wildcardIndex);

			// handle partial wildcards e.g. Dir*
			if (!rootDir.EndsWith($"{Path.DirectorySeparatorChar}"))
			{
				rootDir = rootDir.Substring(0, rootDir.LastIndexOf(Path.DirectorySeparatorChar));
			}

			var wildcardSegments = new Queue<string>(inputPath.Substring(wildcardIndex).Split(Path.DirectorySeparatorChar));

			var pathCandidates = new Queue<string>();
			pathCandidates.Enqueue(rootDir);

			// process all wildcard segments except the last (these would be directories)
			// e.g. /foo/*/ba*/r.config or /foo/*/*.config
			while (wildcardSegments.Count > 1)
			{
				var currentSegment = wildcardSegments.Dequeue();

				var newPathCandidates = new Queue<string>();

				while (pathCandidates.Count > 0)
				{
					var currentPathCandidate = pathCandidates.Dequeue();

					string[] directories;

					if ("**".Equals(currentSegment, StringComparison.Ordinal))
					{
						// recursive inclusion
						directories = Directory.GetDirectories(currentPathCandidate, "*", SearchOption.AllDirectories);
					}
					else
					{
						// normal filter inclusion
						directories = Directory.GetDirectories(currentPathCandidate, currentSegment, SearchOption.TopDirectoryOnly);
					}

					foreach (var directory in directories) newPathCandidates.Enqueue(directory);
				}

				// replace existing candidates with new candidates that are lower down a wildcard element
				pathCandidates = newPathCandidates;
			}

			var results = new List<string>();
			var finalSegment = wildcardSegments.Dequeue();

			// finally we have a queue with the last segment in it - in this case, we want to evaluate against files, not directories
			while (pathCandidates.Count > 0)
			{
				var currentPathCandidate = pathCandidates.Dequeue();

				if (!Directory.Exists(currentPathCandidate))
				{
					_logger.Warn($"The import path {currentPathCandidate} did not exist, and will be skipped!");
					continue;
				}

				var currentPathFiles = Directory.GetFiles(currentPathCandidate, finalSegment, SearchOption.TopDirectoryOnly);

				results.AddRange(currentPathFiles);
			}

			return results.ToArray();
		}
	}
}
