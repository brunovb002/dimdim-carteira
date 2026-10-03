# Roteiro de gravação — DimDim Carteira

Baseado na seção 9 do `PLANO.md`, com os nomes reais dos recursos já provisionados nesta conta Azure. Grave em 1080p, narrando cada etapa. Cada etapa faltante vale -1 ponto, então siga a ordem.

**Recursos já criados (region `centralus`):**

| Recurso | Nome |
|---|---|
| Resource Group | `rg-dimdim-dimdim566366` |
| Azure SQL Server | `sql-dimdim-dimdim566366.database.windows.net` |
| Azure SQL Database | `dimdim-db` |
| App Service | `app-dimdim-dimdim566366` → https://app-dimdim-dimdim566366.azurewebsites.net |
| Application Insights | `ai-dimdim-dimdim566366` |
| Log Analytics | `law-dimdim-dimdim566366` |

> **Antes de gravar:** decida se quer mostrar a criação dos recursos **do zero** (mais fiel ao roteiro, mas exige excluir o Resource Group antes — `az group delete --name rg-dimdim-dimdim566366 --yes --no-wait` — e recriar tudo durante a gravação, o que pode levar ~5 min) ou mostrar os recursos **já criados** no portal e rodar os scripts só para demonstrar o conteúdo (mais rápido, já que os comandos são idempotentes e vão apenas confirmar que os recursos existem). Ambas são aceitáveis; a primeira é visualmente mais convincente.

---

## 1. Abertura (30s)

- Diga o nome do grupo, os integrantes e o objetivo do projeto.
- Mostre o diagrama em `docs/arquitetura.svg` (abra no navegador ou no VS Code com preview de SVG) e explique rapidamente: App Service → Azure SQL, App Service → Application Insights, Dev → deploy via Azure CLI.

## 2. Criação dos recursos em nuvem

```bash
export SQL_PASSWORD='<sua senha>'
bash scripts/02-criar-recursos.sh
```

Depois, no [portal do Azure](https://portal.azure.com), abra o Resource Group `rg-dimdim-dimdim566366` e mostre, um a um: SQL Server/Database, App Service, Application Insights, Log Analytics.

> Se a região configurada em `scripts/00-variaveis.sh` não estiver liberada pela sua assinatura, o Azure retorna `RequestDisallowedByAzure`; consulte as regiões permitidas com `az policy assignment list --query "[].parameters.listOfAllowedLocations.value"`. Nesta conta, `centralus` funcionou (brazilsouth e eastus2 foram bloqueados por política, e eastus estava sem capacidade para novos SQL Servers no momento do teste).

## 3. Banco de dados

```bash
bash scripts/03-aplicar-ddl.sh
```

No portal, abra o SQL Database `dimdim-db` → **Query editor** → faça login com o usuário `dimdimadmin` → rode:

```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo';
```

Mostre as tabelas `CLIENTE` e `TRANSACAO` criadas.

## 4. Deploy

```bash
bash scripts/04-deploy.sh
```

> No Windows sem `zip` instalado, publique manualmente: `dotnet publish src/DimDim.Web -c Release -o publish`, compacte garantindo que os caminhos internos do `.zip` usem `/` (o `Compress-Archive` do PowerShell grava `\`, o que quebra o `rsync` no Linux — construa o zip com `System.IO.Compression.ZipArchive` ou use o 7-Zip/WSL), e rode `az webapp deploy --resource-group rg-dimdim-dimdim566366 --name app-dimdim-dimdim566366 --src-path publish.zip --type zip`.

Abra `https://app-dimdim-dimdim566366.azurewebsites.net` no navegador (aba anônima, para provar que não depende de cache/login local) e mostre a home.

## 5. Testes com persistência (vale -3 se não mostrar)

Para **cada** entidade (Cliente e Transação), nessa ordem:

1. **Create**: crie um registro pela tela.
2. No Query Editor do portal: `SELECT * FROM dbo.CLIENTE;` (ou `TRANSACAO`) mostrando o registro novo.
3. **Update**: edite o mesmo registro pela tela.
4. `SELECT` de novo, mostrando o valor atualizado.
5. **Read/Details**: abra a tela de detalhes do registro.
6. **Delete**: exclua pela tela.
7. `SELECT` de novo, mostrando que o registro não existe mais.

Dica: crie primeiro um Cliente, depois uma Transação vinculada a ele, para também mostrar o relacionamento (nome do cliente aparecendo na listagem de transações).

## 6. Monitoramento

- Abra o Application Insights (`ai-dimdim-dimdim566366`) → **Live Metrics** **antes** de fazer as ações do passo 5 (ou repita alguma ação com o Live Metrics já aberto) para a telemetria aparecer em tempo real.
- Mostre **Transaction search** ou **Logs** com uma consulta simples:
  ```kusto
  requests | order by timestamp desc | take 20
  ```
  e também os logs customizados da aplicação:
  ```kusto
  traces | where message contains "Cliente" or message contains "Transa" | order by timestamp desc
  ```
- Mostre **Failures** e **Performance**.
- No Azure SQL Database, mostre **Query Performance Insight** ou as métricas de DTU/conexões.

> Dados históricos no Application Insights podem levar alguns minutos para aparecer; o Live Metrics é instantâneo.

## 7. Encerramento

Mostre o repositório no GitHub e percorra rapidamente o `README.md`.

---

**Checklist rápido antes de exportar o vídeo:** 1080p ✓ · narrado ✓ · todas as 7 etapas acima ✓ · app sempre em `azurewebsites.net`, nunca em `localhost` ✓.
