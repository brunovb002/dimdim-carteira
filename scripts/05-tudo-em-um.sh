#!/usr/bin/env bash
# Roda a criação dos recursos, o DDL, o publish e o deploy numa única execução,
# em vez de separar o deploy como um passo manual depois. Os scripts 02/03/04
# continuam existindo individualmente (documentação/granularidade), mas este é
# o fluxo recomendado para rodar de ponta a ponta sem intervenção manual no meio.
set -euo pipefail
DIR="$(dirname "$0")"
ROOT="$(cd "$DIR/.." && pwd)"

echo ">> [1/4] Criando recursos na Azure"
bash "$DIR/02-criar-recursos.sh"

echo ">> [2/4] Aplicando o DDL no banco"
bash "$DIR/03-aplicar-ddl.sh"

echo ">> [3/4] Publicando a aplicação"
dotnet publish "$ROOT/src/DimDim.Web" -c Release -o "$ROOT/publish"

if command -v zip >/dev/null 2>&1; then
  (cd "$ROOT/publish" && rm -f "$ROOT/publish.zip" && zip -r "$ROOT/publish.zip" .)
else
  echo "   (zip não encontrado, usando scripts/make-zip.ps1)"
  powershell -ExecutionPolicy Bypass -File "$DIR/make-zip.ps1"
fi

echo ">> [4/4] Fazendo o deploy"
source "$DIR/00-variaveis.sh"
az webapp deploy --resource-group "$RG" --name "$WEBAPP" --src-path "$ROOT/publish.zip" --type zip

echo ">> Tudo pronto: https://${WEBAPP}.azurewebsites.net"
