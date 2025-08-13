# Script para gerar relatório de cobertura de código
Write-Host "🔍 Iniciando análise de cobertura de código..." -ForegroundColor Green

# Executar testes com cobertura
Write-Host "📊 Executando testes com cobertura..." -ForegroundColor Yellow
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults

# Verificar se o ReportGenerator está instalado
Write-Host "📋 Verificando ReportGenerator..." -ForegroundColor Yellow
$reportGenerator = Get-Command reportgenerator -ErrorAction SilentlyContinue

if (-not $reportGenerator) {
    Write-Host "📦 Instalando ReportGenerator..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

# Gerar relatório HTML
Write-Host "📈 Gerando relatório HTML..." -ForegroundColor Yellow
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html

# Verificar se o relatório foi gerado
if (Test-Path "coverage-report/index.html") {
    Write-Host "✅ Relatório de cobertura gerado com sucesso!" -ForegroundColor Green
    Write-Host "📁 Localização: coverage-report/index.html" -ForegroundColor Cyan
    
    # Abrir o relatório no navegador
    Write-Host "🌐 Abrindo relatório no navegador..." -ForegroundColor Yellow
    Start-Process "coverage-report/index.html"
} else {
    Write-Host "❌ Erro ao gerar relatório de cobertura" -ForegroundColor Red
}

Write-Host "🎉 Análise de cobertura concluída!" -ForegroundColor Green 