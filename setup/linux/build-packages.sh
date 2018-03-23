#!/bin/bash

BuildDir="$HOME/build/$BUILD_DEFINITIONNAME/$BUILD_BUILDNUMBER"
mkdir -p "$BuildDir"

#Prepare package files
PkgRoot="$BuildDir/root"
mkdir -p "$PkgRoot"
mkdir -p "$PkgRoot/opt/qbt-cli"
mkdir -p "$PkgRoot/usr/local/bin"

BinDir="$AGENT_BUILDDIRECTORY/raw/linux-x64"
cp -a "$BinDir/." "$PkgRoot/opt/qbt-cli"
ln -sf /opt/qbt-cli/qbt "$PkgRoot/usr/local/bin/qbt"

#Prepare output directories
mkdir -p "$BuildDir/packages"
pushd "$BuildDir"
rm -r "packages/*"
mkdir  -p "packages/ubuntu/trusty" 
mkdir  -p "packages/ubuntu/xenial"
popd

#TODO: Use correct debian naming conventions
fpm -s dir -t deb -f -C "$PkgRoot" --name qbt-cli --version $BUILD_BUILDNUMBER --iteration 1 --description "qBittorrent remote command line client." -p "$BuildDir/packages/ubuntu/trusty" -d libunwind8 -d libcurl3 -d libssl1.0.0 -d libicu52
fpm -s dir -t deb -f -C "$PkgRoot" --name qbt-cli --version $BUILD_BUILDNUMBER --iteration 1 --description "qBittorrent remote command line client." -p "$BuildDir/packages/ubuntu/xenial" -d libunwind8 -d libcurl3 -d libssl1.0.0 -d libicu55

mkdir -p "$BUILD_ARTIFACTSTAGINGDIRECTORY/setup"
cp -a "$BuildDir/packages/" "$BUILD_ARTIFACTSTAGINGDIRECTORY/setup/"
ls "$BUILD_ARTIFACTSTAGINGDIRECTORY/setup/"
