Remove-Item Alias:Curl

curl -T "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\cli\qbittorrent-cli-$env:BUILD_BUILDNUMBER-x86.msi" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-windows/qbittorrent-cli/$env:BUILD_BUILDNUMBER/qbittorrent-cli-$env:BUILD_BUILDNUMBER-x86.msi?publish=1"
curl -T "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\cli\qbittorrent-cli-$env:BUILD_BUILDNUMBER-x64.msi" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-windows/qbittorrent-cli/$env:BUILD_BUILDNUMBER/qbittorrent-cli-$env:BUILD_BUILDNUMBER-x64.msi?publish=1"
