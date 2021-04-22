#!/bin/bash

set -x

PkgVersion="${BUILD_BUILDNUMBER%.*}"
PkgIteration="${BUILD_BUILDNUMBER##*.}"
PkgFullVersion="$PkgVersion-$PkgIteration"

###############################################################################
# Upload Debian packages                                                      #
###############################################################################

# amd64
package_cloud push fedarovich/qbittorrent-cli/debian/stretch "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/universal/qbittorrent-cli_${PkgFullVersion}_amd64.deb"
package_cloud push fedarovich/qbittorrent-cli/ubuntu/xenial "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/universal/qbittorrent-cli_${PkgFullVersion}_amd64.deb"

# armhf
package_cloud push fedarovich/qbittorrent-cli/debian/stretch "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/universal/qbittorrent-cli_${PkgFullVersion}_armhf.deb"
package_cloud push fedarovich/qbittorrent-cli/ubuntu/xenial "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/universal/qbittorrent-cli_${PkgFullVersion}_armhf.deb"

# arm64
package_cloud push fedarovich/qbittorrent-cli/debian/stretch "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/universal/qbittorrent-cli_${PkgFullVersion}_arm64.deb"
package_cloud push fedarovich/qbittorrent-cli/ubuntu/xenial "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/universal/qbittorrent-cli_${PkgFullVersion}_arm64.deb"

###############################################################################
# Upload Fedora/CentOS/RHEL packages                                          #
###############################################################################

# x86_64
package_cloud push fedarovich/qbittorrent-cli/fedora/32 "$BUILD_BINARIESDIRECTORY/pkg/fedora/32/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm"
package_cloud push fedarovich/qbittorrent-cli/opensuse/15.0 "$BUILD_BINARIESDIRECTORY/pkg/opensuse/15/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm"
package_cloud push fedarovich/qbittorrent-cli/opensuse/15.3 "$BUILD_BINARIESDIRECTORY/pkg/opensuse/tw/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm"

# armv7hl
package_cloud push fedarovich/qbittorrent-cli/fedora/32 "$BUILD_BINARIESDIRECTORY/pkg/fedora/32/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm"
package_cloud push fedarovich/qbittorrent-cli/opensuse/15.0 "$BUILD_BINARIESDIRECTORY/pkg/opensuse/15/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm"
package_cloud push fedarovich/qbittorrent-cli/opensuse/15.3 "$BUILD_BINARIESDIRECTORY/pkg/opensuse/tw/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm"

# aarch64
package_cloud push fedarovich/qbittorrent-cli/fedora/32 "$BUILD_BINARIESDIRECTORY/pkg/fedora/32/qbittorrent-cli-${PkgFullVersion}.aarch64.rpm"
package_cloud push fedarovich/qbittorrent-cli/opensuse/15.0 "$BUILD_BINARIESDIRECTORY/pkg/opensuse/15/qbittorrent-cli-${PkgFullVersion}.aarch64.rpm"
package_cloud push fedarovich/qbittorrent-cli/opensuse/15.3 "$BUILD_BINARIESDIRECTORY/pkg/opensuse/tw/qbittorrent-cli-${PkgFullVersion}.aarch64.rpm"

set +x
