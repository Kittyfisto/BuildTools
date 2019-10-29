using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AssemblyInfo.Applications
{
	public sealed class PatchInternalsVisibleTo
	{
		public static void Run(PatchInternalsVisibleToOptions options)
		{
			string publicKey;
			if (!string.IsNullOrEmpty(options.StrongNameKeyPath))
				publicKey = GetPublicKeyFrom(options.StrongNameKeyPath);
			else
				publicKey = options.PublicKey;

			if (!string.IsNullOrEmpty(publicKey))
			{
				Console.WriteLine("Changing PublicKeyToken of references to '{0}' to '{1}'",
				                  string.Join(", ", options.Assemblies),
				                  publicKey);
			}
			else
			{
				Console.WriteLine("Removing PublicKeyToken of references to '{0}'",
				                  string.Join(", ", options.Assemblies));
			}

			ChangePublicKeyToken(options.AssemblyInfoPath, options.Assemblies, publicKey);

			Console.WriteLine("Changes written to: {0}", options.AssemblyInfoPath);
		}

		/// <summary>
		///     Extracts the public key token from the given SNK file and returns its hexadecimal representation.
		/// </summary>
		/// <param name="optionsStrongNameKeyPath"></param>
		/// <returns></returns>
		[Pure]
		public static string GetPublicKeyFrom(string optionsStrongNameKeyPath)
		{
			var publicKeyFilePath = CreatePublicKeyFileFrom(optionsStrongNameKeyPath);
			var publicKey = GetPublicKeyFromPublicKeyFile(publicKeyFilePath);
			return publicKey;
		}

		private static string CreatePublicKeyFileFrom(string optionsStrongNameKeyPath)
		{
			var publicKeyFilePath = Path.GetTempFileName();
			Execute("sn.exe", $"-p \"{optionsStrongNameKeyPath}\" \"{publicKeyFilePath}\"");
			return publicKeyFilePath;
		}

		[Pure]
		private static string GetPublicKeyFromPublicKeyFile(string publicKeyFilePath)
		{
			var output = Execute("sn.exe", $"-tp \"{publicKeyFilePath}\"");
			var regex = new Regex(@"Public key[^\n]+\n(([0-9a-fA-F]+(\r\n?|\n))+)");
			var match = regex.Match(output);
			if (!match.Success)
				throw new ProgramException($"Unable to extract public key from the following output:\r\n{output}");

			var publicKey = new StringBuilder(match.Groups[1].Value);
			publicKey.Replace("\r", "");
			publicKey.Replace("\n", "");
			return publicKey.ToString();
		}

		/// <summary>
		/// Executes the given command and returns its std out.
		/// </summary>
		/// <param name="executable"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		private static string Execute(string executable, string arguments)
		{
			Process p;
			try
			{
				var absoluteExecutablePath = Path.Combine(ResolveVSToolsPath(), executable);
				var startInfo = new ProcessStartInfo(absoluteExecutablePath, arguments)
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
				};
				p = Process.Start(startInfo);
			}
			catch (Win32Exception e)
			{
				if (e.ErrorCode == -2147467259)
					throw new ProgramException($"Unable to execute '{executable}' because the executable file cannot be found");

				throw;
			}

			var output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			var exitCode = p.ExitCode;
			if (exitCode != 0)
				throw new ProgramException($"{executable} returned {exitCode}");

			return output;
		}

		private static void ChangePublicKeyToken(string assemblyInfoPath,
		                                         IEnumerable<string> assemblies,
		                                         string publicKey)
		{
			var assemblyInfo = ReadFile(assemblyInfoPath);

			foreach (var assembly in assemblies)
			{
				assemblyInfo = ChangePublicKeyToken(assemblyInfo, assembly, publicKey);
			}

			File.WriteAllText(assemblyInfoPath, assemblyInfo.ToString(), new UTF8Encoding(true));
		}

		[Pure]
		private static string ReadFile(string assemblyInfoPath)
		{
			try
			{
				return File.ReadAllText(assemblyInfoPath);
			}
			catch (FileNotFoundException)
			{
				throw new ProgramException($"Could not find '{assemblyInfoPath}'!");
			}
			catch (DirectoryNotFoundException)
			{
				throw new ProgramException($"Could not find '{assemblyInfoPath}'!");
			}
		}

		public static string ChangePublicKeyToken(string assemblyInfo, string assembly, string publicKey)
		{
			var regex = new Regex(@"\[assembly\s*:\s*InternalsVisibleTo\s*\(""([^,""]+)(\s*[^\)]*)\s*""\s*\)\s*\]", RegexOptions.Multiline);

			int i = 0;
			while (i < assemblyInfo.Length)
			{
				var match = regex.Match(assemblyInfo, i);
				if (match.Success)
				{
					if (match.Groups[1].Value == assembly)
					{
						var builder = new StringBuilder(assemblyInfo);
						builder.Remove(match.Index, match.Length);

						var innerPart = publicKey != null
							? $"{assembly},PublicKey={publicKey}"
							: $"{assembly}";
						var newPart = $"[assembly: InternalsVisibleTo(\"{innerPart}\")]";

						builder.Insert(match.Index, newPart);
						assemblyInfo = builder.ToString();

						i = match.Index + newPart.Length;
					}
					else
					{
						i = match.Index + match.Length;
					}
				}
				else
				{
					break;
				}
			}

			return assemblyInfo;
		}

		private static string ResolveVSToolsPath()
		{
			var paths = new[]
			{
				@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\x64\"
			};

			foreach (var path in paths)
			{
				if (Directory.Exists(path))
				{
					return path;
				}
			}

			var candidates = string.Join("\r\n\t", paths);
			throw new ProgramException($"Unable to find a directory where VS tools are deployed! Tried the following paths:\r\n\t{candidates}");
		}
	}
}