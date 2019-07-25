using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Zip
{
	public sealed class Program
	{
		static int Main(string[] args)
		{
			try
			{
				Parser.Default.ParseArguments<Options>(args)
					.WithParsed<Options>(Run);

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
				return -1;
			}
		}

		private static void Run(Options options)
		{
			using (var zipFile = CreateZipFile(options.OutputFileName, options.Append))
			{
				foreach (var path in options.Input)
				{
					Add(zipFile, path, options.Folder);
				}
			}
		}

		private static ZipArchive CreateZipFile(string fileName, bool append)
		{
			if (append)
			{
				var stream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
				try
				{
					return new ZipArchive(stream, ZipArchiveMode.Update);
				}
				catch (Exception)
				{
					stream.Dispose();
					throw;
				}
			}

			if (File.Exists(fileName))
				throw new ProgramException($"The file '{fileName}' already exists. You have to specify --append to append to an existing zip archive.");

			var fileStream = File.Open(fileName, FileMode.CreateNew, FileAccess.ReadWrite);
			try
			{
				return new ZipArchive(fileStream, ZipArchiveMode.Create);
			}
			catch (Exception)
			{
				fileStream.Dispose();
				throw;
			}
		}

		private static void Add(ZipArchive zipFile, string path, string folder)
		{
			if (File.Exists(path))
			{
				var fileName = Path.GetFileName(path);
				if (folder != null)
					fileName = Path.Combine(folder, fileName);
				AddFile(zipFile, path, fileName);
			}
			else if (Directory.Exists(path))
			{
				AddDirectory(zipFile, path, folder);
			}
			else
			{
				throw new ProgramException($"{path} is neither a file nor a valid directory!");
			}
		}

		private static void AddDirectory(ZipArchive zipFile, string path, string folder)
		{
			Console.WriteLine("Adding directory '{0}'...", path);

			var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
			foreach (var filePath in files)
			{
				var entryName = GetRelativePath(filePath, path);
				if (folder != null)
					entryName = Path.Combine(folder, entryName);
				AddFile(zipFile, filePath, entryName);
			}
		}

		[Pure]
		private static string GetRelativePath(string filePath, string folder)
		{
			Uri pathUri = new Uri(filePath);
			// Folders must end in a slash
			if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				folder += Path.DirectorySeparatorChar;
			}
			Uri folderUri = new Uri(folder);
			return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}

		private static void AddFile(ZipArchive zipFile, string fullFilePath, string entryName)
		{
			Console.WriteLine("Adding '{0}' as '{1}'...", fullFilePath, entryName);

			var existingEntry = zipFile.GetEntry(entryName);
			if (existingEntry != null)
				throw new ProgramException($"The zip archive already contains a file named '{entryName}'!");

			var entry = zipFile.CreateEntry(entryName, CompressionLevel.Optimal);

			using (var source = File.OpenRead(fullFilePath))
			using (var dest = entry.Open())
			{
				source.CopyTo(dest);
			}
		}
	}
}
