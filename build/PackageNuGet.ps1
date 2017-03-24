param($scriptRoot)

$ErrorActionPreference = "Stop"
$programFilesx86 = ${Env:ProgramFiles(x86)}
$msBuild = "$programFilesx86\MSBuild\14.0\bin\msbuild.exe"
$nuGet = "$scriptRoot..\tools\NuGet.exe"
$solution = "$scriptRoot\..\Leprechaun.sln"

& $nuGet restore $solution
& $msBuild $solution /p:Configuration=Release /t:Rebuild /m

& $nuGet pack "$scriptRoot\..\src\Leprechaun\Leprechaun.csproj" -Symbols -Prop Configuration=Release