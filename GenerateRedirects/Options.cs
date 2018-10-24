using CommandLine;

namespace GenerateRedirects
{
	class Options
	{
		[Option('a', "appconfig", Required = true,
			HelpText = "The path to the app.config file which is to be modified")]
		public string AppConfigFile { get; set; }

		[Option('n', "assemblyname", Required = true,
			HelpText = "The name of the assembly for which a redirect binding should be generated")]
		public string AssemblyName { get; set; }

		[Option('i', "assemblyinfo", Required = true,
			HelpText = "The path to the AssemblyInfo.cs from which the new version is to be read from")]
		public string NewVersionAssemblyInfo { get; set; }

		/*
		// Omitting long name, default --verbose
		[Option(
			HelpText = "Prints all messages to standard output.")]
		public bool Verbose { get; set; }
		´*/
	}
}