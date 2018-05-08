[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$BinTrayApiKey
)

Remove-Item Alias:Curl

curl --silent --show-error --upload-file "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\publish\qbittorrent-cli-$env:BUILD_BUILDNUMBER-x86.msi" "-ufedarovich:$BinTrayApiKey" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-windows/qbittorrent-cli/$env:BUILD_BUILDNUMBER/qbittorrent-cli-$env:BUILD_BUILDNUMBER-x86.msi?publish=1"
curl --silent --show-error --upload-file "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\publish\qbittorrent-cli-$env:BUILD_BUILDNUMBER-x64.msi" "-ufedarovich:$BinTrayApiKey" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-windows/qbittorrent-cli/$env:BUILD_BUILDNUMBER/qbittorrent-cli-$env:BUILD_BUILDNUMBER-x64.msi?publish=1"
