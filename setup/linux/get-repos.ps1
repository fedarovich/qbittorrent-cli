[CmdletBinding()]
Param()

New-Item -ItemType directory -Path 'repo/ubuntu/xenial' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.deb.txt?distro=ubuntu&codename=xenial' --output repo/ubuntu/xenial/qbittorrent-cli.list
New-Item -ItemType directory -Path 'repo/ubuntu/bionic' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.deb.txt?distro=ubuntu&codename=bionic' --output repo/ubuntu/bionic/qbittorrent-cli.list
New-Item -ItemType directory -Path 'repo/ubuntu/focal' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.deb.txt?distro=ubuntu&codename=focal' --output repo/ubuntu/focal/qbittorrent-cli.list
New-Item -ItemType directory -Path 'repo/ubuntu/groovy' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.deb.txt?distro=ubuntu&codename=groovy' --output repo/ubuntu/groovy/qbittorrent-cli.list
New-Item -ItemType directory -Path 'repo/ubuntu/hirsute' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.deb.txt?distro=ubuntu&codename=hirsute' --output repo/ubuntu/hirsute/qbittorrent-cli.list
New-Item -ItemType directory -Path 'repo/debian/stretch' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.deb.txt?distro=debian&codename=any-version' --output repo/debian/stretch/qbittorrent-cli.list
New-Item -ItemType directory -Path 'repo/debian/buster' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.deb.txt?distro=debian&codename=any-version' --output repo/debian/buster/qbittorrent-cli.list

New-Item -ItemType directory -Path 'repo/opensuse/15' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.rpm.txt?distro=opensuse&codename=15.2' --output repo/opensuse/15/qbittorrent-cli.repo
New-Item -ItemType directory -Path 'repo/opensuse/tw' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.rpm.txt?distro=opensuse&codename=Tumbleweed' --output repo/opensuse/tw/qbittorrent-cli.repo

New-Item -ItemType directory -Path 'repo/fedora/32' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.rpm.txt?distro=fedora&codename=any-version' --output repo/fedora/32/qbittorrent-cli.repo
New-Item -ItemType directory -Path 'repo/fedora/33' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.rpm.txt?distro=fedora&codename=any-version' --output repo/fedora/33/qbittorrent-cli.repo
New-Item -ItemType directory -Path 'repo/fedora/34' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.rpm.txt?distro=fedora&codename=any-version' --output repo/fedora/34/qbittorrent-cli.repo
New-Item -ItemType directory -Path 'repo/fedora/35' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.rpm.txt?distro=fedora&codename=any-version' --output repo/fedora/35/qbittorrent-cli.repo

New-Item -ItemType directory -Path 'repo/el/7' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.rpm.txt?distro=el&codename=any-version' --output repo/el/7/qbittorrent-cli.repo
New-Item -ItemType directory -Path 'repo/el/8' -Force; curl.exe -1sLf 'https://dl.cloudsmith.io/public/qbittorrent-cli/qbittorrent-cli/config.rpm.txt?distro=el&codename=any-version' --output repo/el/8/qbittorrent-cli.repo
