using System;

namespace Leprechaun
{
	public static class Assert
	{
		public static void ArgumentNotNull(object arg, string argName)
		{
			if (arg != null)
				return;

			if (argName != null)
				throw new ArgumentNullException(argName);

			throw new ArgumentNullException();
		}

		public static void IsNotNull(object value, string valueName)
		{
			if (value != null)
				return;

			if(valueName != null)
				throw new InvalidOperationException($"Value for '{valueName}' cannot be null");
			throw new InvalidOperationException("Value cannot be null");
		}
	}
}
