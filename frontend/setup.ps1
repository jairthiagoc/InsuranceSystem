# Script de Setup do Frontend - Sistema de Seguros
Write-Host "🎨 Frontend Setup - Sistema de Seguros" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# Verificar se o Node.js está instalado
Write-Host "🔍 Verificando Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js encontrado: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Node.js não encontrado!" -ForegroundColor Red
    Write-Host "📥 Instale o Node.js em: https://nodejs.org/" -ForegroundColor Yellow
    exit 1
}

# Verificar se o npm está instalado
Write-Host "🔍 Verificando npm..." -ForegroundColor Yellow
try {
    $npmVersion = npm --version
    Write-Host "✅ npm encontrado: $npmVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ npm não encontrado!" -ForegroundColor Red
    exit 1
}

# Verificar se o backend está rodando
Write-Host "🔍 Verificando backend..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:7001/api/proposals" -Method GET -TimeoutSec 5
    Write-Host "✅ Backend está rodando na porta 7001" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Backend não está rodando na porta 7001" -ForegroundColor Yellow
    Write-Host "💡 Execute o backend primeiro: docker-compose up -d" -ForegroundColor Cyan
}

# Instalar dependências
Write-Host "📦 Instalando dependências..." -ForegroundColor Yellow
npm install

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Dependências instaladas com sucesso!" -ForegroundColor Green
} else {
    Write-Host "❌ Erro ao instalar dependências" -ForegroundColor Red
    exit 1
}

# Verificar se o build funciona
Write-Host "🔨 Testando build..." -ForegroundColor Yellow
npm run build

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build realizado com sucesso!" -ForegroundColor Green
} else {
    Write-Host "❌ Erro no build" -ForegroundColor Red
    exit 1
}

Write-Host "`n🎉 Setup concluído com sucesso!" -ForegroundColor Green
Write-Host "`n🚀 Para executar o frontend:" -ForegroundColor Cyan
Write-Host "   npm start" -ForegroundColor White
Write-Host "`n📱 O frontend estará disponível em: http://localhost:3000" -ForegroundColor Cyan
Write-Host "`n🔗 URLs dos serviços:" -ForegroundColor Cyan
Write-Host "   Frontend: http://localhost:3000" -ForegroundColor White
Write-Host "   ProposalService: http://localhost:7001" -ForegroundColor White
Write-Host "   ContractService: http://localhost:7002" -ForegroundColor White
Write-Host "   RabbitMQ UI: http://localhost:15672" -ForegroundColor White 