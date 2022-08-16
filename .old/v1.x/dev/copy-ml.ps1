$mlFolder = "./Menagerie.ML/ML/"
$debugFolder = "./Menagerie/bin/Debug/net5.0-windows/"
$releaseFolder = "./Menagerie/bin/Release/net5.0-windows/"
$ideaDebugFolder = $debugFolder + "ML/.idea"
$ideaReleaseFolder = $releaseFolder + "ML/.idea"
$tempDebugFolder = $debugFolder + "ML/.temp/"
$tempReleaseFolder = $releaseFolder + "ML/.temp/"
$trainedDebugFolder = $debugFolder + "ML/trained/"
$trainedReleaseFolder = $releaseFolder + "ML/trained/"

Copy-item -Force -Recurse $mlFolder -Destination $debugFolder
Copy-item -Force -Recurse $mlFolder -Destination $releaseFolder

if (Test-Path -Path ideaDebugFolder) {
	Remove-Item -Path $ideaDebugFolder -Force -Recurse
}
if (Test-Path -Path ideaReleaseFolder) {
	Remove-Item -Path $ideaReleaseFolder -Force -Recurse
}
if (Test-Path -Path tempDebugFolder) {
	Remove-Item -Path $tempDebugFolder -Force -Recurse
}
if (Test-Path -Path tempReleaseFolder) {
	Remove-Item -Path $tempReleaseFolder -Force -Recurse
}
if (Test-Path -Path trainedDebugFolder) {
	Remove-Item -Path $trainedDebugFolder -Force -Recurse
}
if (Test-Path -Path trainedReleaseFolder) {
	Remove-Item -Path $trainedReleaseFolder -Force -Recurse
}
