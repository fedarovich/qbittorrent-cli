#!/bin/bash
SCRIPT_PATH="${BASH_SOURCE[0]}";
while [ -h "${SCRIPT_PATH}" ]; do
    DIR="$( cd -P "$( dirname "$SCRIPT_PATH" )" >/dev/null 2>&1 && pwd )"
    SCRIPT_PATH="$(readlink "$SCRIPT_PATH")"
    [[ $SCRIPT_PATH != /* ]] && SCRIPT_PATH="$DIR/$SCRIPT_PATH"
done
DIR="$( cd -P "$( dirname "$SCRIPT_PATH" )" >/dev/null 2>&1 && pwd )"
exec dotnet "$DIR/qbt.dll" "$@"
