# Scripts de Inicialização - Sistema de Seguros

## Visão Geral

Scripts automatizados para inicializar e parar o sistema de seguros completo, incluindo:
- Inicialização do Docker Desktop
- Criação dos containers de banco de dados
- Execução das migrations
- Inicialização dos microserviços

## Scripts Disponíveis

### Windows

#### `start-system.bat`
Script batch simples para Windows.
```cmd
start-system.bat
```

#### `start-system.ps1` (Recomendado)
Script PowerShell com recursos avançados.
```powershell
# Execução normal
.\start-system.ps1

# Pular Docker (se já estiver rodando)
.\start-system.ps1 -SkipDocker

# Pular migrations (se já executadas)
.\start-system.ps1 -SkipMigrations

# Pular serviços (só bancos)
.\start-system.ps1 -SkipServices
```

#### `stop-system.bat`
Para parar todos os serviços.
```cmd
stop-system.bat
```

#### `stop-system.ps1`
Script PowerShell para parar serviços.
```powershell
.\stop-system.ps1
```

### Linux/macOS

#### `start-system.sh`
Script bash para Linux/macOS.
```bash
# Dar permissão de execução (primeira vez)
chmod +x start-system.sh

# Executar
./start-system.sh
```

#### `stop-system.sh`
Para parar todos os serviços.
```bash
chmod +x stop-system.sh
./stop-system.sh
```

## O que os Scripts Fazem

### Scripts de Inicialização

1. **Verificação do Docker**
   - Verifica se Docker está instalado
   - Verifica se Docker Desktop está rodando

2. **Inicialização dos Containers**
   - Executa `docker-compose up -d`
   - Inicia SQL Server e PostgreSQL

3. **Aguardar Bancos Prontos**
   - Aguarda 10-15 segundos para bancos inicializarem
   - Spinner visual durante aguardo

4. **Executar Migrations**
   - ProposalService (SQL Server)
   - ContractService (PostgreSQL)

5. **Iniciar Serviços**
   - ProposalService na porta 7001
   - ContractService na porta 7002
   - Abre janelas separadas para cada serviço

### Scripts de Parada

1. **Parar Serviços .NET**
   - Mata todos os processos dotnet
   - Para serviços específicos

2. **Parar Containers**
   - Executa `docker-compose down`
   - Remove containers e redes

3. **Limpeza**
   - Remove arquivos temporários (.pid, .log)
   - Limpa processos órfãos

## URLs dos Serviços

Após inicialização bem-sucedida:

- **ProposalService Swagger**: https://localhost:7001/swagger
- **ContractService Swagger**: https://localhost:7002/swagger
- **ProposalService Health**: https://localhost:7001/health
- **ContractService Health**: https://localhost:7002/health

## Troubleshooting

### Problemas Comuns

#### Docker não está rodando
```bash
# Windows: Iniciar Docker Desktop manualmente
# Linux: sudo systemctl start docker
# macOS: Abrir Docker Desktop
```

#### Portas em uso
```bash
# Verificar portas
netstat -ano | findstr :7001
netstat -ano | findstr :7002

# Matar processos nas portas
taskkill /PID <PID> /F
```

#### Erro de migrations
```bash
# Limpar e recriar migrations
cd src/ProposalService/ProposalService.Infrastructure
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Certificado HTTPS
```bash
# Confiar no certificado de desenvolvimento
dotnet dev-certs https --trust
```

### Logs

#### Windows
- Logs aparecem nas janelas dos serviços
- Arquivos temporários em `src/ProposalService/` e `src/ContractService/`

#### Linux/macOS
```bash
# Ver logs em tempo real
tail -f src/ProposalService/ProposalService.log
tail -f src/ContractService/ContractService.log

# Logs do Docker
docker-compose logs -f
```

## Comandos Manuais

Se preferir executar manualmente:

```bash
# 1. Iniciar containers
docker-compose up -d

# 2. Executar migrations
cd src/ProposalService/ProposalService.Infrastructure
dotnet ef database update

cd ../../ContractService/ContractService.Infrastructure
dotnet ef database update

# 3. Iniciar serviços
cd ../../ProposalService/ProposalService.API
dotnet run

# Em outro terminal:
cd src/ContractService/ContractService.API
dotnet run
```

## Configurações Avançadas

### PowerShell com Parâmetros

```powershell
# Só bancos, sem serviços
.\start-system.ps1 -SkipServices

# Só serviços, sem bancos
.\start-system.ps1 -SkipDocker -SkipMigrations

# Execução completa
.\start-system.ps1
```

### Bash com Variáveis de Ambiente

```bash
# Configurar timeouts
export DB_WAIT_TIME=20
export SERVICE_START_DELAY=5

# Executar
./start-system.sh
```

## Monitoramento

### Health Checks
```bash
# Verificar saúde dos serviços
curl https://localhost:7001/health
curl https://localhost:7002/health
```

### Status dos Containers
```bash
# Ver containers rodando
docker-compose ps

# Ver logs dos containers
docker-compose logs
```
