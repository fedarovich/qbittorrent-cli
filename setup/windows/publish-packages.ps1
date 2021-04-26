[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$MyGetApiKey
)

Remove-Item Alias:Curl

# publish chocolatey package to MyGet
echo "Publishing qbittorrent-cli.$env:BUILD_BUILDNUMBER.nupkg to MyGet..."
choco push "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\pkg\chocolatey\qbittorrent-cli.$env:BUILD_BUILDNUMBER.nupkg" --source https://www.myget.org/F/qbittorrent-cli/api/v2/package --api-key $MyGetApiKey
