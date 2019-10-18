using System;
using System.Linq;
using System.Reflection;

namespace Leprechaun.Console
{
	static class Ascii
	{
		public static void Leprechaun()
		{
			// credit 'jgs' http://ascii.co.uk/art/leprechaun
			// with minor alterations

			System.Console.ForegroundColor = ConsoleColor.Green;

			System.Console.Write(
@"  _                             _                       
 | |    ___ _ __  _ __ ___  ___| |__   __ _ _   _ _ __  
 | |   / _ \ '_ \| '__/ _ \/ __| '_ \ / _` | | | | '_ \ 
 | |__|  __/ |_) | | |  __/ (__| | | | (_| | |_| | | | |
 |_____\___| .__/|_|  \___|\___|_| |_|\__,_|\__,_|_| |_|
           |_|");
			var version = $"Version {GetVersion()}";
			int versionPadding = version.Length < 42 ? 42 - version.Length : 0;

			System.Console.ForegroundColor = ConsoleColor.DarkGreen;
			System.Console.WriteLine(new string(' ', versionPadding) + version);
			System.Console.WriteLine();

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
			System.Console.ForegroundColor = ConsoleColor.DarkGreen;
			System.Console.Write(
@"        / __/--\__\     ");
			System.Console.ForegroundColor = ConsoleColor.Yellow;
			System.Console.WriteLine("(oO@OoO@@o@oO@@o)");
			System.Console.ForegroundColor = ConsoleColor.DarkGreen;
			System.Console.WriteLine(
@"        '-.______.-'    /`""""""""""""""""""""""""""`\
          _|_||_|_     |                 |
       ___LI)||(LI___  |                 |
      (   ~~ || ~~   )  \               /
       `-----''-----`    '.___________.'
 ");
			System.Console.ResetColor();
		}

		private static void WriteBow(string precursor)
		{
			System.Console.ForegroundColor = ConsoleColor.DarkGreen;
			System.Console.Write(precursor);
			System.Console.ForegroundColor = ConsoleColor.Red;
			System.Console.Write(".###@");
			System.Console.ForegroundColor = ConsoleColor.Green;
			System.Console.Write("@::;%");
			System.Console.ForegroundColor = ConsoleColor.Blue;
			System.Console.WriteLine("%&&00\'");
			System.Console.ResetColor();
		}

		private static string GetVersion()
		{
			var attribute = Assembly
					.GetEntryAssembly()
					.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false) as AssemblyInformationalVersionAttribute[];

			return attribute?.FirstOrDefault()?.InformationalVersion ?? Assembly.GetEntryAssembly().GetName().Version.ToString(3);
		}
	}
}
