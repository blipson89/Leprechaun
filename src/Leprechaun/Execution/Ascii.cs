using System;
using System.Linq;
using System.Reflection;

namespace Leprechaun.Execution
{
	public static class Ascii
	{
		public static void Leprechaun()
		{
			// credit 'jgs' http://ascii.co.uk/art/leprechaun
			// with minor alterations

			Console.ForegroundColor = ConsoleColor.Green;

			Console.Write(
@"  _                             _                       
 | |    ___ _ __  _ __ ___  ___| |__   __ _ _   _ _ __  
 | |   / _ \ '_ \| '__/ _ \/ __| '_ \ / _` | | | | '_ \ 
 | |__|  __/ |_) | | |  __/ (__| | | | (_| | |_| | | | |
 |_____\___| .__/|_|  \___|\___|_| |_|\__,_|\__,_|_| |_|
           |_|");
			var version = $"Version {GetVersion()}";
			int versionPadding = version.Length < 42 ? 42 - version.Length : 0;

			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine(new string(' ', versionPadding) + version);
			Console.WriteLine();

			WriteBow(@"                 @@                 ");
			WriteBow(@"                @><@               ");
			WriteBow(@"          ________)               ");
			WriteBow(@"         |        |              ");
			WriteBow(@"    _   _|===LI===|_            ");
			WriteBow(@"   / \_(____________)          ");
			WriteBow(@"   \  / (88 o  o 88)          ");
			WriteBow(@"    \/\  88:  7 :88`         ");
			WriteBow(@"     \/\ '88'=='88'         ");
			WriteBow(@"      \ \__'8888'__________");
			WriteBow(@"       \___<\""""/>_____/_/_");
			WriteBow(@"          /  ><  \       ");
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.Write(
@"        / __/--\__\     ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("(oO@OoO@@o@oO@@o)");
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine(
@"        '-.______.-'    /`""""""""""""""""""""""""""`\
          _|_||_|_     |                 |
       ___LI)||(LI___  |                 |
      (   ~~ || ~~   )  \               /
       `-----''-----`    '.___________.'
 ");
			Console.ResetColor();
		}

		private static void WriteBow(string precursor)
		{
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.Write(precursor);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write(".###@");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("@::;%");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine("%&&00\'");
			Console.ResetColor();
		}

		private static string GetVersion()
		{
			var attribute = Assembly
					.GetEntryAssembly()?
					.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false) as AssemblyFileVersionAttribute[];

			return attribute?.FirstOrDefault()?.Version ?? Assembly.GetEntryAssembly()?.GetName().Version.ToString(3);
		}
	}
}
