param($installPath, $toolsPath, $package, $project)

# Load project XML.
$doc = New-Object System.Xml.XmlDocument
$doc.Load($project.FullName)
$namespace = 'http://schemas.microsoft.com/developer/msbuild/2003'

# Find the node containing the file. The tag "Content" may be replace by "None" depending of the case, check .csproj file.
$targets = "ChromeHook32.dll", "ChromeHook64.dll", "ChromeHookCLR32.dll", "ChromeHookCLR64.dll", "ChromeHook.InjectDll32.exe"

Foreach ($target in $targets)
{
    $xmlNode = Select-Xml "//msb:Project/msb:ItemGroup/msb:Content[@Include='$target']" $doc -Namespace @{msb = $namespace}

    if ($xmlNode -ne $null)
    {
        $nodeName = "CopyToOutputDirectory"

        # Check if the property already exists, just in case.
        $property = $xmlNode.Node.SelectSingleNode($nodeName)
        if ($property -eq $null)
        {
            $property = $doc.CreateElement($nodeName, $namespace)
            $property.AppendChild($doc.CreateTextNode("PreserveNewest"))
            $xmlNode.Node.AppendChild($property)

            # Save changes.
            $doc.Save($project.FullName)
        }
    }
}
