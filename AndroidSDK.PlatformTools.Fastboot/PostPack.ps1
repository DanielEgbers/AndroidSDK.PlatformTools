$toolPath = [System.IO.Path]::Combine((Get-Location), 'bin\Debug');

$result = dotnet tool update AndroidSDK.PlatformTools.Fastboot --tool-path $toolPath 2>&1

if ($result -match "is not currently installed") {
    dotnet tool install AndroidSDK.PlatformTools.Fastboot --tool-path $toolPath
}