#!/usr/bin/env bash
# Ajuste os valores. NÃO colocar senha aqui.
export SUFIXO="dimdim566366v2"              # tema DimDim + RM (v2: nomes antigos ficaram reservados globalmente após desativação da assinatura anterior)
export LOCATION="centralus"                  # regiões permitidas pela política da assinatura: canadacentral, eastus, southcentralus, centralus, northcentralus (eastus sem capacidade p/ SQL no momento)
export RG="rg-dimdim-${SUFIXO}"
export SQL_SERVER="sql-dimdim-${SUFIXO}"
export SQL_DB="dimdim-db"
export SQL_ADMIN="dimdimadmin"
export PLAN="plan-dimdim-${SUFIXO}"
export WEBAPP="app-dimdim-${SUFIXO}"
export LAW="law-dimdim-${SUFIXO}"
export APPINSIGHTS="ai-dimdim-${SUFIXO}"
export RUNTIME="DOTNETCORE:10.0"            # conferir com: az webapp list-runtimes --os linux

# A senha vem do ambiente (defina no terminal antes de rodar):
#   export SQL_PASSWORD='SuaSenhaForte#2026'
: "${SQL_PASSWORD:?Defina a variavel SQL_PASSWORD antes de rodar}"
