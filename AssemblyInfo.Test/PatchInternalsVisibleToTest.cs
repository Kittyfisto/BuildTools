using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using AssemblyInfo.Applications;
using FluentAssertions;
using NUnit.Framework;

namespace AssemblyInfo.Test
{
	[TestFixture]
	public sealed class PatchInternalsVisibleToTest
	{
		[Test]
		public void TestExtractPublicKeyTokenFromKey1()
		{
			ExtractPublicKeyTokenFrom(@"Key1.snk")
				.Should().Be("0024000004800000940000000602000000240000525341310004000001000100210c86c5918a07" +
				             "9261aef7cca3d24b1f51108fc0a416f4c4978140cf9372141637e3251169bc5bc862a9829b0f23" +
				             "39d5b3fb89f6983a52587c25a312344a49fd2427b0c46893f1c7a038f7ef3d65428593fe7c3d25" +
				             "4ae637c4ef119414f2a38b1a1c0e55a9d9832c687f5ff5715bfd292fedf0b59a70a00cb9eb4d13" +
				             "b9b89eb3");
		}

		[Test]
		public void TestExtractPublicKeyTokenFromKey2()
		{
			ExtractPublicKeyTokenFrom(@"Key2.snk")
				.Should().Be("00240000048000009400000006020000002400005253413100040000010001005919731211e941" +
				             "4f6e943e16c29e5d6b22cf1aa818ffd23983367c9c512eaabd41afbd7bfadcd022f492f33b379b" +
				             "1276c5059543a4cd5e50992edad7ab84801676e2f15d92b57e45ba27775b8d7df3a285650016b5" +
				             "91b4e51574e0f1932e94d62f2da41066ceb9786e9e4f8e2e8e6f44dac1ea358755a8f61ab1d4b9" +
				             "eea769d1");
		}

		[Test]
		[Description("Verifies that removing the PublicKey part is possible")]
		public void TestChangePublicKeyToken1()
		{
			var content = "[assembly: InternalsVisibleTo(\"Tailviewer.Test,PublicKey=00240000048000009400000006020000002400005253413100040000010001006d873a2f2f5d54\" +\r\n\t\"280b91e8a2b6997fbe287f0631db99675716fbd9ded5ae79276ec77851fbe7be4e975bae1bc1d6\" +\r\n\t\"dcc76d4e00ab7dbba236f2c2e842310cc6b842ae0785afd969bf0b2fc79b5a902cf0e7278dbf33\" +\r\n\t\"00e9158b2693d209dfda4670b3ef8f660b7bc7be6028bcef1665f4aaaa8cc6851d36968210ea77\" +\r\n\t\"1db7ebdb\")]";
			PatchInternalsVisibleTo.ChangePublicKeyToken(content, "Tailviewer.Test", null)
			                       .Should().Be("[assembly: InternalsVisibleTo(\"Tailviewer.Test\")]");
		}

		[Test]
		[Description("Verifies that adding the PublicKey part is possible")]
		public void TestChangePublicKeyToken2()
		{
			var content = "[assembly: InternalsVisibleTo(\"Tailviewer\")]";
			PatchInternalsVisibleTo.ChangePublicKeyToken(content, "Tailviewer", "00240000048000009400000006020000002400005253413100040000010001005919731211e9414f6e943e16c29e5d6b22cf1aa818ffd23983367c9c512eaabd41afbd7bfadcd022f492f33b379b1276c5059543a4cd5e50992edad7ab84801676e2f15d92b57e45ba27775b8d7df3a285650016b591b4e51574e0f1932e94d62f2da41066ceb9786e9e4f8e2e8e6f44dac1ea358755a8f61ab1d4b9eea769d1")
			                       .Should().Be("[assembly: InternalsVisibleTo(\"Tailviewer,PublicKey=00240000048000009400000006020000002400005253413100040000010001005919731211e9414f6e943e16c29e5d6b22cf1aa818ffd23983367c9c512eaabd41afbd7bfadcd022f492f33b379b1276c5059543a4cd5e50992edad7ab84801676e2f15d92b57e45ba27775b8d7df3a285650016b591b4e51574e0f1932e94d62f2da41066ceb9786e9e4f8e2e8e6f44dac1ea358755a8f61ab1d4b9eea769d1\")]");
		}

		[Test]
		[Description("Verifies that replacing the PublicKey part is possible")]
		public void TestChangePublicKeyToken3()
		{
			var content = "[assembly: InternalsVisibleTo(\"Foobar,PublicKey=00240000048000009400000006020000002400005253413100040000010001005919731211e9414f6e943e16c29e5d6b22cf1aa818ffd23983367c9c512eaabd41afbd7bfadcd022f492f33b379b1276c5059543a4cd5e50992edad7ab84801676e2f15d92b57e45ba27775b8d7df3a285650016b591b4e51574e0f1932e94d62f2da41066ceb9786e9e4f8e2e8e6f44dac1ea358755a8f61ab1d4b9eea769d1\")]";
			PatchInternalsVisibleTo.ChangePublicKeyToken(content, "Foobar", "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")
			                       .Should().Be("[assembly: InternalsVisibleTo(\"Foobar,PublicKey=00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\")]");
		}

		[Test]
		[Description("Verifies that only desired assembly declarations are changed")]
		public void TestChangePublicKeyToken4()
		{
			var content = "[assembly: InternalsVisibleTo(\"SomeAssembly,PublicKey=00240000048000009400000006020000002400005253413100040000010001005919731211e9414f6e943e16c29e5d6b22cf1aa818ffd23983367c9c512eaabd41afbd7bfadcd022f492f33b379b1276c5059543a4cd5e50992edad7ab84801676e2f15d92b57e45ba27775b8d7df3a285650016b591b4e51574e0f1932e94d62f2da41066ceb9786e9e4f8e2e8e6f44dac1ea358755a8f61ab1d4b9eea769d1\")]";
			PatchInternalsVisibleTo.ChangePublicKeyToken(content, "AnotherAssembly", "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")
			                       .Should().Be("[assembly: InternalsVisibleTo(\"SomeAssembly,PublicKey=00240000048000009400000006020000002400005253413100040000010001005919731211e9414f6e943e16c29e5d6b22cf1aa818ffd23983367c9c512eaabd41afbd7bfadcd022f492f33b379b1276c5059543a4cd5e50992edad7ab84801676e2f15d92b57e45ba27775b8d7df3a285650016b591b4e51574e0f1932e94d62f2da41066ceb9786e9e4f8e2e8e6f44dac1ea358755a8f61ab1d4b9eea769d1\")]");
		}

		[Test]
		[Description("Verifies that only desired assembly declarations are changed")]
		public void TestChangePublicKeyToken5()
		{
			var content = "using System.Reflection;\r\nusing System.Runtime.CompilerServices;\r\nusing System.Runtime.InteropServices;\r\nusing System.Windows;\r\n\r\n// General Information about an assembly is controlled through the following \r\n// set of attributes. Change these attribute values to modify the information\r\n// associated with an assembly.\r\n\r\n[assembly: AssemblyTitle(\"Application to view log files - live and offline\")]\r\n[assembly: AssemblyDescription(\"Application to view log files - live and offline\")]\r\n[assembly: AssemblyConfiguration(\"\")]\r\n[assembly: AssemblyCompany(\"\")]\r\n[assembly: AssemblyProduct(\"Tailviewer\")]\r\n[assembly: AssemblyCopyright(\"Copyright © Simon Mießler 2019\")]\r\n[assembly: AssemblyTrademark(\"\")]\r\n[assembly: AssemblyCulture(\"\")]\r\n\r\n// Setting ComVisible to false makes the types in this assembly not visible \r\n// to COM components.  If you need to access a type in this assembly from \r\n// COM, set the ComVisible attribute to true on that type.\r\n\r\n[assembly: ComVisible(false)]\r\n\r\n//In order to begin building localizable applications, set \r\n//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file\r\n//inside a <PropertyGroup>.  For example, if you are using US english\r\n//in your source files, set the <UICulture> to en-US.  Then uncomment\r\n//the NeutralResourceLanguage attribute below.  Update the \"en-US\" in\r\n//the line below to match the UICulture setting in the project file.\r\n\r\n//[assembly: NeutralResourcesLanguage(\"en-US\", UltimateResourceFallbackLocation.Satellite)]\r\n\r\n\r\n[assembly: ThemeInfo(\r\n\tResourceDictionaryLocation.None, //where theme specific resource dictionaries are located\r\n\t//(used if a resource is not found in the page, \r\n\t// or application resource dictionaries)\r\n\tResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located\r\n\t//(used if a resource is not found in the page, \r\n\t// app, or any theme specific resource dictionaries)\r\n\t)]\r\n\r\n\r\n// Version information for an assembly consists of the following four values:\r\n//\r\n//      Major Version\r\n//      Minor Version \r\n//      Build Number\r\n//      Revision\r\n//\r\n// You can specify all the values or you can default the Build and Revision Numbers \r\n// by using the '*' as shown below:\r\n// [assembly: AssemblyVersion(\"1.0.*\")]\r\n\r\n[assembly: AssemblyVersion(\"0.9.0.0\")]\r\n[assembly: AssemblyFileVersion(\"0.9.0.0\")]\r\n\r\n[assembly: InternalsVisibleTo(\"Tailviewer.Test,PublicKey=00240000048000009400000006020000002400005253413100040000010001006d873a2f2f5d54\" +\r\n\t\"280b91e8a2b6997fbe287f0631db99675716fbd9ded5ae79276ec77851fbe7be4e975bae1bc1d6\" +\r\n\t\"dcc76d4e00ab7dbba236f2c2e842310cc6b842ae0785afd969bf0b2fc79b5a902cf0e7278dbf33\" +\r\n\t\"00e9158b2693d209dfda4670b3ef8f660b7bc7be6028bcef1665f4aaaa8cc6851d36968210ea77\" +\r\n\t\"1db7ebdb\")]\r\n[assembly: InternalsVisibleTo(\"Tailviewer.AcceptanceTests,PublicKey=00240000048000009400000006020000002400005253413100040000010001006d873a2f2f5d54\" +\r\n\t\"280b91e8a2b6997fbe287f0631db99675716fbd9ded5ae79276ec77851fbe7be4e975bae1bc1d6\" +\r\n\t\"dcc76d4e00ab7dbba236f2c2e842310cc6b842ae0785afd969bf0b2fc79b5a902cf0e7278dbf33\" +\r\n\t\"00e9158b2693d209dfda4670b3ef8f660b7bc7be6028bcef1665f4aaaa8cc6851d36968210ea77\" +\r\n\t\"1db7ebdb\")]\r\n";
			PatchInternalsVisibleTo.ChangePublicKeyToken(content, "Tailviewer.Test", "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")
			                       .Should().Be("using System.Reflection;\r\nusing System.Runtime.CompilerServices;\r\nusing System.Runtime.InteropServices;\r\nusing System.Windows;\r\n\r\n// General Information about an assembly is controlled through the following \r\n// set of attributes. Change these attribute values to modify the information\r\n// associated with an assembly.\r\n\r\n[assembly: AssemblyTitle(\"Application to view log files - live and offline\")]\r\n[assembly: AssemblyDescription(\"Application to view log files - live and offline\")]\r\n[assembly: AssemblyConfiguration(\"\")]\r\n[assembly: AssemblyCompany(\"\")]\r\n[assembly: AssemblyProduct(\"Tailviewer\")]\r\n[assembly: AssemblyCopyright(\"Copyright © Simon Mießler 2019\")]\r\n[assembly: AssemblyTrademark(\"\")]\r\n[assembly: AssemblyCulture(\"\")]\r\n\r\n// Setting ComVisible to false makes the types in this assembly not visible \r\n// to COM components.  If you need to access a type in this assembly from \r\n// COM, set the ComVisible attribute to true on that type.\r\n\r\n[assembly: ComVisible(false)]\r\n\r\n//In order to begin building localizable applications, set \r\n//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file\r\n//inside a <PropertyGroup>.  For example, if you are using US english\r\n//in your source files, set the <UICulture> to en-US.  Then uncomment\r\n//the NeutralResourceLanguage attribute below.  Update the \"en-US\" in\r\n//the line below to match the UICulture setting in the project file.\r\n\r\n//[assembly: NeutralResourcesLanguage(\"en-US\", UltimateResourceFallbackLocation.Satellite)]\r\n\r\n\r\n[assembly: ThemeInfo(\r\n\tResourceDictionaryLocation.None, //where theme specific resource dictionaries are located\r\n\t//(used if a resource is not found in the page, \r\n\t// or application resource dictionaries)\r\n\tResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located\r\n\t//(used if a resource is not found in the page, \r\n\t// app, or any theme specific resource dictionaries)\r\n\t)]\r\n\r\n\r\n// Version information for an assembly consists of the following four values:\r\n//\r\n//      Major Version\r\n//      Minor Version \r\n//      Build Number\r\n//      Revision\r\n//\r\n// You can specify all the values or you can default the Build and Revision Numbers \r\n// by using the '*' as shown below:\r\n// [assembly: AssemblyVersion(\"1.0.*\")]\r\n\r\n[assembly: AssemblyVersion(\"0.9.0.0\")]\r\n[assembly: AssemblyFileVersion(\"0.9.0.0\")]\r\n\r\n[assembly: InternalsVisibleTo(\"Tailviewer.Test,PublicKey=00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\")]\r\n[assembly: InternalsVisibleTo(\"Tailviewer.AcceptanceTests,PublicKey=00240000048000009400000006020000002400005253413100040000010001006d873a2f2f5d54\" +\r\n\t\"280b91e8a2b6997fbe287f0631db99675716fbd9ded5ae79276ec77851fbe7be4e975bae1bc1d6\" +\r\n\t\"dcc76d4e00ab7dbba236f2c2e842310cc6b842ae0785afd969bf0b2fc79b5a902cf0e7278dbf33\" +\r\n\t\"00e9158b2693d209dfda4670b3ef8f660b7bc7be6028bcef1665f4aaaa8cc6851d36968210ea77\" +\r\n\t\"1db7ebdb\")]\r\n");
		}

		[Test]
		[Description("Verifies that the public key can be extracted from an SNK file and be replaced in a real-world AssemblyInfo.cs file")]
		public void TestRun1()
		{
			TestRun(sourceFile: "AssemblyInfo1.cs",
			        assemblies: new[]
			        {
				        "Tailviewer.Test",
				        "Tailviewer.AcceptanceTests"
			        },
			        keyFile: "Key1.snk",
			        expectedFile: "ExpectedAssemblyInfo1.cs");
		}

		[Test]
		[Description("Verifies that the public key can be extracted from an SNK file and be replaced in a real-world AssemblyInfo.cs file")]
		public void TestRun2()
		{
			TestRun(sourceFile: "AssemblyInfo2.cs",
			        assemblies: new[]
			        {
				        "Tailviewer",
				        "archiver",
				        "Tailviewer.Test",
				        "Tailviewer.AcceptanceTests"
			        },
			        keyFile: "Key2.snk",
			        expectedFile: "ExpectedAssemblyInfo2.cs");
		}

		[Test]
		public void TestRunSourceAssemblyInfoNotFound1()
		{
			new Action(() => PatchInternalsVisibleTo.Run(new PatchInternalsVisibleToOptions
				{
					AssemblyInfoPath = @"D:\whatever.cs",
					Assemblies = new List<string>{"Foo"}
				}))
				.Should()
				.Throw<ProgramException>()
				.WithMessage(@"Could not find 'D:\whatever.cs'!");
		}

		[Test]
		public void TestRunSourceAssemblyInfoNotFound2()
		{
			new Action(() => PatchInternalsVisibleTo.Run(new PatchInternalsVisibleToOptions
				{
					AssemblyInfoPath = @"X:\somefile.cs",
					Assemblies = new List<string>{"Foo"}
				}))
				.Should()
				.Throw<ProgramException>()
				.WithMessage(@"Could not find 'X:\somefile.cs'!");
		}

		private void TestRun(string sourceFile, string[] assemblies, string keyFile, string expectedFile)
		{
			var assemblyInfoPath = CopyToTemp(sourceFile);

			var snkPath = Path.Combine(TestDataFolder, keyFile);
			PatchInternalsVisibleTo.Run(new PatchInternalsVisibleToOptions
			{
				AssemblyInfoPath = assemblyInfoPath,
				Assemblies = new List<string>(assemblies),
				StrongNameKeyPath = snkPath
			});

			var actualAssemblyInfo = File.ReadAllText(assemblyInfoPath);
			var expectedAssemblyInfo = File.ReadAllText(Path.Combine(TestDataFolder, expectedFile));
			actualAssemblyInfo.Should().Be(expectedAssemblyInfo);
		}

		private string CopyToTemp(string assemblyInfo)
		{
			var fullKeyFilePath = Path.Combine(TestDataFolder, assemblyInfo);
			var tempFileName = Path.Combine(Path.GetTempPath(), assemblyInfo);

			if (File.Exists(tempFileName))
				File.Delete(tempFileName);

			File.Copy(fullKeyFilePath, tempFileName);
			return tempFileName;
		}

		[Pure]
		private string ExtractPublicKeyTokenFrom(string relativeKeyFilePath)
		{
			var fullKeyFilePath = Path.Combine(TestDataFolder, relativeKeyFilePath);
			return PatchInternalsVisibleTo.GetPublicKeyFrom(fullKeyFilePath);
		}

		private static string TestDataFolder
		{
			get
			{
				string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				var testDataFolder = Path.Combine(assemblyPath, "..", "..", "AssemblyInfo.Test", "TestData");
				return testDataFolder;
			}
		}
	}
}