param(
	[Parameter(Mandatory = $true)][string]$version
)

echo Building projects...
dotnet build .\Menagerie.Core\ -c Release
dotnet build .\Menagerie\ -c Release

echo Copying build files...
if (Test-Path -Path build) {
	echo Deleting..
	Remove-Item -Force -Recurse build
} 

New-Item -Name build\files -ItemType Directory
New-Item -Name build\files\ML -ItemType Directory
New-Item -Name build\files\ML\images -ItemType Directory

Copy-Item -Path Menagerie\bin\Release\net5.0-windows\* -Destination build\files -Include *.dll, *.exe, *.json
Copy-Item -Path Menagerie.ML\ML\* -Destination build\files\ML -Include *.py, *.jpeg, *.txt
Copy-Item -Path Menagerie.ML\ML\images\* -Destination build\files\ML\images\ -Include *.jpeg

echo Editing Menagerie.nuspec...
$content = Get-Content MenagerieTemplate.nuspec -Raw
$content = $content.Replace("#{version}", $version)
Set-Content build\Menagerie-$version.nuspec -Value $content

echo Packaging...
D:\nuget.exe pack build\Menagerie-$version.nuspec -OutputDirectory build 

echo Cleaning...
Remove-Item -Force -Recurse build\files
Remove-Item -Force build\Menagerie-$version.nuspec

echo Squirrel...
./packages/squirrel.windows.2.0.1/tools/Squirrel.exe --releasify ("./build/Menagerie." + $version + ".nupkg")
