#!/bin/bash

###############################################################################
# Create packages for x64                                                     #
###############################################################################

#prepare file system
PkgRoot="$BUILD_BINARIESDIRECTORY/build/x64"
mkdir -p "$PkgRoot/usr/bin"
mkdir -p "$PkgRoot/usr/lib/qbittorrent-cli"
tar -xvf "$BUILD_BINARIESDIRECTORY/tgz/qbt-linux-x64-*.tag.gz" -C "$PkgRoot/usr/lib/qbittorrent-cli"
ln -sf "$PkgRoot/usr/lib/qbittorrent-cli/qbt" "$PkgRoot/usr/bin/qbt"

#build packages
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/xenial/x64" && fpm -s dir -t deb -f -C "$PkgRoot" --name qbittorrent-cli --version $BUILD_BUILDNUMBER --iteration 1 -a amd64 --description "qBittorrent remote command line client." -p "$_" -d libunwind8 -d libcurl3 -d libssl1.0.0 -d libicu55

###############################################################################
# Create packages for ARM                                                     #
###############################################################################
PkgRoot="$BUILD_BINARIESDIRECTORY/build/arm"
mkdir -p "$PkgRoot/usr/bin"
mkdir -p "$PkgRoot/usr/lib/qbittorrent-cli"
tar -xvf "$BUILD_BINARIESDIRECTORY/tgz/qbt-linux-arm-*.tag.gz" -C "$PkgRoot/usr/lib/qbittorrent-cli"
ln -sf "$PkgRoot/usr/lib/qbittorrent-cli/qbt" "$PkgRoot/usr/bin/qbt"

#build packages
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/xenial/armhf" && fpm -s dir -t deb -f -C "$PkgRoot" --name qbittorrent-cli --version $BUILD_BUILDNUMBER --iteration 1 -a armhf --description "qBittorrent remote command line client." -p "$_" -d libunwind8 -d libcurl3 -d libssl1.0.0 -d libicu55

###############################################################################
# Clean up                                                     #
###############################################################################
pushd "$BUILD_BINARIESDIRECTORY"
rm -rf tgz/* tgz/.* build/* build/.*
popd
