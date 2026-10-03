#!/usr/bin/env bash
set -euo pipefail
source "$(dirname "$0")/00-variaveis.sh"

sqlcmd -S "${SQL_SERVER}.database.windows.net" -d "$SQL_DB" \
  -U "$SQL_ADMIN" -P "$SQL_PASSWORD" -N \
  -i "$(dirname "$0")/01-ddl.sql"
