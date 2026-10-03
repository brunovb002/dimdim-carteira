#!/usr/bin/env bash
set -euo pipefail
source "$(dirname "$0")/00-variaveis.sh"

echo ">> Resource Group"
az group create --name "$RG" --location "$LOCATION"

echo ">> Azure SQL Server + Database"
az sql server create \
  --name "$SQL_SERVER" --resource-group "$RG" --location "$LOCATION" \
  --admin-user "$SQL_ADMIN" --admin-password "$SQL_PASSWORD"

az sql db create \
  --name "$SQL_DB" --server "$SQL_SERVER" --resource-group "$RG" \
  --service-objective Basic

# Libera serviços do Azure (o Web App) a acessar o SQL
az sql server firewall-rule create \
  --server "$SQL_SERVER" --resource-group "$RG" \
  --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0

# Libera o IP da máquina de quem está rodando (para aplicar o DDL)
MEU_IP=$(curl -s https://api.ipify.org)
az sql server firewall-rule create \
  --server "$SQL_SERVER" --resource-group "$RG" \
  --name AllowMyIP --start-ip-address "$MEU_IP" --end-ip-address "$MEU_IP"

echo ">> Log Analytics + Application Insights"
az monitor log-analytics workspace create \
  --resource-group "$RG" --workspace-name "$LAW" --location "$LOCATION"

az monitor app-insights component create \
  --app "$APPINSIGHTS" --location "$LOCATION" --resource-group "$RG" \
  --workspace "$LAW" --kind web --application-type web

AI_CONN=$(az monitor app-insights component show \
  --app "$APPINSIGHTS" --resource-group "$RG" --query connectionString -o tsv)

echo ">> App Service Plan + Web App"
az appservice plan create \
  --name "$PLAN" --resource-group "$RG" --location "$LOCATION" \
  --is-linux --sku B1

az webapp create \
  --name "$WEBAPP" --resource-group "$RG" --plan "$PLAN" --runtime "$RUNTIME"

echo ">> Configuracoes do Web App (segredos ficam so no Azure)"
CONN="Server=tcp:${SQL_SERVER}.database.windows.net,1433;Initial Catalog=${SQL_DB};User ID=${SQL_ADMIN};Password=${SQL_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

az webapp config connection-string set \
  --name "$WEBAPP" --resource-group "$RG" \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="$CONN"

az webapp config appsettings set \
  --name "$WEBAPP" --resource-group "$RG" \
  --settings APPLICATIONINSIGHTS_CONNECTION_STRING="$AI_CONN"

echo ">> Pronto. URL: https://${WEBAPP}.azurewebsites.net"
