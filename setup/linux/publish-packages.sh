#!/bin/bash

PkgVersion="${BUILD_BUILDNUMBER%.*}"
PkgIteration="${BUILD_BUILDNUMBER##*.}"
PkgFullVersion="$PkgVersion-$PkgIteration"

###############################################################################
# Upload Debian packages                                                      #
###############################################################################

# amd64
curl -T "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/trusty/qbittorrent-cli_${PkgFullVersion}_amd64.deb" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-debian/qbittorrent-cli/${PkgFullVersion}/trusty/pool/main/q/qbittorrent-cli_${PkgFullVersion}_amd64.deb;deb_distribution=trusty;deb_distribution=jessie;deb_component=main;deb_architecture=amd64?publish=1"
curl -T "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/xenial/qbittorrent-cli_${PkgFullVersion}_amd64.deb" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-debian/qbittorrent-cli/${PkgFullVersion}/xenial/pool/main/q/qbittorrent-cli_${PkgFullVersion}_amd64.deb;deb_distribution=xenial;deb_component=main;deb_architecture=amd64?publish=1"
curl -T "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/artful/qbittorrent-cli_${PkgFullVersion}_amd64.deb" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-debian/qbittorrent-cli/${PkgFullVersion}/artful/pool/main/q/qbittorrent-cli_${PkgFullVersion}_amd64.deb;deb_distribution=artful;deb_distribution=stretch;deb_component=main;deb_architecture=amd64?publish=1"
curl -T "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/bionic/qbittorrent-cli_${PkgFullVersion}_amd64.deb" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-debian/qbittorrent-cli/${PkgFullVersion}/bionic/pool/main/q/qbittorrent-cli_${PkgFullVersion}_amd64.deb;deb_distribution=bionic;deb_component=main;deb_architecture=amd64?publish=1"

# armhf
curl -T "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/trusty/qbittorrent-cli_${PkgFullVersion}_armhf.deb" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-debian/qbittorrent-cli/${PkgFullVersion}/trusty/pool/main/q/qbittorrent-cli_${PkgFullVersion}_armhf.deb;deb_distribution=trusty;deb_distribution=jessie;deb_component=main;deb_architecture=armhf?publish=1"
curl -T "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/xenial/qbittorrent-cli_${PkgFullVersion}_armhf.deb" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-debian/qbittorrent-cli/${PkgFullVersion}/xenial/pool/main/q/qbittorrent-cli_${PkgFullVersion}_armhf.deb;deb_distribution=xenial;deb_component=main;deb_architecture=armhf?publish=1"
curl -T "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/artful/qbittorrent-cli_${PkgFullVersion}_armhf.deb" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-debian/qbittorrent-cli/${PkgFullVersion}/artful/pool/main/q/qbittorrent-cli_${PkgFullVersion}_armhf.deb;deb_distribution=artful;deb_distribution=stretch;deb_component=main;deb_architecture=armhf?publish=1"
curl -T "$BUILD_BINARIESDIRECTORY/pkg/ubuntu/bionic/qbittorrent-cli_${PkgFullVersion}_armhf.deb" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-debian/qbittorrent-cli/${PkgFullVersion}/bionic/pool/main/q/qbittorrent-cli_${PkgFullVersion}_armhf.deb;deb_distribution=bionic;deb_component=main;deb_architecture=armhf?publish=1"

curl --user "fedarovich:$1" --data '' https://bintray.com/api/v1/calc_metadata/fedarovich/qbittorrent-cli-debian

###############################################################################
# Upload Fedora/CentOS/RHEL packages                                          #
###############################################################################

# x86_64
curl -T "$BUILD_BINARIESDIRECTORY/pkg/fedora/26/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-rpm/qbittorrent-cli/${PkgFullVersion}/fedora/26/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm?publish=1"
curl -T "$BUILD_BINARIESDIRECTORY/pkg/opensuse/42/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-rpm/qbittorrent-cli/${PkgFullVersion}/opensuse/42/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm?publish=1"
curl -T "$BUILD_BINARIESDIRECTORY/pkg/opensuse/15/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-rpm/qbittorrent-cli/${PkgFullVersion}/opensuse/15/qbittorrent-cli-${PkgFullVersion}.x86_64.rpm?publish=1"

# armv7hl
curl -T "$BUILD_BINARIESDIRECTORY/pkg/fedora/26/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-rpm/qbittorrent-cli/${PkgFullVersion}/fedora/26/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm?publish=1"
curl -T "$BUILD_BINARIESDIRECTORY/pkg/opensuse/42/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-rpm/qbittorrent-cli/${PkgFullVersion}/opensuse/42/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm?publish=1"
curl -T "$BUILD_BINARIESDIRECTORY/pkg/opensuse/15/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm" "-ufedarovich:$1" "https://api.bintray.com/content/fedarovich/qbittorrent-cli-rpm/qbittorrent-cli/${PkgFullVersion}/opensuse/15/qbittorrent-cli-${PkgFullVersion}.armv7hl.rpm?publish=1"
