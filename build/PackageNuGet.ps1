param($scriptRoot)

if ($scriptRoot -eq "")
{
    $scriptRoot = $PSScriptRoot
}

$ErrorActionPreference = "Stop"

& dotnet pack "$scriptRoot\..\src\Leprechaun\Leprechaun.csproj" -c "Release" --include-symbols -o "$scriptRoot\packages"
& dotnet pack "$scriptRoot\..\src\Leprechaun.CodeGen.Roslyn\Leprechaun.CodeGen.Roslyn.csproj" -c "Release" --include-symbols -o "$scriptRoot\packages"
& dotnet pack "$scriptRoot\..\src\Leprechaun.InputProviders.Rainbow\Leprechaun.InputProviders.Rainbow.csproj" -c "Release" --include-symbols -o "$scriptRoot\packages"
& dotnet pack "$scriptRoot\..\src\Leprechaun.InputProviders.Sitecore\Leprechaun.InputProviders.Sitecore.csproj" -c "Release" --include-symbols -o "$scriptRoot\packages"
& dotnet pack "$scriptRoot\..\src\Leprechaun.Console\Leprechaun.Console.csproj" -c "Release" --include-symbols -o "$scriptRoot\packages"
& dotnet pack "$scriptRoot\..\src\Leprechaun.Cli\Leprechaun.Cli.csproj" -c "Release" --include-symbols -o "$scriptRoot\packages"
