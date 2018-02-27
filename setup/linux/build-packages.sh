#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

function unixpath() {
	if [ -e /bin/wslpath ]
	then
		/bin/wslpath -u -a "$1"
	else
		"$DIR/wslpath.sh" -u -r "$1"
	fi
}

function winvar() {
	local var="$(cmd.exe /c echo %$1%)"
	
	if [ $# -gt 1 ] && [ $2 == '-p' ]
	then
		echo $(unixpath "$var")
	else
		echo $var
	fi
}

function importvar() {
	declare -g $1="$(winvar $1 $2)"
}

importvar BUILD_REPOSITORY_LOCALPATH -p
echo AGENT_BUILDDIRECTORY=$BUILD_REPOSITORY_LOCALPATH

importvar BUILD_ARTIFACTSTAGINGDIRECTORY -p
echo BUILD_ARTIFACTSTAGINGDIRECTORY=$BUILD_ARTIFACTSTAGINGDIRECTORY

importvar BUILD_BUILDNUMBER
echo BUILD_BUILDNUMBER=$BUILD_BUILDNUMBER

importvar BUILD_DEFINITIONNAME
echo BUILD_DEFINITIONNAME=$BUILD_DEFINITIONNAME

importvar BuildConfiguration
echo BuildConfiguration=$BuildConfiguration

BuildDir="$HOME/build/$BUILD_DEFINITIONNAME/$BUILD_BUILDNUMBER"
mkdir -p "$BuildDir"
mkdir -p "$BuildDir/opt/qbt"
mkdir -p "$BuildDir/usr/bin"

BinDir="$AGENT_BUILDDIRECTORY/raw/linux-x64"
cp "$BinDir" "$BuildDir/opt/qbt"

fpm -s dir -t deb -C "$BuildDir" --name qbt-cli --version $BUILD_BUILDNUMBER --iteration 1 --description "qBittorrent remote command client client."
