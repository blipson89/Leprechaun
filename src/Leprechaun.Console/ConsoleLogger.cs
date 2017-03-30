using System;
using System.Text;
using Leprechaun.Logging;

namespace Leprechaun.Console
{
	public class ConsoleLogger : ILogger
	{
		public void Info(string message)
		{
			System.Console.ForegroundColor = ConsoleColor.White;
			System.Console.WriteLine(message);
			System.Console.ResetColor();
		}

		public void Debug(string message)
		{
			System.Console.ForegroundColor = ConsoleColor.DarkGray;
			System.Console.WriteLine(message);
			System.Console.ResetColor();
		}

		public void Warn(string message)
		{
			System.Console.ForegroundColor = ConsoleColor.Yellow;
			System.Console.WriteLine(message);
			System.Console.ResetColor();
		}

		public void Error(string message)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
			System.Console.WriteLine(message);
			System.Console.ResetColor();
		}

		public void Error(Exception exception)
		{
			Error(FormatExceptionAsText(exception));
		}

		protected virtual string FormatExceptionAsText(Exception exception)
		{
			var exMessage = new StringBuilder();
			exMessage.AppendFormat("ERROR: {0} ({1})", exception.Message, exception.GetType().FullName);
			exMessage.AppendLine();

			if (exception.StackTrace != null)
				exMessage.Append(exception.StackTrace.Trim());
			else
				exMessage.Append("No stack trace available.");

			exMessage.AppendLine();

			WriteInnerExceptionAsText(exception.InnerException, exMessage);

			return exMessage.ToString();
		}

		protected virtual void WriteInnerExceptionAsText(Exception innerException, StringBuilder exMessage)
		{
			if (innerException == null) return;

			exMessage.AppendLine();
			exMessage.AppendLine("INNER EXCEPTION");
			exMessage.AppendFormat("{0} ({1})", innerException.Message, innerException.GetType().FullName);
			exMessage.AppendLine();

			if (innerException.StackTrace != null)
				exMessage.Append(innerException.StackTrace.Trim());
			else
				exMessage.Append("No stack trace available.");

			WriteInnerExceptionAsText(innerException.InnerException, exMessage);

			exMessage.AppendLine();
		}
	}
}
