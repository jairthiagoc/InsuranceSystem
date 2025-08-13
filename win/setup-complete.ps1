# Script de Setup Completo - Sistema de Seguros
Write-Host "🏢 Setup Completo - Sistema de Seguros" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# Verificar se o Docker está rodando
Write-Host "🔍 Verificando Docker..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version
    Write-Host "✅ Docker encontrado: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker não encontrado!" -ForegroundColor Red
    Write-Host "📥 Instale o Docker Desktop em: https://www.docker.com/products/docker-desktop/" -ForegroundColor Yellow
    exit 1
}

# Verificar se o Docker Compose está disponível
Write-Host "🔍 Verificando Docker Compose..." -ForegroundColor Yellow
try {
    $composeVersion = docker-compose --version
    Write-Host "✅ Docker Compose encontrado: $composeVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker Compose não encontrado!" -ForegroundColor Red
    exit 1
}

# Verificar se o Node.js está instalado (para desenvolvimento)
Write-Host "🔍 Verificando Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js encontrado: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Node.js não encontrado (opcional para desenvolvimento)" -ForegroundColor Yellow
}

# Parar containers existentes
Write-Host "🛑 Parando containers existentes..." -ForegroundColor Yellow
docker-compose down -v

# Build e executar todos os serviços
Write-Host "🚀 Iniciando todos os serviços..." -ForegroundColor Yellow
docker-compose up -d --build

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Serviços iniciados com sucesso!" -ForegroundColor Green
} else {
    Write-Host "❌ Erro ao iniciar serviços" -ForegroundColor Red
    exit 1
}

# Aguardar serviços ficarem prontos
Write-Host "⏳ Aguardando serviços ficarem prontos..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Verificar status dos serviços
Write-Host "🔍 Verificando status dos serviços..." -ForegroundColor Yellow
docker-compose ps

# Testar APIs
Write-Host "🧪 Testando APIs..." -ForegroundColor Yellow

# Testar ProposalService
try {
    $response = Invoke-WebRequest -Uri "http://localhost:7001/api/proposals" -Method GET -TimeoutSec 10
    Write-Host "✅ ProposalService está respondendo" -ForegroundColor Green
} catch {
    Write-Host "⚠️  ProposalService não está respondendo ainda" -ForegroundColor Yellow
}

# Testar ContractService
try {
    $response = Invoke-WebRequest -Uri "http://localhost:7002/api/contracts" -Method GET -TimeoutSec 10
    Write-Host "✅ ContractService está respondendo" -ForegroundColor Green
} catch {
    Write-Host "⚠️  ContractService não está respondendo ainda" -ForegroundColor Yellow
}

# Setup do Frontend (se Node.js estiver disponível)
try {
    $nodeVersion = node --version
    Write-Host "🎨 Configurando Frontend..." -ForegroundColor Yellow
    
    # Verificar se o diretório frontend existe
    if (Test-Path "frontend") {
        Set-Location frontend
        
        # Instalar dependências
        Write-Host "📦 Instalando dependências do frontend..." -ForegroundColor Yellow
        npm install
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Dependências do frontend instaladas!" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Erro ao instalar dependências do frontend" -ForegroundColor Yellow
        }
        
        Set-Location ..
    } else {
        Write-Host "⚠️  Diretório frontend não encontrado" -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠️  Node.js não disponível para setup do frontend" -ForegroundColor Yellow
}

Write-Host "`n🎉 Setup completo finalizado!" -ForegroundColor Green
Write-Host "`n🌐 URLs dos Serviços:" -ForegroundColor Cyan
Write-Host "   Frontend: http://localhost:3000" -ForegroundColor White
Write-Host "   ProposalService: http://localhost:7001" -ForegroundColor White
Write-Host "   ContractService: http://localhost:7002" -ForegroundColor White
Write-Host "   RabbitMQ UI: http://localhost:15672" -ForegroundColor White
Write-Host "   SQL Server: localhost:1433" -ForegroundColor White
Write-Host "   PostgreSQL: localhost:5432" -ForegroundColor White
Write-Host "   Redis: localhost:6379" -ForegroundColor White

Write-Host "`n🔧 Comandos Úteis:" -ForegroundColor Cyan
Write-Host "   Ver logs: docker-compose logs -f" -ForegroundColor White
Write-Host "   Parar serviços: docker-compose down" -ForegroundColor White
Write-Host "   Reiniciar: docker-compose restart" -ForegroundColor White
Write-Host "   Status: docker-compose ps" -ForegroundColor White

Write-Host "`n📚 Documentação:" -ForegroundColor Cyan
Write-Host "   README.md - Documentação principal" -ForegroundColor White
Write-Host "   docs/ - Documentação detalhada" -ForegroundColor White
Write-Host "   frontend/README.md - Documentação do frontend" -ForegroundColor White

Write-Host "`n🚀 Para desenvolvimento local do frontend:" -ForegroundColor Cyan
Write-Host "   cd frontend" -ForegroundColor White
Write-Host "   npm start" -ForegroundColor White

Write-Host "`n✅ Sistema pronto para uso!" -ForegroundColor Green 