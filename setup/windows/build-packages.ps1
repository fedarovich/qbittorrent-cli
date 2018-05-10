$Hash32 = "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\publish\qbittorrent-cli-$env:BUILD_BUILDNUMBER-x86.msi"
$Hash64 = "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\publish\qbittorrent-cli-$env:BUILD_BUILDNUMBER-x64.msi"

pushd "$env:BUILD_BINARIESDIRECTORY"

$RepoPath = "$env:BUILD_BINARIESDIRECTORY\qbittorrent-cli"
if (Test-Path "$RepoPath") {
    Remove-Item "$RepoPath" -Recurse -Force
}

choco new qbittorrent-cli `
    --version=$env:BUILD_BUILDNUMBER `
    --maintainer="Pavel Fedarovich" `
    maintainerrepo="https://github.com/fedarovich/qbittorrent-cli" `
    installertype=msi `
    url="https://dl.bintray.com/fedarovich/qbittorrent-cli-windows/qbittorrent-cli-$env:BUILD_BUILDNUMBER-x86.msi" `
    url64="https://dl.bintray.com/fedarovich/qbittorrent-cli-windows/qbittorrent-cli-$env:BUILD_BUILDNUMBER-x64.msi" `
    checksum="$($Hash32.Hash)" `
    checksum64="$($Hash64.Hash)"

pushd qbittorrent-cli

Invoke-WebRequest https://rawgit.com/fedarovich/qbittorrent-cli/master/LICENSE -OutFile .\tools\LICENSE.txt

cp -Path "$env:BUILD_SOURCESDIRECTORY\setup\windows\VERIFICATION.txt" -Destination .\tools\VERIFICATION.txt -Force

# Remove comments from the chocolateyinstall.ps1 script
$f="$env:BUILD_BINARIESDIRECTORY\qbittorrent-cli\tools\chocolateyinstall.ps1"
gc $f | ? {$_ -notmatch "^\s*#"} | % {$_ -replace '(^.*?)\s*?[^``]#.*','$1'} | Out-File $f+".~" -en utf8; mv -fo $f+".~" $f

choco pack --outputdirectory "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\publish"

popd
popd