#!/bin/bash

PkgName="qbittorrent-cli"
PkgDesc="qBittorrent remote command line client."
PkgVersion="${BUILD_BUILDNUMBER%.*}"
PkgIteration="${BUILD_BUILDNUMBER##*.}"

###############################################################################
# Create packages for x64                                                     #
###############################################################################

#prepare file system
PkgRoot="$BUILD_BINARIESDIRECTORY/build/x64"
mkdir -p "$PkgRoot/usr/bin"
mkdir -p "$PkgRoot/usr/lib/qbittorrent-cli"
tar -xvf "$BUILD_BINARIESDIRECTORY/tgz/cli/qbt-linux-x64-$BUILD_BUILDNUMBER.tar.gz" -C "$PkgRoot/usr/lib/qbittorrent-cli"
chmod +x "$PkgRoot/usr/lib/qbittorrent-cli/qbt"
ln -sfr "$PkgRoot/usr/lib/qbittorrent-cli/qbt" "$PkgRoot/usr/bin/qbt"

#build packages
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/trusty" && fpm -s dir -t deb -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a amd64 --description "$PkgDesc" -p "$_" -d libunwind8 -d libcurl3 -d libssl1.0.0 -d libicu52
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/xenial" && fpm -s dir -t deb -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a amd64 --description "$PkgDesc" -p "$_" -d libunwind8 -d libcurl3 -d libssl1.0.0 -d libicu55
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/artful" && fpm -s dir -t deb -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a amd64 --description "$PkgDesc" -p "$_" -d libunwind8 -d libcurl3 -d "libssl1.0.0 | libssl1.0.2" -d libicu57
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/bionic" && fpm -s dir -t deb -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a amd64 --description "$PkgDesc" -p "$_" -d libunwind8 -d libcurl3 -d libssl1.0.0 -d libicu60

mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/fedora/25" && fpm -s dir -t rpm -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a x86_64 --description "$PkgDesc" -p "$_" -d libunwind -d libcurl -d openssl-libs -d libicu
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/fedora/26" && fpm -s dir -t rpm -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a x86_64 --description "$PkgDesc" -p "$_" -d libunwind -d libcurl -d openssl-libs -d libicu -d compat-openssl10

mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/opensuse/42" && fpm -s dir -t rpm -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a x86_64 --description "$PkgDesc" -p "$_" -d libunwind -d libcurl4 -d libssl43 -d libicu

###############################################################################
# Create packages for ARM                                                     #
###############################################################################
PkgRoot="$BUILD_BINARIESDIRECTORY/build/arm"
mkdir -p "$PkgRoot/usr/bin"
mkdir -p "$PkgRoot/usr/lib/qbittorrent-cli"
tar -xvf "$BUILD_BINARIESDIRECTORY/tgz/cli/qbt-linux-arm-$BUILD_BUILDNUMBER.tar.gz" -C "$PkgRoot/usr/lib/qbittorrent-cli"
chmod +x "$PkgRoot/usr/lib/qbittorrent-cli/qbt"
ln -sfr "$PkgRoot/usr/lib/qbittorrent-cli/qbt" "$PkgRoot/usr/bin/qbt"

#build packages
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/trusty" && fpm -s dir -t deb -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a armhf --description "$PkgDesc" -p "$_" -d libunwind8 -d libcurl3 -d libssl1.0.0 -d libicu52
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/xenial" && fpm -s dir -t deb -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a armhf --description "$PkgDesc" -p "$_" -d libunwind8 -d libcurl3 -d libssl1.0.0 -d libicu55
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/artful" && fpm -s dir -t deb -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a armhf --description "$PkgDesc" -p "$_" -d libunwind8 -d libcurl3 -d "libssl1.0.0 | libssl1.0.2" -d libicu57
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/bionic" && fpm -s dir -t deb -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a armhf --description "$PkgDesc" -p "$_" -d libunwind8 -d libcurl3 -d libssl1.0.0 -d libicu60

mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/fedora/25" && fpm -s dir -t rpm -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a armv7hl --description "$PkgDesc" -p "$_" -d libunwind -d libcurl -d openssl-libs -d libicu
mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/fedora/26" && fpm -s dir -t rpm -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a armv7hl --description "$PkgDesc" -p "$_" -d libunwind -d libcurl -d openssl-libs -d libicu -d compat-openssl10

mkdir -p "$BUILD_BINARIESDIRECTORY/pkg/opensuse/42" && fpm -s dir -t rpm -f -C "$PkgRoot" --name $PkgName --version $PkgVersion --iteration $PkgIteration -a armv7hl --description "$PkgDesc" -p "$_" -d libunwind -d libcurl4 -d libssl43 -d libicu

###############################################################################
# Clean up                                                     #
###############################################################################
pushd "$BUILD_BINARIESDIRECTORY"
rm -rf tgz/* build/*
popd
