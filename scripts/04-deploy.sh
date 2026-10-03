#!/usr/bin/env bash
set -euo pipefail
source "$(dirname "$0")/00-variaveis.sh"

ROOT="$(cd "$(dirname "$0")/.." && pwd)"

dotnet publish "$ROOT/src/DimDim.Web" -c Release -o "$ROOT/publish"
(cd "$ROOT/publish" && zip -r ../publish.zip .)

az webapp deploy \
  --resource-group "$RG" --name "$WEBAPP" \
  --src-path "$ROOT/publish.zip" --type zip

echo ">> Deploy concluido: https://${WEBAPP}.azurewebsites.net"
