# 🎭 Bogus - Geração de Dados Fake para Testes

## 📋 Visão Geral

Este diretório contém o `FakeDataGenerator` que utiliza a biblioteca [Bogus](https://github.com/bchavez/Bogus) para gerar dados fake realistas nos testes unitários.

## 🚀 Como Usar

### Importar o Helper

```csharp
using ProposalService.Tests.Helpers;
```

### Gerar Dados Básicos

```csharp
// Gerar uma proposta fake
var proposal = FakeDataGenerator.GenerateProposal();

// Gerar múltiplas propostas
var proposals = FakeDataGenerator.GenerateProposals(10);

// Gerar DTOs
var createDto = FakeDataGenerator.GenerateCreateProposalDto();
var updateDto = FakeDataGenerator.GenerateUpdateProposalStatusDto();
```

### Gerar Dados Específicos para Cenários

```csharp
// Proposta aprovada
var approvedProposal = FakeDataGenerator.GenerateApprovedProposal();

// Proposta rejeitada
var rejectedProposal = FakeDataGenerator.GenerateRejectedProposal("Motivo da rejeição");

// Proposta em revisão
var underReviewProposal = FakeDataGenerator.GenerateUnderReviewProposal();

// DTOs específicos
var approvalDto = FakeDataGenerator.GenerateApprovalDto();
var rejectionDto = FakeDataGenerator.GenerateRejectionDto();
```

## 🎯 Tipos de Dados Gerados

### Proposal
- **CustomerName**: Nome completo em português
- **CustomerEmail**: Email válido
- **InsuranceType**: Tipos de seguro (Auto, Home, Life, Health, Travel)
- **CoverageAmount**: Valor entre R$ 1.000 e R$ 100.000
- **PremiumAmount**: Valor entre R$ 100 e R$ 10.000
- **Status**: Status inicial (Draft)

### CreateProposalDto
- Mesmos campos da Proposal
- Gerado com dados realistas

### UpdateProposalStatusDto
- **Status**: Status aleatório (Draft, UnderReview, Approved, Rejected)
- **RejectionReason**: Motivo de rejeição (apenas se Status = Rejected)

## 🔧 Configuração

### Locale
O Bogus está configurado para usar `pt_BR` (português brasileiro) para gerar dados mais realistas para o contexto brasileiro.

### Seed
Para testes determinísticos, você pode definir um seed:

```csharp
// No início do teste
Randomizer.Seed = new Random(12345);
```

## 📝 Exemplos de Uso nos Testes

### Teste de Use Case

```csharp
[Fact]
public async Task ExecuteAsync_WithValidData_ShouldCreateProposal()
{
    // Arrange
    var dto = FakeDataGenerator.GenerateCreateProposalDto();
    
    // ... resto do teste
}
```

### Teste de Repository

```csharp
[Fact]
public async Task AddAsync_ShouldAddProposalAndInvalidateCache()
{
    // Arrange
    var newProposal = FakeDataGenerator.GenerateProposal();
    
    // ... resto do teste
}
```

### Teste com Cenários Específicos

```csharp
[Fact]
public async Task UpdateStatus_WithApproval_ShouldApproveProposal()
{
    // Arrange
    var proposal = FakeDataGenerator.GenerateUnderReviewProposal();
    var dto = FakeDataGenerator.GenerateApprovalDto();
    
    // ... resto do teste
}
```

## 🎨 Vantagens do Bogus

1. **Dados Realistas**: Gera dados que parecem reais
2. **Consistência**: Mesmo seed gera os mesmos dados
3. **Flexibilidade**: Fácil de customizar para cenários específicos
4. **Manutenibilidade**: Centraliza a geração de dados fake
5. **Performance**: Geração rápida de dados

## 🔄 Atualizações

Para adicionar novos tipos de dados fake:

1. Adicione o novo Faker no `FakeDataGenerator`
2. Crie métodos utilitários se necessário
3. Atualize esta documentação

## 📚 Referências

- [Bogus GitHub](https://github.com/bchavez/Bogus)
- [Bogus Documentation](https://github.com/bchavez/Bogus#bogus)
- [Faker.js (inspiração)](https://github.com/faker-js/faker) 