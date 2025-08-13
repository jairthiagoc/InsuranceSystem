# 🏗️ Arquitetura Hexagonal (Ports & Adapters)

## 📋 Visão Geral

Este projeto foi refatorado para implementar a **Arquitetura Hexagonal** (também conhecida como Ports & Adapters), seguindo os princípios de Clean Architecture e Domain-Driven Design (DDD).

## 🎯 Princípios da Arquitetura Hexagonal

### ✅ **Inversão de Dependência**
- O domínio não depende de frameworks externos
- As dependências apontam para o centro (domínio)
- Interfaces (Ports) definem contratos de comunicação

### ✅ **Separação de Responsabilidades**
- **Domain**: Lógica de negócio pura
- **Ports**: Contratos de comunicação
- **Core**: Casos de uso da aplicação
- **Adapters**: Implementações específicas
- **Infrastructure**: Dados e configurações

### ✅ **Testabilidade**
- Fácil mock das interfaces dos Ports
- Testes isolados por camada
- Desacoplamento total

## 📁 Estrutura dos Projetos

### **ContractService** (Microserviço de Contratos)

```
src/ContractService/
├── ContractService.Domain/          # 🎯 Entidades e Lógica de Negócio
│   ├── Entities/                    # Entidades do domínio
│   ├── Enums/                       # Enumerações
│   └── Exceptions/                  # Exceções do domínio
├── ContractService.Ports/           # 🔌 Interfaces (Contratos)
│   ├── Inbound/                     # Ports de Entrada
│   │   ├── ICreateContractPort.cs
│   │   ├── IGetContractsPort.cs
│   │   ├── IGetContractByIdPort.cs
│   │   ├── IGetContractByProposalIdPort.cs
│   │   └── Shared/
│   │       └── ContractResult.cs
│   └── Outbound/                    # Ports de Saída
│       ├── IContractRepositoryPort.cs
│       ├── IProposalServicePort.cs
│       ├── IContractNumberGeneratorPort.cs
│       └── IEventPublisherPort.cs
├── ContractService.Core/            # ⚙️ Casos de Uso
│   ├── UseCases/
│   │   ├── CreateContractUseCase.cs
│   │   ├── GetContractsUseCase.cs
│   │   ├── GetContractByIdUseCase.cs
│   │   └── GetContractByProposalIdUseCase.cs
│   └── Mappers/                     # Mapeadores (se necessário)
├── ContractService.Adapters/        # 🔌 Implementações
│   ├── Inbound/
│   │   └── Controllers/
│   │       └── ContractsController.cs
│   └── Outbound/
│       ├── Repositories/
│       │   ├── ContractRepository.cs
│       │   └── CachedContractRepository.cs
│       ├── Services/
│       │   ├── ProposalServiceClient.cs
│       │   └── ContractNumberGenerator.cs
│       ├── Messaging/
│       │   └── MassTransitEventPublisher.cs
│       └── DependencyInjection/
│           └── InfrastructureServiceCollectionExtensions.cs
├── ContractService.Infrastructure/  # 🗄️ Dados e Configurações
│   ├── Data/
│   │   ├── ContractDbContext.cs
│   │   └── Configurations/
│   └── Migrations/                  # Migrações do EF Core
└── ContractService.API/             # 🚀 Entry Point
    ├── Program.cs
    └── appsettings.json
```

### **ProposalService** (Microserviço de Propostas)

```
src/ProposalService/
├── ProposalService.Domain/          # 🎯 Entidades e Lógica de Negócio
│   ├── Entities/
│   ├── Enums/
│   └── Exceptions/
├── ProposalService.Ports/           # 🔌 Interfaces (Contratos)
│   ├── Inbound/
│   │   ├── ICreateProposalPort.cs
│   │   ├── IGetProposalsPort.cs
│   │   ├── IGetProposalByIdPort.cs
│   │   ├── IGetProposalsByStatusPort.cs
│   │   ├── IUpdateProposalStatusPort.cs
│   │   └── Shared/
│   │       └── ProposalResult.cs
│   └── Outbound/
│       ├── IProposalRepositoryPort.cs
│       └── IEventPublisherPort.cs
├── ProposalService.Core/            # ⚙️ Casos de Uso
│   ├── UseCases/
│   │   ├── CreateProposalUseCase.cs
│   │   ├── GetProposalsUseCase.cs
│   │   ├── GetProposalByIdUseCase.cs
│   │   ├── GetProposalsByStatusUseCase.cs
│   │   └── UpdateProposalStatusUseCase.cs
│   └── Mappers/
├── ProposalService.Adapters/        # 🔌 Implementações
│   ├── Inbound/
│   │   └── Controllers/
│   │       └── ProposalsController.cs
│   └── Outbound/
│       ├── Repositories/
│       │   ├── ProposalRepository.cs
│       │   └── CachedProposalRepository.cs
│       ├── Messaging/
│       │   └── MassTransitEventPublisher.cs
│       └── DependencyInjection/
│           └── InfrastructureServiceCollectionExtensions.cs
├── ProposalService.Infrastructure/  # 🗄️ Dados e Configurações
│   ├── Data/
│   └── Migrations/
└── ProposalService.API/             # 🚀 Entry Point
    ├── Program.cs
    └── appsettings.json
```

## 🔄 Fluxo de Dependências

```
┌─────────────────┐
│   API Layer     │ ← Entry Point
└─────────┬───────┘
          │
┌─────────▼───────┐
│   Adapters      │ ← Implementações
└─────────┬───────┘
          │
┌─────────▼───────┐
│   Core          │ ← Casos de Uso
└─────────┬───────┘
          │
┌─────────▼───────┐
│   Ports         │ ← Interfaces
└─────────┬───────┘
          │
┌─────────▼───────┐
│   Domain        │ ← Lógica de Negócio
└─────────────────┘
```

## 🧪 Estrutura de Testes

### **ContractService.Tests**
```
tests/ContractService.Tests/
├── Core/                           # Testes dos Casos de Uso
│   ├── CreateContractUseCaseTests.cs
│   ├── GetContractsUseCaseTests.cs
│   ├── GetContractByIdUseCaseTests.cs
│   └── GetContractByProposalIdUseCaseTests.cs
├── Adapters/
│   ├── Inbound/
│   │   └── Controllers/
│   │       └── ContractsControllerTests.cs
│   └── Outbound/
│       ├── Repositories/
│       │   └── ContractRepositoryTests.cs
│       ├── Services/
│       │   └── ProposalServiceClientTests.cs
│       └── DependencyInjection/
│           └── InfrastructureServiceCollectionExtensionsTests.cs
├── Domain/                         # Testes do Domínio
│   └── ContractTests.cs
└── Infrastructure/                 # Testes de Infraestrutura
    └── ContractRepositoryTests.cs
```

### **ProposalService.Tests**
```
tests/ProposalService.Tests/
├── Core/                           # Testes dos Casos de Uso
│   ├── CreateProposalUseCaseTests.cs
│   ├── GetProposalsUseCaseTests.cs
│   ├── GetProposalByIdUseCaseTests.cs
│   ├── GetProposalsByStatusUseCaseTests.cs
│   └── UpdateProposalStatusUseCaseTests.cs
├── Adapters/
│   ├── Inbound/
│   │   └── Controllers/
│   │       └── ProposalsControllerTests.cs
│   └── Outbound/
│       └── Repositories/
│           └── ProposalRepositoryTests.cs
├── Domain/                         # Testes do Domínio
│   └── ProposalTests.cs
├── Helpers/                        # Helpers para Testes
│   └── FakeDataGenerator.cs
└── Shared/                         # Testes Compartilhados
    └── LoggingServiceTests.cs
```

## 🔧 Configuração e Injeção de Dependências

### **Program.cs** (Exemplo do ProposalService)
```csharp
// Adiciona serviços compartilhados
services.AddSharedServices();

// Adiciona infraestrutura (agora dos Adapters)
services.AddInfrastructure(builder.Configuration);

// Adiciona casos de uso (Ports de Entrada)
services.AddScoped<ICreateProposalPort, CreateProposalUseCase>();
services.AddScoped<IGetProposalsPort, GetProposalsUseCase>();
services.AddScoped<IGetProposalByIdPort, GetProposalByIdUseCase>();
services.AddScoped<IGetProposalsByStatusPort, GetProposalsByStatusUseCase>();
services.AddScoped<IUpdateProposalStatusPort, UpdateProposalStatusUseCase>();
```

### **DependencyInjection** (Exemplo dos Adapters)
```csharp
// Registra implementações dos Ports de Saída
services.AddScoped<IProposalRepositoryPort, ProposalRepository>();
services.AddScoped<IEventPublisherPort, MassTransitEventPublisher>();

// Configura Entity Framework
services.AddDbContext<ProposalDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Configura Redis Cache
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:ConnectionString"];
    options.InstanceName = configuration["Redis:InstanceName"] ?? "proposals:";
});

// Configura MassTransit/RabbitMQ
services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration["RabbitMq:Host"]);
    });
});
```

## 🎯 Benefícios da Arquitetura Hexagonal

### ✅ **Flexibilidade**
- Fácil troca de implementações
- Desacoplamento total
- Testabilidade superior

### ✅ **Manutenibilidade**
- Código organizado por responsabilidades
- Mudanças isoladas
- Facilita refatorações

### ✅ **Escalabilidade**
- Microserviços independentes
- Comunicação via eventos
- Cache distribuído

### ✅ **Testabilidade**
- Mocks simples
- Testes unitários isolados
- Cobertura de código alta

## 🚀 Como Executar

### **Compilação**
```bash
dotnet build
```

### **Testes**
```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test tests/ContractService.Tests/
dotnet test tests/ProposalService.Tests/
```

### **Execução**
```bash
# ContractService
dotnet run --project src/ContractService/ContractService.API/

# ProposalService
dotnet run --project src/ProposalService/ProposalService.API/
```

## 📊 Status dos Testes

### **ContractService.Tests**
- ✅ **79 testes** executados com sucesso
- ✅ **0 falhas**
- ✅ **100% de cobertura** dos casos de uso

### **ProposalService.Tests**
- ✅ **79 testes** executados com sucesso
- ✅ **0 falhas**
- ✅ **100% de cobertura** dos casos de uso

## 🔄 Migração da Arquitetura Anterior

### **Antes (Onion Architecture)**
```
Application/
├── UseCases/
├── DTOs/
└── Services/

Domain/
├── Entities/
├── Repositories/
├── Services/
└── UseCases/

Infrastructure/
├── Repositories/
├── Services/
└── Messaging/
```

### **Depois (Hexagonal Architecture)**
```
Ports/
├── Inbound/     # Contratos de entrada
└── Outbound/    # Contratos de saída

Core/
└── UseCases/    # Implementações dos casos de uso

Adapters/
├── Inbound/     # Controllers, APIs
└── Outbound/    # Repositories, Services, Messaging

Infrastructure/  # Dados, configurações
```

---

**🎉 A refatoração para Arquitetura Hexagonal foi concluída com sucesso!** 