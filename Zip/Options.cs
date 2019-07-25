using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Zip
{
	class Options
	{
		[Option('o', "output", Required = true,
			HelpText = "The output path of the resulting zip file")]
		public string OutputFileName { get; set; }

		[Option('a', "append",
			Required = false,
			HelpText = "When specified, will append to an existing zip-file")]
		public bool Append { get; set; }

		[Option('i', "input", Required = true,
			HelpText = "The list of input files and/or folders to zip. When a path points to a folder, all files in that folder (recursively) will be added to the zip file.")]
		public IEnumerable<string> Input { get; set; }

		[Option('f', "folder",
			Required = false,
			HelpText = "The relative path within the zip archive the input file should be added to")]
		public string Folder { get; set; }
	}
}
