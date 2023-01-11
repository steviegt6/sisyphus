#!/usr/bin/env bash

cd "$(dirname "$0")" || { echo "Unable to cd into $(dirname "$0")"; exit; }

./delete_cache.sh
./build.sh
./publish.sh
