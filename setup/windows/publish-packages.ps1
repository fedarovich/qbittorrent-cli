[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$BinTrayApiKey,

    [Parameter(Mandatory=$True)]
    [string]$MyGetApiKey
)

Remove-Item Alias:Curl

# publish MSI packages to BinTray
echo "Publishing qbittorrent-cli-$env:BUILD_BUILDNUMBER-x86.msi to BinTray..."
curl --silent --show-error --upload-file "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\publish\qbittorrent-cli-$env:BUILD_BUILDNUMBER-x86.msi" "-ufedarovich:$BinTrayApiKey" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-windows/qbittorrent-cli/$env:BUILD_BUILDNUMBER/qbittorrent-cli-$env:BUILD_BUILDNUMBER-x86.msi?publish=1"
echo "Publishing qbittorrent-cli-$env:BUILD_BUILDNUMBER-x64.msi to BinTray..."
curl --silent --show-error --upload-file "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\publish\qbittorrent-cli-$env:BUILD_BUILDNUMBER-x64.msi" "-ufedarovich:$BinTrayApiKey" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-windows/qbittorrent-cli/$env:BUILD_BUILDNUMBER/qbittorrent-cli-$env:BUILD_BUILDNUMBER-x64.msi?publish=1"

# publish chocolatey package to MyGet
echo "Publishing qbittorrent-cli.$env:BUILD_BUILDNUMBER.nupkg to MyGet..."
choco push "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\pkg\chocolatey\qbittorrent-cli.$env:BUILD_BUILDNUMBER.nupkg" --source https://www.myget.org/F/qbittorrent-cli/api/v2/package --api-key $MyGetApiKey
