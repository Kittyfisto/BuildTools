using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Application to view log files - live and offline")]
[assembly: AssemblyDescription("Application to view log files - live and offline")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Tailviewer")]
[assembly: AssemblyCopyright("Copyright © Simon Mießler 2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
	//(used if a resource is not found in the page, 
	// or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
	//(used if a resource is not found in the page, 
	// app, or any theme specific resource dictionaries)
	)]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("0.9.0.0")]
[assembly: AssemblyFileVersion("0.9.0.0")]

[assembly: InternalsVisibleTo("Tailviewer.Test,PublicKey=0024000004800000940000000602000000240000525341310004000001000100210c86c5918a079261aef7cca3d24b1f51108fc0a416f4c4978140cf9372141637e3251169bc5bc862a9829b0f2339d5b3fb89f6983a52587c25a312344a49fd2427b0c46893f1c7a038f7ef3d65428593fe7c3d254ae637c4ef119414f2a38b1a1c0e55a9d9832c687f5ff5715bfd292fedf0b59a70a00cb9eb4d13b9b89eb3")]
[assembly: InternalsVisibleTo("Tailviewer.AcceptanceTests,PublicKey=0024000004800000940000000602000000240000525341310004000001000100210c86c5918a079261aef7cca3d24b1f51108fc0a416f4c4978140cf9372141637e3251169bc5bc862a9829b0f2339d5b3fb89f6983a52587c25a312344a49fd2427b0c46893f1c7a038f7ef3d65428593fe7c3d254ae637c4ef119414f2a38b1a1c0e55a9d9832c687f5ff5715bfd292fedf0b59a70a00cb9eb4d13b9b89eb3")]
