# qbittorrent-cli
Command line interface for QBittorrent

The binaries are available for different platforms:

Package | Platform | Dependencies
---- | -------- | ------------
qbt-netfx461.zip | Windows | .Net Framework 4.6.1 or later.
qbt-core20-portable | Any platform | .Net Core 2.0 runtime
qbt-win-x86.zip | Windows 32-bit | None
qbt-win-x64.zip | Windows 64-bit | None
qbt-linux-x64.zip | Linux 64-bit | None
qbt-osx-x64.zip | Mac OS X 64-bit (version 10.10 or later) | None

To see the list of currently available commands run (any package except `qbt-core20-portable`):

    qbt --help

or for `qbt-core20-portable`

    dotnet qbt.dll --help
