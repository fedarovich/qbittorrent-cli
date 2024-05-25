#!/bin/bash

set -x

PkgVersion="${BUILD_BUILDNUMBER%.*}"
PkgIteration="${BUILD_BUILDNUMBER##*.}"
PkgFullVersion="$PkgVersion-$PkgIteration"

###############################################################################
# Upload Debian packages                                                      #
###############################################################################

# amd64
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/xenial "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/xenial/qbittorrent-cli_${PkgFullVersion}_amd64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/bionic "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/bionic/qbittorrent-cli_${PkgFullVersion}_amd64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/focal "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/focal/qbittorrent-cli_${PkgFullVersion}_amd64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/jammy "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/jammy/qbittorrent-cli_${PkgFullVersion}_amd64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/kinetic "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/jammy/qbittorrent-cli_${PkgFullVersion}_amd64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/noble "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/noble/qbittorrent-cli_${PkgFullVersion}_amd64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/debian/any-version "$BUILD_BINARIESDIRECTORY/pkg/debian/any-version/qbittorrent-cli_${PkgFullVersion}_amd64.deb" -k $1

# armhf
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/xenial "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/xenial/qbittorrent-cli_${PkgFullVersion}_armhf.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/bionic "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/bionic/qbittorrent-cli_${PkgFullVersion}_armhf.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/focal "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/focal/qbittorrent-cli_${PkgFullVersion}_armhf.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/jammy "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/jammy/qbittorrent-cli_${PkgFullVersion}_armhf.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/kinetic "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/jammy/qbittorrent-cli_${PkgFullVersion}_armhf.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/noble "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/noble/qbittorrent-cli_${PkgFullVersion}_armhf.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/debian/any-version "$BUILD_BINARIESDIRECTORY/pkg/debian/any-version/qbittorrent-cli_${PkgFullVersion}_armhf.deb" -k $1

# arm64
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/xenial "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/xenial/qbittorrent-cli_${PkgFullVersion}_arm64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/bionic "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/bionic/qbittorrent-cli_${PkgFullVersion}_arm64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/focal "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/focal/qbittorrent-cli_${PkgFullVersion}_arm64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/jammy "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/jammy/qbittorrent-cli_${PkgFullVersion}_arm64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/kinetic "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/jammy/qbittorrent-cli_${PkgFullVersion}_arm64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/ubuntu/noble "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/noble/qbittorrent-cli_${PkgFullVersion}_arm64.deb" -k $1
cloudsmith push deb qbittorrent-cli/qbittorrent-cli/debian/any-version "$BUILD_BINARIESDIRECTORY/pkg/debian/any-version/qbittorrent-cli_${PkgFullVersion}_arm64.deb" -k $1

###############################################################################
# Upload Fedora/CentOS/RHEL packages                                          #
###############################################################################

# x86_64
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/fedora/any-version "$BUILD_BINARIESDIRECTORY/pkg/fedora/33/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm" -k $1
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/el/any-version "$BUILD_BINARIESDIRECTORY/pkg/fedora/33/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm" -k $1
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/opensuse/15.2 "$BUILD_BINARIESDIRECTORY/pkg/opensuse/15/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm" -k $1
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/opensuse/Tumbleweed "$BUILD_BINARIESDIRECTORY/pkg/opensuse/tw/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm" -k $1

# armv7hl
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/fedora/any-version "$BUILD_BINARIESDIRECTORY/pkg/fedora/33/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm" -k $1
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/el/any-version "$BUILD_BINARIESDIRECTORY/pkg/fedora/33/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm" -k $1
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/opensuse/15.2 "$BUILD_BINARIESDIRECTORY/pkg/opensuse/15/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm" -k $1
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/opensuse/Tumbleweed "$BUILD_BINARIESDIRECTORY/pkg/opensuse/tw/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm" -k $1

# aarch64
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/fedora/any-version "$BUILD_BINARIESDIRECTORY/pkg/fedora/33/qbittorrent-cli-${PkgFullVersion}.aarch64.rpm" -k $1
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/el/any-version "$BUILD_BINARIESDIRECTORY/pkg/fedora/33/qbittorrent-cli-${PkgFullVersion}.aarch64.rpm" -k $1
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/opensuse/15.2 "$BUILD_BINARIESDIRECTORY/pkg/opensuse/15/qbittorrent-cli-${PkgFullVersion}.aarch64.rpm" -k $1
cloudsmith push rpm qbittorrent-cli/qbittorrent-cli/opensuse/Tumbleweed "$BUILD_BINARIESDIRECTORY/pkg/opensuse/tw/qbittorrent-cli-${PkgFullVersion}.aarch64.rpm" -k $1

set +x
