using System;

namespace Zip
{
	internal sealed class ProgramException : Exception
	{
		public ProgramException(string message)
			: base(message)
		{ }
	}
}