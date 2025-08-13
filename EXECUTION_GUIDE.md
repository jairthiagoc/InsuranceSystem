# Guia de Execução - Sistema de Seguros

## Pré-requisitos

- .NET 8 SDK
- Docker Desktop
- SQL Server (opcional, pode usar Docker)
- PostgreSQL (opcional, pode usar Docker)

## Execução Local

### 1. Build da Solução
```bash
dotnet build
```

### 2. Configuração do Banco de Dados

#### Opção A: Bancos Locais
1. **SQL Server** (ProposalService):
   - Instale SQL Server
   - Crie o banco: `InsuranceProposals`
   - Atualize a connection string em `src/ProposalService/ProposalService.API/appsettings.json`

2. **PostgreSQL** (ContractService):
   - Instale PostgreSQL
   - Crie o banco: `insurance_contracts`
   - Atualize a connection string em `src/ContractService/ContractService.API/appsettings.json`

#### Opção B: Bancos Docker
```bash
# SQL Server
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# PostgreSQL
docker run -e POSTGRES_DB=insurance_contracts -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres123 -p 5432:5432 --name postgres -d postgres:15
```

### 3. Executar Migrations
```bash
# ProposalService (SQL Server)
cd src/ProposalService/ProposalService.Infrastructure
dotnet ef migrations add InitialCreate
dotnet ef database update

# ContractService (PostgreSQL)
cd ../../ContractService/ContractService.Infrastructure
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Executar Serviços
```bash
# Terminal 1 - ProposalService
cd src/ProposalService/ProposalService.API
dotnet run

# Terminal 2 - ContractService
cd src/ContractService/ContractService.API
dotnet run
```

## Execução com Docker

### 1. Build e Execução
```bash
# Build das imagens
docker-compose build

# Executar todos os serviços
docker-compose up -d
```

### 2. Verificar Status
```bash
docker-compose ps
```

### 3. Logs
```bash
# Todos os serviços
docker-compose logs

# Serviço específico
docker-compose logs proposalservice
docker-compose logs contractservice
```

## Testes

### Executar Todos os Testes
```bash
dotnet test
```

### Executar Testes Específicos
```bash
# ProposalService
dotnet test tests/ProposalService.Tests/

# ContractService
dotnet test tests/ContractService.Tests/
```

## APIs

### ProposalService (Port 7001)
- **Swagger**: https://localhost:7001/swagger
- **Health Check**: https://localhost:7001/health

### ContractService (Port 7002)
- **Swagger**: https://localhost:7002/swagger
- **Health Check**: https://localhost:7002/health

## Exemplos de Uso

### 1. Criar Proposta
```bash
curl -X POST "https://localhost:7001/api/proposals" \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "João Silva",
    "customerEmail": "joao@email.com",
    "insuranceType": "Auto",
    "coverageAmount": 50000,
    "premiumAmount": 1200
  }'
```

### 2. Listar Propostas
```bash
curl -X GET "https://localhost:7001/api/proposals"
```

### 3. Atualizar Status da Proposta
```bash
curl -X PUT "https://localhost:7001/api/proposals/{proposalId}/status" \
  -H "Content-Type: application/json" \
  -d '{
    "status": 1
  }'
```

### 4. Criar Contrato
```bash
curl -X POST "https://localhost:7002/api/contracts" \
  -H "Content-Type: application/json" \
  -d '{
    "proposalId": "proposal-guid-here",
    "contractNumber": "CTR-2024-001",
    "premiumAmount": 1200
  }'
```

### 5. Listar Contratos
```bash
curl -X GET "https://localhost:7002/api/contracts"
```

## Troubleshooting

### Problemas Comuns

#### 1. Erro de Conexão com Banco
- Verifique se o SQL Server está rodando
- Confirme a connection string
- Teste a conexão: `sqlcmd -S localhost -U sa -P YourStrong@Passw0rd`

#### 2. Erro de Porta em Uso
- Verifique se as portas 7001 e 7002 estão livres
- Use `netstat -ano | findstr :7001` para verificar

#### 3. Erro de Certificado HTTPS
- Use `dotnet dev-certs https --trust` para confiar no certificado
- Ou acesse via HTTP: `http://localhost:5001`

#### 4. Erro de Migration
- Delete a pasta `Migrations` e recrie
- Verifique se o banco existe
- Confirme as permissões do usuário

### Logs Detalhados
```bash
# Habilitar logs detalhados
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS=true
export ASPNETCORE_LOGGING__CONSOLE__FORMAT=json
```

## Monitoramento

### Health Checks
- ProposalService: https://localhost:7001/health
- ContractService: https://localhost:7002/health

### Métricas (Futuro)
- Prometheus endpoints
- Grafana dashboards
- Application Insights


### Comandos Úteis
```bash
# Limpar build
dotnet clean

# Restaurar pacotes
dotnet restore

# Build em Release
dotnet build -c Release

# Publicar
dotnet publish -c Release

# Análise de código
dotnet format
dotnet analyze
```

## Desenvolvimentos Realizados

1. **Autenticação/Autorização**: Implementar JWT
2. **Validação**: Adicionar FluentValidation
3. **Logging**: Configurar Serilog
4. **Cache**: Implementar Redis
5. **Message Queue**: Adicionar RabbitMQ/Azure Service Bus
6. **Documentação**: Expandir documentação da API 