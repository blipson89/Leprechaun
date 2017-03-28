using System;
using System.Runtime.Serialization;

namespace Leprechaun.Validation
{
	[Serializable]
	public class ArchitectureValidationException : Exception
	{
		public ArchitectureValidationException()
		{
		}

		public ArchitectureValidationException(string message) : base(message)
		{
		}

		public ArchitectureValidationException(string message, Exception inner) : base(message, inner)
		{
		}

		protected ArchitectureValidationException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}
