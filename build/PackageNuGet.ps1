param($scriptRoot)

$ErrorActionPreference = "Stop"

function Resolve-MsBuild {
	$msb2017 = Resolve-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\*\*\MSBuild\*\bin\msbuild.exe" -ErrorAction SilentlyContinue
	if($msb2017) {
		Write-Host "Found MSBuild 2017 (or later)."
		Write-Host $msb2017
		return $msb2017
	}

	$msBuild2015 = "${env:ProgramFiles(x86)}\MSBuild\14.0\bin\msbuild.exe"

	if(-not (Test-Path $msBuild2015)) {
		throw 'Could not find MSBuild 2015 or later.'
	}

	Write-Host "Found MSBuild 2015."
	Write-Host $msBuild2015

	return $msBuild2015
}

$msBuild = Resolve-MsBuild
$nuGet = "$scriptRoot..\tools\NuGet.exe"
$solution = "$scriptRoot\..\Leprechaun.sln"

& $nuGet restore $solution
& $msBuild $solution /p:Configuration=Release /t:Rebuild /m

& $nuGet pack "$scriptRoot\..\src\Leprechaun\Leprechaun.csproj" -Symbols -Prop Configuration=Release
& $nuGet pack "$scriptRoot\..\src\Leprechaun.Console\Leprechaun.Console.csproj" -Symbols -Prop Configuration=Release
& $nuGet pack "$scriptRoot\..\src\Leprechaun.CodeGen.Roslyn\Leprechaun.CodeGen.Roslyn.csproj" -Symbols -Prop Configuration=Release

$assembly = Get-Item "$scriptRoot\..\src\Leprechaun\bin\Release\Leprechaun.dll" | Select-Object -ExpandProperty VersionInfo
$targetAssemblyVersion = $assembly.ProductVersion
$releaseZipName = "Leprechaun-$targetAssemblyVersion.zip"

& $nuGet pack "$scriptRoot\..\src\Leprechaun.Console\Leprechaun.Console.Runner.nuspec" -version $targetAssemblyVersion

if((Test-Path $releaseZipName)) {
	Remove-Item $releaseZipName -Force
}

$releaseRoot = "$PSScriptRoot\..\src\Leprechaun.Console\bin\Release"

Remove-Item $releaseRoot\*.xml
Remove-Item $releaseRoot\*.pdb

& "$PSScriptRoot\..\Tools\7za.exe" a $releaseZipName "$releaseRoot\*" -mx9