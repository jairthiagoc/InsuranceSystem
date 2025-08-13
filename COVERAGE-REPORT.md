# 📊 Relatório de Cobertura de Código

## 🎯 Objetivo
Este documento descreve como gerar e interpretar o relatório de cobertura de código do projeto.

## 🚀 Como Gerar o Relatório

### Opção 1: Script Automatizado (Recomendado)
```powershell
.\coverage-analysis.ps1
```

### Opção 2: Comandos Manuais
```bash
# 1. Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults

# 2. Gerar relatório HTML
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

## 📁 Estrutura do Relatório

### Pasta `coverage-report/`
- **`index.html`** - Página principal do relatório
- **`*.html`** - Relatórios detalhados por classe
- **`*.css`** - Estilos do relatório
- **`*.js`** - Scripts de interação

## 📈 Interpretando o Relatório

### Cores e Indicadores
- 🟢 **Verde** - 100% de cobertura
- 🟡 **Amarelo** - 80-99% de cobertura
- 🔴 **Vermelho** - <80% de cobertura

### Métricas Importantes
- **Line Coverage** - Porcentagem de linhas executadas
- **Branch Coverage** - Porcentagem de branches testados
- **Method Coverage** - Porcentagem de métodos testados

## 🎯 Classes com 100% de Cobertura

### ✅ ProposalService
- `ProposalsController` - 100% (38.4% → 100%)
- `ProposalRepository` - 100% (0% → 100%)
- `UpdateProposalStatusDto` - 100% (0% → 100%)

### ✅ Shared
- `ResilientHttpClientBuilderExtensions` - 100% (0% → 100%)

## 🔧 Configuração

### Arquivo `coverlet.runsettings`
```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage">
        <Configuration>
          <Format>cobertura</Format>
          <ExcludeByAttribute>ExcludeFromCodeCoverage</ExcludeByAttribute>
          <ExcludeByFile>**/Program.cs;**/*ServiceCollectionExtensions.cs;**/*DbContext.cs</ExcludeByFile>
          <Include>**/*Repository.cs;**/*Controller.cs;**/*UseCase.cs;**/*Dto.cs;**/*Extensions.cs</Include>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

## 📊 Estatísticas Atuais

- **Total de Testes**: 142
- **Taxa de Sucesso**: 100%
- **Cobertura Geral**: 76.3%
- **Classes Testadas**: 26


## 🔗 Links Úteis

- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator](https://github.com/danielpalme/ReportGenerator)
- [Cobertura de Código - Boas Práticas](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

---

**📅 Última Atualização**: $(Get-Date -Format "dd/MM/yyyy HH:mm")
**🔧 Versão**: 1.0.0 