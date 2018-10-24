using System;

namespace GenerateRedirects
{
	sealed class ProgramException
		: Exception
	{
		public ProgramException(string message)
			: base(message)
		{}
	}
}