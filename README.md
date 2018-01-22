# qbittorrent-cli
Command line interface for QBittorrent

The binaries are available for different platforms:

Package | Platform | Dependencies
---- | -------- | ------------
qbt-netfx461.zip | Windows 7 or later | .Net Framework 4.6.1 or later (included in Windows 10 v1511)
qbt-core20-portable | Any platform | .Net Core runtime 2.0 or later
qbt-win-x86.zip | Windows 7 or later (32-bit) | None
qbt-win-x64.zip | Windows 7 or later (64-bit) | None
qbt-linux-x64.zip | Linux (64-bit) | None
qbt-osx-x64.zip | Mac OS X 10.10 or later (64-bit) | None

**This software is at an early development stage and only several commands are available now. The syntax of the commands may change in future**

To see the list of currently available commands run (any package except `qbt-core20-portable`):

    qbt --help

or for `qbt-core20-portable`

    dotnet qbt.dll --help
