#!/bin/bash

if [[ $EUID -ne 0 ]]
then
  echo "This script must be run as root."
  exit 1
fi

read -p "Are you sure that you want to uninstall qBittorrent CLI? (y/N) " -n 1 -r
echo

if [[ $REPLY =~ ^[Yy]$ ]]
then
  echo "Deleting files..."
  rm -fv  /usr/local/bin/qbt
  rm -fvr /usr/local/qbittorrent-cli
  pkgutil --forget com.fedarovich.pkg.qbittorrent-cli
fi
	