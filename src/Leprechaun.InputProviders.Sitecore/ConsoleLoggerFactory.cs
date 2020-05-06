using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Leprechaun.InputProviders.Sitecore
{
	public class ConsoleLoggerFactory : LoggerFactory, ILoggerFactory
	{
		public ConsoleLoggerFactory() : base(new[] {new ConsoleLoggerProvider(new ConsoleLoggerSettings()),})
		{

		}
	}
}