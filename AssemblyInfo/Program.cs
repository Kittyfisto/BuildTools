using System;
using AssemblyInfo.Applications;
using CommandLine;

namespace AssemblyInfo
{
	class Program
	{
		static int Main(string[] args)
		{
			try
			{
				Run(args);

				return 0;
			}
			catch (ProgramException e)
			{
				Console.WriteLine("ERROR: {0}", e.Message);
				return -1;
			}
			catch (Exception e)
			{
				Console.WriteLine("ERROR: {0}", e);
				return -2;
			}
		}

		private static void Run(string[] args)
		{
			Parser.Default.ParseArguments<PatchInternalsVisibleToOptions>(args)
			      .WithParsed<PatchInternalsVisibleToOptions>(PatchInternalsVisibleTo.Run);
		}
	}
}
