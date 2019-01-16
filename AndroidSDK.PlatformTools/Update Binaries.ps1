param (
  [string]$Version
)

Add-Type -AssemblyName System.IO.Compression.FileSystem

$ProgressPreference = 'SilentlyContinue'

if ([string]::IsNullOrWhitespace($Version)) {  
  $url = "https://dl.google.com/android/repository/platform-tools-latest-windows.zip"
} else {
  $url = "https://dl.google.com/android/repository/platform-tools_r$version-windows.zip"
}

$tempPath = Join-Path (Get-Location) "tmp"
if (Test-Path $tempPath)
{
  Remove-Item $tempPath -Recurse
}
New-Item -ItemType Directory $tempPath | Out-Null

Write-Host "Downloading $url"
Invoke-WebRequest $url -OutFile "$tempPath\platform-tools.zip"

Write-Host "Extracting $tempPath\platform-tools.zip"
[System.IO.Compression.ZipFile]::ExtractToDirectory("$tempPath\platform-tools.zip", $tempPath)

$dataPath = Join-Path (Get-Location) "Binaries"
if (Test-Path $dataPath)
{
	Remove-Item $dataPath -Recurse
}
New-Item -ItemType Directory $dataPath | Out-Null

Write-Host "Copying $tempPath\platform-tools to $dataPath"
Copy-Item "$tempPath\platform-tools\*" "$dataPath\" -Recurse -Include @("adb.exe", "fastboot.exe", "AdbWinApi.dll", "AdbWinUsbApi.dll")

(& "$dataPath\adb.exe" --version)[0]
(& "$dataPath\fastboot.exe" --version)[0]