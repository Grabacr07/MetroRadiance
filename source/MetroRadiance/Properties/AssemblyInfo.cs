using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("MetroRadiance")]
[assembly: AssemblyCompany("grabacr.net")]
[assembly: AssemblyProduct("MetroRadiance")]
[assembly: AssemblyDescription("Modern WPF Themes")]
[assembly: AssemblyCopyright("Copyright © 2014 Manato KAMEYA")]

[assembly: ThemeInfo(
	ResourceDictionaryLocation.None,
	ResourceDictionaryLocation.SourceAssembly)]

[assembly: XmlnsDefinition("http://schemes.grabacr.net/winfx/2014/behaviors", "MetroRadiance.UI.Behaviors")]
[assembly: XmlnsDefinition("http://schemes.grabacr.net/winfx/2014/controls", "MetroRadiance.UI.Controls")]
[assembly: XmlnsDefinition("http://schemes.grabacr.net/winfx/2014/converters", "MetroRadiance.UI.Converters")]

[assembly: AssemblyVersion("2.0.4")]
[assembly: AssemblyInformationalVersion("2.0.4-alpha")]
[assembly: ComVisible(false)]
