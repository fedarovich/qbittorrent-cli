#!/bin/bash

# This is free and unencumbered software released into the public domain.
#
# Anyone is free to copy, modify, publish, use, compile, sell, or
# distribute this software, either in source code form or as a compiled
# binary, for any purpose, commercial or non-commercial, and by any
# means.
#
# In jurisdictions that recognize copyright laws, the author or authors
# of this software dedicate any and all copyright interest in the
# software to the public domain. We make this dedication for the benefit
# of the public at large and to the detriment of our heirs and
# successors. We intend this dedication to be an overt act of
# relinquishment in perpetuity of all present and future rights to this
# software under copyright law.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
# EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
# MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
# IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
# OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
# ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
# OTHER DEALINGS IN THE SOFTWARE.
#
# For more information, please refer to <http://unlicense.org/>

# Author: djcj <djcj@gmx.de>

last=${!#}

function print_usage() {
  cat <<EOL
usage:
$(basename $0) [-u|-w|-m] [-a] path
$(basename $0) -h|--help

Convert Unix and Windows format paths.

Output type options:
  -u, --unix        (default) print Unix form of NAMEs (/mnt/c/Windows)
  -w, --windows     print Windows form of NAMEs (C:\\Windows)
  -m, --mixed       like --windows, but with regular slashes (C:/Windows)

Path conversion options:
  -r, --realpath    output absolute path with resolved symbolic links
  -s, --subst-home  substitute Unix HOME path with Windows user path

Other options:
  -h, --help        output usage information and exit

EOL
  exit $1
}

function checkarg() {
  if [ "$1" = "$last" ]; then
    echo "Error: missing path argument"
    exit 1
  fi
}

function win_env() {
  echo $(cmd.exe /C echo %${1}% 2>/dev/null | sed -e 's|\r||;s|\\|/|g')
}

if !(grep -q 'Microsoft' /proc/version 2>/dev/null || 
     grep -q 'Microsoft' /proc/sys/kernel/osrelease 2>/dev/null)
then
  echo "Warning: script was made for \"Bash on Windows\"" > /dev/stderr
fi

if [ -z "$1" ]; then
  print_usage 1
fi

to_unix=yes
mixed_mode=no
realpath=no
subst_home=no

while [[ $# -gt 0 ]]; do
  case $1 in
    -w|--windows)
      to_unix=no
      checkarg $1
      ;;
    -u|--unix)
      to_unix=yes
      checkarg $1
      ;;
    -m|--mixed)
      to_unix=no
      mixed_mode=yes
      checkarg $1
      ;;
    -r|--realpath)
      realpath=yes
      checkarg $1
      ;;
    -s|--subst-home)
      subst_home=yes
      ;;
    -h|--help)
      print_usage 0
      ;;
    *)
      ;;
  esac
  shift
done

p=$(echo $last | sed -e 's|\\\+|/|g;s|/\+|/|g')

lxss=$(win_env LOCALAPPDATA)/lxss
if [ $subst_home = yes ] && [ $to_unix = no ]; then
  userprofile=$(win_env USERPROFILE)
  drive=${userprofile:0:1}
  win_home=/mnt/${drive,,}${userprofile:2}
fi

if [ $to_unix = yes ]; then
  # windows to unix
  lxssLen=$(echo $lxss | wc -m)
  if [ "${p:0:$lxssLen}" = "$lxss" ]; then
    subdir=$(echo $p | cut -d'/' -f7)
    lxssDirs=$(grep -e ' lxfs ' /proc/mounts | awk '{print $1}')
    if [ -n "$(echo $lxssDirs | tr ' ' '\n' | grep -e "^$subdir\$")" ]; then
      offset=$(echo $lxss/$subdir | wc -m)
      prefix=$(grep -e "^$subdir " /proc/mounts | awk '{print $2}')
      [ "$prefix" != / ] || prefix=""
      p=$prefix${p:$offset}
    fi
  fi
  if [ -n "$(echo ${p:0:3} | grep -e '^[A-Za-z]:/\?$')" ]; then
    drive=${p:0:1}
    p=/mnt/${drive,,}${p:2}
  fi
  if [ $realpath = yes ]; then
    p=$(realpath -m "$p")
  fi
else
  # unix to windows
  if [ $realpath = yes ]; then
    p=$(realpath -m "$p")
  fi
  if [ "${p:0:1}" = / ]; then
    if [ $subst_home = yes ]; then
      p=$(echo $p | sed "s|^$HOME|$win_home|")
    fi
    if [ -n "$(echo ${p:0:7} | grep -e '^/mnt/[a-z]/\?$')" ]; then
      drive=${p:5:1}
      p=${drive^^}:${p:6}
    else
      firstDir=/$(echo $p | cut -d'/' -f2)
      offset=$(printf "$firstDir" | wc -m)
      rootDirs=$(grep -e ' lxfs ' /proc/mounts | awk '{print $2}')
      if [ -z "$(echo $rootDirs | tr ' ' '\n' | grep -e "^$firstDir\$")" ]; then
        firstDir=/
        offset=0
      fi
      p=$lxss/$(grep -e " $firstDir " /proc/mounts | awk '{print $1}')${p:$offset}
    fi
  fi
  if [ $mixed_mode = no ]; then
    p=$(echo $p | tr '/' '\\')
  fi
fi

echo $p

