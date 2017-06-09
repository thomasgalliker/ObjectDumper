<#
.SYNOPSIS
	Update AssemblyInfo.cs files.
.DESCRIPTION
	Updates following properties in AssemblyInfo.cs
	- AssemblyVersion
	- AssemblyFileVersion
	- AssemblyCopyright
.EXAMPLE
  Update-AssemblyInfoFiles "[Path to .nuspec file]" "[Path to AssemblyInfo.cs]"
  Update-AssemblyInfoFiles "Package.nuspec" "..\*\Properties\AssemblyInfo.cs"

.NOTES
	Author: Andreas Gullberg Larsen
	Date:   May 2, 2014
	Based on original work by Luis Rocha from: http://www.luisrocha.net/2009/11/setting-assembly-version-with-windows.html
#>
[CmdletBinding()]

$root=Resolve-Path "$PSScriptRoot"

#-------------------------------------------------------------------------------
# Displays how to use this script.
#-------------------------------------------------------------------------------
function Help {
  "Sets the AssemblyVersion and AssemblyFileVersion of AssemblyInfo.cs files`n"
  ".\UpdateAssemblyInfo.ps1 [VersionNumber]`n"
  "   [VersionNumber]     The version number to set, for example: 1.1.9301.0"
}

#-------------------------------------------------------------------------------
# Returns the <version></version> element value of the .nuspec file.
#-------------------------------------------------------------------------------
function Get-VersionFromNuspec ([string] $nuspecFilePath) {


  [ xml ]$nuspecXml = Get-Content -Path $nuspecFilePath
  $version = $nuspecXml.package.metadata.version

  Write-Host "Read <version> of file $($nuspecFilePath): $version"
  return $version;
}

#-------------------------------------------------------------------------------
# Description: Sets the AssemblyVersion and AssemblyFileVersion of
#              AssemblyInfo.cs files.
#
# Author: Andreas Larsen
# Version: 1.0
#-------------------------------------------------------------------------------
function Update-AssemblyInfoFiles ([string] $nuspecFilePath, [string] $assemblyInfoFilePath) {

	$version = Get-VersionFromNuspec "$root\$nuspecFilePath"
	$versionSplit = $version.Split('-')

	$assemblyVersionPattern = 'AssemblyVersion\("[^"]*"\)'
	$assemblyVersion = 'AssemblyVersion("' + $versionSplit[0] + '")';
	Write-Host "AssemblyVersion = $assemblyVersion"

	$assemblyFileVersionPattern = 'AssemblyFileVersion\("[^"]*"\)'
	$assemblyFileVersion = 'AssemblyFileVersion("' + $version + '")';
	Write-Host "AssemblyFileVersion = $assemblyFileVersion"

	$copyright = "Copyright © $(Get-Date –f yyyy)"
	$assemblyAssemblyCopyrightPattern = 'AssemblyCopyright\("[^"]*"\)'
	$assemblyAssemblyCopyright = 'AssemblyCopyright("' + $copyright + '")';
	Write-Host "AssemblyCopyright = $assemblyAssemblyCopyright"

	Get-ChildItem "$root\$assemblyInfoFilePath" | ForEach-Object {
		$filename = $_.Directory.ToString() + '\' + $_.Name
		$filename + ' -> ' + $version

		(Get-Content $filename -Encoding UTF8) | ForEach-Object {
			% {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
			% {$_ -replace $assemblyFileVersionPattern, $assemblyFileVersion } |
			% {$_ -replace $assemblyAssemblyCopyrightPattern, $assemblyAssemblyCopyright }
		} | Set-Content $filename -Encoding UTF8
	}
}

try {
  "Updating assembly info to version: $setVersion"
  ""
  Update-AssemblyInfoFiles "Package.nuspec" "..\*\Properties\AssemblyInfo.cs"
}
catch {
  $myError = $_.Exception.ToString()
	Write-Error "Failed to update AssemblyInfo files: `n$myError' ]"
	exit 1
}