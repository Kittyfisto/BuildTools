using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CommandLine;

namespace GenerateRedirects
{
	class Program
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
			var appConfigFile = options.AppConfigFile;
			if (!File.Exists(appConfigFile))
				throw new ProgramException(string.Format("Unable to find '{0}'!", appConfigFile));

			var root = XElement.Load(appConfigFile);

			Console.WriteLine("Opened {0} to update binding redirects...", appConfigFile);

			var dependentAssembly = FindDependentAssembly(root, options.AssemblyName);
			var redirect = FindBindingRedirect(dependentAssembly);

			var newVersion = ReadNewVersion(options.NewVersionAssemblyInfo);

			Console.WriteLine("Changing newVersion of assembly {0} to {1}", options.AssemblyName, newVersion);

			SetNewVersion(redirect, newVersion);

			root.Save(appConfigFile);

			Console.WriteLine("Done!");
		}

		private static XElement FindDependentAssembly(XElement root, string assemblyName)
		{
			var @namespace = GetNamespace();
			var foo = root.Descendants().ToList();

			var identities = root.Descendants(@namespace + "assemblyIdentity");
			foreach (var identity in identities)
			{
				var name = identity.Attribute("name");
				if (name?.Value == assemblyName)
				{
					return identity.Parent;
				}
			}

			throw new ProgramException(string.Format("Unable to find an <assemblyIdentity> element for assembly with name '{0}'!", assemblyName));
		}

		private static XNamespace GetNamespace()
		{
			/*var assemblyBindings = root.Descendants("assemblyBinding");
			var binding = assemblyBindings.FirstOrDefault();
			if (binding == null)
				return null;

			return binding.Name.Namespace;*/
			return "urn:schemas-microsoft-com:asm.v1";
		}

		private static XElement FindBindingRedirect(XElement dependentAssembly)
		{
			var @namespace = GetNamespace();
			var redirects = dependentAssembly.Descendants(@namespace + "bindingRedirect");
			var redirect = redirects.FirstOrDefault();
			if (redirect == null)
				throw new ProgramException("TODO");

			return redirect;
		}

		private static void SetNewVersion(XElement redirect, Version newVersion)
		{
			redirect.SetAttributeValue("newVersion", newVersion);
		}

		private static Version ReadNewVersion(string fileName)
		{
			if (!File.Exists(fileName))
				throw new ProgramException(string.Format("Unable to find '{0}'!", fileName));

			using (var stream = File.OpenRead(fileName))
			using (var reader = new StreamReader(stream))
			{
				var regex = new Regex("\\[assembly:\\s+AssemblyVersion\\(\"([^\"]+)\"\\)\\]");

				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					if (line != null && !line.StartsWith("//"))
					{
						var match = regex.Match(line);
						if (match.Success)
						{
							var version = match.Groups[1].Value;
							return Version.Parse(version);
						}
					}
				}
			}

			throw new ProgramException(string.Format("Unable to find AssemblyVersion in '{0}'!", fileName));
		}
	}
}
