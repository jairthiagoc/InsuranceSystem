# 🐳 Configuração Docker - Sistema de Seguros

## 📋 Visão Geral

Este arquivo descreve a configuração Docker para o sistema de seguros, incluindo todos os serviços necessários: APIs, bancos de dados, Redis e RabbitMQ.

## 🚀 Serviços Configurados

### 📊 Bancos de Dados
- **SQL Server**: Para ProposalService
- **PostgreSQL**: Para ContractService

### 🎯 APIs
- **ProposalService**: Porta 7001
- **ContractService**: Porta 7002

### 🗄️ Cache e Mensageria
- **Redis**: Cache distribuído (Porta 6379)
- **RabbitMQ**: Mensageria assíncrona (Porta 5672 + UI 15672)

### 🌐 Rede
- **insurance-system-network**: Rede bridge dedicada para todos os serviços

## 🛠️ Como Executar

### 1. Iniciar Todos os Serviços
```bash
docker-compose up -d
```

### 2. Verificar Status dos Serviços
```bash
docker-compose ps
```

### 3. Visualizar Logs
```bash
# Todos os serviços
docker-compose logs -f

# Serviço específico
docker-compose logs -f proposalservice
docker-compose logs -f contractservice
docker-compose logs -f redis
docker-compose logs -f rabbitmq
```

### 4. Parar Todos os Serviços
```bash
docker-compose down
```

### 5. Parar e Remover Volumes
```bash
docker-compose down -v
```

## 🌐 Acessos

### APIs
- **ProposalService**: http://localhost:7001
- **ContractService**: http://localhost:7002

### RabbitMQ Management UI
- **URL**: http://localhost:15672
- **Usuário**: admin
- **Senha**: admin123

### Redis CLI
```bash
docker exec -it intd-inovation-redis-1 redis-cli
```

## ⚙️ Configurações de Ambiente

### ProposalService
```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=InsuranceProposals;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
  - Redis__ConnectionString=redis:6379
  - RabbitMQ__Host=rabbitmq
  - RabbitMQ__Username=admin
  - RabbitMQ__Password=admin123
  - RabbitMQ__VirtualHost=/
```

### ContractService
```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ConnectionStrings__DefaultConnection=Host=postgres;Database=insurance_contracts;Username=postgres;Password=postgres123;
  - ProposalService__BaseUrl=http://proposalservice
  - Redis__ConnectionString=redis:6379
  - RabbitMQ__Host=rabbitmq
  - RabbitMQ__Username=admin
  - RabbitMQ__Password=admin123
  - RabbitMQ__VirtualHost=/
```

## 🔍 Health Checks

Todos os serviços possuem health checks configurados:

- **PostgreSQL**: `pg_isready -U postgres`
- **Redis**: `redis-cli ping`
- **RabbitMQ**: `rabbitmq-diagnostics ping`

## 📁 Volumes

Os dados são persistidos nos seguintes volumes:
- `sqlserver_data`: Dados do SQL Server
- `postgres_data`: Dados do PostgreSQL
- `redis_data`: Dados do Redis (com AOF habilitado)
- `rabbitmq_data`: Dados do RabbitMQ

## 🌐 Rede

Todos os serviços estão conectados na rede `insurance-system-network`:
- **Tipo**: Bridge
- **Nome**: insurance-system-network
- **Isolamento**: Comunicação interna entre containers
- **DNS**: Resolução automática por nome do serviço

## 🔧 Comandos Úteis

### Verificar Conectividade
```bash
# Testar Redis
docker exec intd-inovation-redis-1 redis-cli ping

# Testar RabbitMQ
docker exec intd-inovation-rabbitmq-1 rabbitmq-diagnostics ping

# Testar PostgreSQL
docker exec intd-inovation-postgres-1 pg_isready -U postgres

# Verificar rede
docker network ls
docker network inspect insurance-system-network
```

### Backup e Restore
```bash
# Backup Redis
docker exec intd-inovation-redis-1 redis-cli BGSAVE

# Backup PostgreSQL
docker exec intd-inovation-postgres-1 pg_dump -U postgres insurance_contracts > backup.sql
```

### Limpeza
```bash
# Remover containers parados
docker container prune

# Remover volumes não utilizados
docker volume prune

# Limpeza completa
docker system prune -a
```

## 🚨 Troubleshooting

### Problemas Comuns

1. **Porta já em uso**
   ```bash
   # Verificar portas em uso
   netstat -tulpn | grep :7001
   netstat -tulpn | grep :7002
   netstat -tulpn | grep :6379
   netstat -tulpn | grep :5672
   ```

2. **Serviços não iniciam**
   ```bash
   # Verificar logs
   docker-compose logs [servico]
   
   # Reiniciar serviço
   docker-compose restart [servico]
   ```

3. **Problemas de conectividade**
   ```bash
   # Verificar rede Docker
   docker network ls
   docker network inspect insurance-system-network
   
   # Verificar conectividade entre containers
   docker exec intd-inovation-proposalservice-1 ping redis
   docker exec intd-inovation-proposalservice-1 ping rabbitmq
   ```

### Logs Importantes

- **Redis**: Verificar se AOF está funcionando
- **RabbitMQ**: Verificar se usuário admin foi criado
- **APIs**: Verificar se consegue conectar aos serviços

## 📚 Referências

- [Docker Compose](https://docs.docker.com/compose/)
- [Redis Docker](https://hub.docker.com/_/redis)
- [RabbitMQ Docker](https://hub.docker.com/_/rabbitmq)
- [SQL Server Docker](https://hub.docker.com/_/microsoft-mssql-server)
- [PostgreSQL Docker](https://hub.docker.com/_/postgres) 