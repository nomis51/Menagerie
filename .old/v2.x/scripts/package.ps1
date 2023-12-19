$csprojFilePaths = "../src/Menagerie/Menagerie.csproj"
[xml]$appXmlDoc = Get-Content $csprojFilePaths
$version = $appXmlDoc.Project.PropertyGroup.AssemblyVersion

echo Building projects...
dotnet build ../src/Menagerie.Shared/ -c Release
dotnet build ../src/Menagerie.Data/ -c Release
dotnet build ../src/Menagerie.Application/ -c Release
dotnet build ../src/Menagerie/ -c Release

echo Copying build files...
if (Test-Path -Path ../build) {
	Remove-Item -Force -Recurse ../build
}

New-Item -Name ../build/files -ItemType directory

Copy-Item -Path ../src/Menagerie/bin/Release/net6.0-windows/* -Destination ../build/files -Include *.dll, *.exe, *.json

echo Editing nuspec...
$content = Get-Content nuget-template.nuspec -Raw
$content = $content.Replace("#{version}", $version)
Set-Content ../build/Menagerie-$version.nuspec -Value $content

echo Packaging...
C:/Tools/nuget/nuget.exe pack ../build/Menagerie-$version.nuspec -OutputDirectory ../build
# $nuVersion = $version.substring(0, $version.lastIndexOf("."))
# Rename-Item -Path "../build/Menagerie.$nuVersion.nupkg" -NewName "Menagerie-$version.nupkg"

# echo Cleaning...
# Remove-Item -Force -Recurse ../build/files
# Remove-Item -Force ../build/Menagerie-$version.nuspec

# echo Squirrel...
# if(Test-Path -Path ../release) {
# 	Remove-Item -Force -Recurse ../release
# }

# &(Join-Path $env:USERPROFILE '.\.nuget\packages\squirrel.windows\1.4.4\tools\squirrel.exe') --releasify ("../build/Menagerie." + $version + ".nupkg") --releaseDir "../release"

# Start-Sleep -s 7

# Remove-Item -Force -Recurse ../build

# Rename-Item -Path "../release/Setup.exe" -NewName "Menagerie-$version-Setup.exe"