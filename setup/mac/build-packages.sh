#!/bin/bash

set -x

#prepare file system
PkgRoot="$BUILD_BINARIESDIRECTORY/build/root"
mkdir -p "$PkgRoot/usr/local/bin"
mkdir -p "$PkgRoot/usr/local/qbittorrent-cli"
tar -xvf "$BUILD_BINARIESDIRECTORY/tgz/cli/qbt-osx-x64-$BUILD_BUILDNUMBER.tar.gz" -C "$PkgRoot/usr/local/qbittorrent-cli"
chmod +x "$PkgRoot/usr/local/qbittorrent-cli/qbt"
ln -sf "../qbittorrent-cli/qbt" "$PkgRoot/usr/local/bin/qbt"
cp -f "$BUILD_SOURCESDIRECTORY/setup/mac/scripts/uninstall.sh" "$PkgRoot/usr/local/qbittorrent-cli/uninstall"
chmod +x "$PkgRoot/usr/local/qbittorrent-cli/uninstall"

#build packages
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/osx" && pkgbuild --root "$PkgRoot" --identifier "com.fedarovich.pkg.qbittorrent-cli" --install-location / --version $BUILD_BUILDNUMBER "$_/qbittorrent-cli-${BUILD_BUILDNUMBER}.pkg"

# Clean up
pushd "$BUILD_BINARIESDIRECTORY"
rm -rf tgz/* build/*
popd

set +x
