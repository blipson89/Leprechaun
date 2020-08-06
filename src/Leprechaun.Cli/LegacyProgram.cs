using System;
using System.Diagnostics;
using System.IO;

namespace Leprechaun.Cli
{
	public class LegacyProgram
	{
		public static void Run(string[] args)
		{
			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Legacy", "Leprechaun.Console.exe"),
					Arguments = string.Join(' ', args),
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};
			proc.Start();
			while (!proc.StandardOutput.EndOfStream)
			{
				System.Console.WriteLine(proc.StandardOutput.ReadLine());
			}

			Console.WriteLine(proc.StandardError.ReadToEnd());
		}
	}
}
