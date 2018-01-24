# qbittorrent-cli
Command line interface for QBittorrent

The binaries are available for different platforms:

Package | Platform | Dependencies
---- | -------- | ------------
qbt-netfx461.zip | Windows 7 or later | .Net Framework 4.6.1 or later (included in Windows 10 v1511)
qbt-core20-portable | Any platform | .Net Core runtime 2.0 or later
qbt-win-x86.zip | Windows 7 or later (32-bit) | None
qbt-win-x64.zip | Windows 7 or later (64-bit) | None
qbt-linux-x64.zip | Linux (64-bit) | *See below*
qbt-osx-x64.zip | Mac OS X 10.10 or later (64-bit) | None

**This software is at an early development stage and only several commands are available now. The syntax of the commands may change in future**

To see the list of currently available commands run (any package except `qbt-core20-portable`):

    qbt --help

or for `qbt-core20-portable`

    dotnet qbt.dll --help

### Prerequisites on Linux
The application has been tested on **Ubuntu 16.04 TLS** and **openSUSE Leap 42**, but it should run on any distro supported by .Net Core:
* Red Hat Enterprise Linux 7
* CentOS 7
* Oracle Linux 7
* Fedora 25, Fedora 26
* Debian 8.7 or later versions
* Ubuntu 17.04, Ubuntu 16.04, Ubuntu 14.04
* Linux Mint 18, Linux Mint 17
* openSUSE 42.2 or later versions
* SUSE Enterprise Linux (SLES) 12 SP2 or later versions

#### The following libraries are known to be required in order to run the application:
##### Ubuntu 16.04
* libunwind8
* libcurl3
* libssl1.0.0
* libicu55
##### openSUSE 42
* libunwind
* libicu

To see the full list of libraries required by .Net Core 2.0 (but not necessary by this application) follow the [link](https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites?tabs=netcore2x).
