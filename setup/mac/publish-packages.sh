#!/bin/bash

curl -T "$BUILD_BINARIESDIRECTORY/pkg/osx/qbittorrent-cli-${BUILD_BUILDNUMBER}.pkg" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-mac/qbittorrent-cli/${BUILD_BUILDNUMBER}/qbittorrent-cli-${BUILD_BUILDNUMBER}.pkg?publish=1"
