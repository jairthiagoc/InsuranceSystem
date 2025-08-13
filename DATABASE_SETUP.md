# Configuração dos Bancos de Dados

## Visão Geral

O sistema utiliza **dois bancos de dados diferentes**:

- **ProposalService**: SQL Server
- **ContractService**: PostgreSQL

## Configuração Local

### 1. SQL Server (ProposalService)

#### Instalação Local
1. Baixe e instale SQL Server Developer/Express
2. Crie o banco de dados:
```sql
CREATE DATABASE InsuranceProposals;
```

#### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InsuranceProposals;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 2. PostgreSQL (ContractService)

#### Instalação Local
1. Baixe e instale PostgreSQL
2. Crie o banco de dados:
```sql
CREATE DATABASE insurance_contracts;
```

#### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=insurance_contracts;Username=postgres;Password=postgres123"
  }
}
```

## Configuração Docker

### 1. SQL Server Container
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. PostgreSQL Container
```bash
docker run -e POSTGRES_DB=insurance_contracts \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres123 \
  -p 5432:5432 --name postgres \
  -d postgres:15
```

### 3. Docker Compose (Recomendado)
```bash
docker-compose up -d
```

## Migrations

### ProposalService (SQL Server)
```bash
cd src/ProposalService/ProposalService.Infrastructure
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### ContractService (PostgreSQL)
```bash
cd src/ContractService/ContractService.Infrastructure
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Diferenças entre os Bancos

### SQL Server (ProposalService)
- **Provider**: `Microsoft.EntityFrameworkCore.SqlServer`
- **Tipo de dados**: `decimal(18,2)`
- **Nomenclatura**: PascalCase
- **Porta**: 1433

### PostgreSQL (ContractService)
- **Provider**: `Npgsql.EntityFrameworkCore.PostgreSQL`
- **Tipo de dados**: `numeric(18,2)`
- **Nomenclatura**: snake_case
- **Porta**: 5432

## Ferramentas de Administração

### SQL Server
- **SQL Server Management Studio (SSMS)**
- **Azure Data Studio**
- **Visual Studio**

### PostgreSQL
- **pgAdmin**
- **DBeaver**
- **DataGrip**

## Troubleshooting

### Problemas Comuns

#### SQL Server
```bash
# Verificar se está rodando
sqlcmd -S localhost -U sa -P YourStrong@Passw0rd

# Testar conexão
sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT @@VERSION"
```

#### PostgreSQL
```bash
# Verificar se está rodando
psql -h localhost -U postgres -d insurance_contracts

# Testar conexão
psql -h localhost -U postgres -d insurance_contracts -c "SELECT version();"
```

### Erros de Conexão

#### SQL Server
- Verificar se o SQL Server está rodando
- Confirmar a porta 1433
- Verificar autenticação Windows/SQL Server

#### PostgreSQL
- Verificar se o PostgreSQL está rodando
- Confirmar a porta 5432
- Verificar usuário e senha

### Erros de Migration

#### SQL Server
```bash
# Limpar migrations
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### PostgreSQL
```bash
# Limpar migrations
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Monitoramento

### Health Checks
- **ProposalService**: https://localhost:7001/health
- **ContractService**: https://localhost:7002/health

### Logs
```bash
# SQL Server logs
docker logs sqlserver

# PostgreSQL logs
docker logs postgres
```

## Backup e Restore

### SQL Server
```bash
# Backup
sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "BACKUP DATABASE InsuranceProposals TO DISK = 'backup.bak'"

# Restore
sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "RESTORE DATABASE InsuranceProposals FROM DISK = 'backup.bak'"
```

### PostgreSQL
```bash
# Backup
pg_dump -h localhost -U postgres insurance_contracts > backup.sql

# Restore
psql -h localhost -U postgres insurance_contracts < backup.sql
```

## Performance

### SQL Server
- Índices otimizados
- Query optimization
- Connection pooling

### PostgreSQL
- Índices B-tree
- Query planning
- Connection pooling

## Segurança

### SQL Server
- Autenticação Windows/SQL Server
- Criptografia de conexão
- Firewall rules

### PostgreSQL
- Autenticação por senha
- SSL/TLS
- pg_hba.conf configuration 