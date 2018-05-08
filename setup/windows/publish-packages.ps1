Remove-Item Alias:Curl

curl -T "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\cli\qbittorrent-cli-$env:Version-x86" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-windows/qbittorrent-cli/$env:Version/qbittorrent-cli-$env:Version-x86.msi?publish=1"
curl -T "$env:BUILD_ARTIFACTSTAGINGDIRECTORY\cli\qbittorrent-cli-$env:Version-x64" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-windows/qbittorrent-cli/$env:Version/qbittorrent-cli-$env:Version-x64.msi?publish=1"
