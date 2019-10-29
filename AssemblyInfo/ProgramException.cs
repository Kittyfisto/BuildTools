using System;

namespace AssemblyInfo
{
	public class ProgramException
		: Exception
	{
		public ProgramException(string message)
			: base(message)
		{ }
	}
}