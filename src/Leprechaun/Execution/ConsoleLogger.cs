using System;
using System.Text;
using Leprechaun.Logging;

namespace Leprechaun.Execution
{
	public class ConsoleLogger : ILogger
	{
		public void Info(string message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public void Debug(string message)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public void Warn(string message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public void Error(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public void Error(Exception exception)
		{
			Error(FormatExceptionAsText(exception));
		}

		public void Error(string message, Exception exception)
		{
			Error(message);
			Error("--- Full error below ---");
			Error(exception);
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
