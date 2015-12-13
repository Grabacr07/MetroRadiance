param($installPath, $toolsPath, $package, $project)
# see also: http://docs.nuget.org/create/creating-and-publishing-a-package#Automatically_Running_PowerShell_Scripts_During_Package_Installation_and_Removal

$doc = New-Object System.Xml.XmlDocument
$doc.Load($project.FullName)
$namespace = 'http://schemas.microsoft.com/developer/msbuild/2003'

$propertyNode  = "CopyToOutputDirectory"
$propertyValue = "PreserveNewest"
$targets       = "ChromeHook32.dll",
				 "ChromeHook64.dll",
				 "ChromeHookCLR32.dll",
				 "ChromeHookCLR64.dll",
				 "ChromeHookService.dll",
				 "ChromeHook.InjectDll32.exe"

Foreach ($target in $targets)
{
	$targetNode = Select-Xml "//msb:Project/msb:ItemGroup/msb:Content[@Include='$target']" $doc -Namespace @{msb = $namespace}

	if ($targetNode -ne $null)
	{
		$property = $targetNode.Node.SelectSingleNode($propertyNode)
		if ($property -eq $null)
		{
			$property = $doc.CreateElement($propertyNode, $namespace)
			$property.AppendChild($doc.CreateTextNode($propertyValue))
			$targetNode.Node.AppendChild($property)

			$doc.Save($project.FullName)
		}
	}
}
