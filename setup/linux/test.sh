#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
BUILD_DEFINITIONNAME="qbt-cli"
BUILD_BUILDNUMBER="1.0.0"
AGENT_BUILDDIRECTORY="/tmp/agent/build"
BUILD_ARTIFACTSTAGINGDIRECTORY="$HOME/build/artifacts"

mkdir -p "/tmp/agent/build"

ln -sf "$DIR/../../src/QBittorrent.CommandLineInterface/bin/publish" "$AGENT_BUILDDIRECTORY/raw"

. ./build-packages.sh
