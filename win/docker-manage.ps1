# Script para gerenciar containers Docker do Sistema de Seguros
param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("start", "stop", "restart", "logs", "status", "clean", "build")]
    [string]$Action = "status"
)

Write-Host "Docker - Sistema de Seguros" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

switch ($Action) {
    "start" {
        Write-Host "Iniciando todos os servicos..." -ForegroundColor Green
        docker-compose up -d
        
        Write-Host "Aguardando servicos ficarem prontos..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        
        Write-Host "Status dos servicos:" -ForegroundColor Yellow
        docker-compose ps
        
        Write-Host "`nURLs de acesso:" -ForegroundColor Cyan
        Write-Host "  ProposalService: http://localhost:7001" -ForegroundColor White
        Write-Host "  ContractService: http://localhost:7002" -ForegroundColor White
        Write-Host "  RabbitMQ UI: http://localhost:15672 (admin/admin123)" -ForegroundColor White
        Write-Host "  Redis: localhost:6379" -ForegroundColor White
    }
    
    "stop" {
        Write-Host "Parando todos os servicos..." -ForegroundColor Red
        docker-compose down
        Write-Host "Servicos parados com sucesso!" -ForegroundColor Green
    }
    
    "restart" {
        Write-Host "Reiniciando todos os servicos..." -ForegroundColor Yellow
        docker-compose down
        docker-compose up -d
        
        Write-Host "Aguardando servicos ficarem prontos..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        
        Write-Host "Status dos servicos:" -ForegroundColor Yellow
        docker-compose ps
    }
    
    "logs" {
        Write-Host "Exibindo logs dos servicos..." -ForegroundColor Yellow
        docker-compose logs -f
    }
    
    "status" {
        Write-Host "Status dos servicos:" -ForegroundColor Yellow
        docker-compose ps
        
        Write-Host "`nVerificando conectividade..." -ForegroundColor Yellow
        
        # Verificar Redis
        try {
            $redisResult = docker exec intdinovation-redis-1 redis-cli ping 2>$null
            if ($redisResult -eq "PONG") {
                Write-Host "  Redis: Conectado" -ForegroundColor Green
            } else {
                Write-Host "  Redis: Erro de conexao" -ForegroundColor Red
            }
        } catch {
            Write-Host "  Redis: Servico nao disponivel" -ForegroundColor Red
        }
        
        # Verificar RabbitMQ
        try {
            $rabbitResult = docker exec intdinovation-rabbitmq-1 rabbitmq-diagnostics ping 2>$null
            if ($rabbitResult -like "*Ping succeeded*") {
                Write-Host "  RabbitMQ: Conectado" -ForegroundColor Green
            } else {
                Write-Host "  RabbitMQ: Erro de conexao" -ForegroundColor Red
            }
        } catch {
            Write-Host "  RabbitMQ: Servico nao disponivel" -ForegroundColor Red
        }
        
        # Verificar PostgreSQL
        try {
            $postgresResult = docker exec intdinovation-postgres-1 pg_isready -U postgres 2>$null
            if ($postgresResult -like "*accepting connections*") {
                Write-Host "  PostgreSQL: Conectado" -ForegroundColor Green
            } else {
                Write-Host "  PostgreSQL: Erro de conexao" -ForegroundColor Red
            }
        } catch {
            Write-Host "  PostgreSQL: Servico nao disponivel" -ForegroundColor Red
        }
        
        Write-Host "`nURLs de acesso:" -ForegroundColor Cyan
        Write-Host "  ProposalService: http://localhost:7001" -ForegroundColor White
        Write-Host "  ContractService: http://localhost:7002" -ForegroundColor White
        Write-Host "  RabbitMQ UI: http://localhost:15672 (admin/admin123)" -ForegroundColor White
        Write-Host "  Redis: localhost:6379" -ForegroundColor White
    }
    
    "clean" {
        Write-Host "Limpeza completa do Docker..." -ForegroundColor Yellow
        Write-Host "ATENCAO: Isso ira remover todos os containers, volumes e imagens!" -ForegroundColor Red
        
        $confirmation = Read-Host "Deseja continuar? (s/N)"
        if ($confirmation -eq "s" -or $confirmation -eq "S") {
            docker-compose down -v
            docker system prune -a -f
            Write-Host "Limpeza concluida!" -ForegroundColor Green
        } else {
            Write-Host "Limpeza cancelada!" -ForegroundColor Yellow
        }
    }
    
    "build" {
        Write-Host "Reconstruindo imagens..." -ForegroundColor Yellow
        docker-compose build --no-cache
        
        Write-Host "Iniciando servicos com novas imagens..." -ForegroundColor Green
        docker-compose up -d
        
        Write-Host "Aguardando servicos ficarem prontos..." -ForegroundColor Yellow
        Start-Sleep -Seconds 15
        
        Write-Host "Status dos servicos:" -ForegroundColor Yellow
        docker-compose ps
    }
}

Write-Host "`nOperacao concluida!" -ForegroundColor Green 