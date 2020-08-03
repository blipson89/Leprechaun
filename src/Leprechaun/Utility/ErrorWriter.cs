using System;

namespace Leprechaun.Utility
{
	public static class ErrorWriter
	{
		public static void WriteError(Exception ex)
		{
			WriteError(ex.Message, ex);
		}
		public static void WriteError(string message, Exception ex)
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"[ERROR]: {message}");
#if DEBUG
			Console.WriteLine(ex.StackTrace);
#endif
			Console.ForegroundColor = oldColor;
		}
		
	}
}
