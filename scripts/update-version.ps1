param(
	[Parameter(Mandatory=$true)][string]$major,
	[Parameter(Mandatory=$true)][string]$minor,
	[Parameter(Mandatory=$true)][string]$patch
)

$version = "$major.$minor.$patch"

$csprojFilePaths = @(
    '../src/Menagerie/Menagerie.csproj',
    '../src/Menagerie.Shared/Menagerie.Shared.csproj',
    '../src/Menagerie.Data/Menagerie.Data.csproj',
    '../src/Menagerie.Application/Menagerie.Application.csproj'
)

Write-Output "Retrieving commits..."
[xml]$appXmlDoc = Get-Content $csprojFilePaths[0]
$lastVersion = $appXmlDoc.Project.PropertyGroup.AssemblyVersion

$range = "v$lastVersion..HEAD"
$nbCommits = git rev-list $range --count 

if ($nbCommits -eq $null -or $nbCommits -eq "") {
    $buildVersion = $version + ".0"
} else {
    $buildVersion = $version + "." + $nbCommits
}

Write-Output $buildVersion

Write-Output "Updating version to $buildVersion..."
foreach ($csprojFilePath in $csprojFilePaths) {
    $path = resolve-path $csprojFilePath
    [xml]$xmlDoc = Get-Content $path
    $xmlDoc.Project.PropertyGroup.PackageVersion = $buildVersion
    $xmlDoc.Project.PropertyGroup.AssemblyVersion = $buildVersion
    $xmlDoc.Project.PropertyGroup.FileVersion = $buildVersion
    $xmlDoc.Save($path)
}

cd ..
git add .
$msg = "Version update $buildVersion"
git commit -m $msg
cd scripts
