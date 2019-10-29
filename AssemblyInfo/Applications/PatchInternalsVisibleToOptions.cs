using System.Collections.Generic;
using CommandLine;

namespace AssemblyInfo.Applications
{
	[Verb("patch-internals-visible-to", HelpText = "Patch the contents of an AssemblyInfo.cs file to ensure that an [InternalsVisibleTo] declaration mentions the correct public key token")]
	public sealed class PatchInternalsVisibleToOptions
	{
		[Option("assembly-info",
			Required = true,
			HelpText = "The path to the AssemblyInfo.cs file which shall be patched")]
		public string AssemblyInfoPath { get; set; }

		[Option("assemblies",
			Required = true,
			HelpText = "The list of assemblies who's [InternalsVisibleTo] declaration shall be patched")]
		public IEnumerable<string> Assemblies { get; set; }

		[Option("key-path",
			HelpText = "The path to the .snk file from which the public key token shall be extracted")]
		public string StrongNameKeyPath { get; set; }

		[Option("public-key",
			HelpText = "The public key token which shall be put into every assembly's [InternalsVisibleTo] declaration in the given AssemblyInfo.cs file")]
		public string PublicKey { get; set; }
	}
}
