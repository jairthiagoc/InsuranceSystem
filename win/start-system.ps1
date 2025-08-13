# Sistema de Seguros - Inicializacao PowerShell
# Executar como: .\start-system.ps1

param(
    [switch]$SkipDocker,
    [switch]$SkipMigrations,
    [switch]$SkipServices
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Sistema de Seguros - Inicializacao" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Função para verificar se comando existe
function Test-Command($cmdname) {
    return [bool](Get-Command -Name $cmdname -ErrorAction SilentlyContinue)
}

# Função para aguardar com spinner
function Wait-WithSpinner($seconds, $message) {
    $spinner = @('⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏')
    $i = 0
    $endTime = (Get-Date).AddSeconds($seconds)
    
    while ((Get-Date) -lt $endTime) {
        Write-Host "`r$($spinner[$i % $spinner.Length]) $message" -NoNewline -ForegroundColor Yellow
        Start-Sleep -Milliseconds 100
        $i++
    }
    Write-Host ""
}

try {
    # 1. Verificar Docker
    if (-not $SkipDocker) {
        Write-Host "[1/5] Verificando Docker Desktop..." -ForegroundColor Green
        
        if (-not (Test-Command "docker")) {
            throw "Docker não encontrado. Instale o Docker Desktop."
        }
        
        # Verificar se Docker está rodando
        try {
            docker version | Out-Null
        }
        catch {
            throw "Docker Desktop não está rodando. Inicie o Docker Desktop."
        }
        
        # 2. Iniciar containers
        Write-Host "[2/5] Iniciando containers Docker..." -ForegroundColor Green
        docker-compose up -d
        if ($LASTEXITCODE -ne 0) {
            throw "Falha ao iniciar containers Docker."
        }
        
        # 3. Aguardar bancos ficarem prontos
        Write-Host "[3/5] Aguardando bancos ficarem prontos..." -ForegroundColor Green
        Wait-WithSpinner 15 "Aguardando SQL Server e PostgreSQL..."
    }
    
    # 4. Executar migrations
    if (-not $SkipMigrations) {
        Write-Host "[4/5] Executando migrations..." -ForegroundColor Green
        
        # ProposalService
        Write-Host "  - ProposalService (SQL Server)..." -ForegroundColor Yellow
        Push-Location "src\ProposalService\ProposalService.Infrastructure"
        dotnet ef database update --no-build
        if ($LASTEXITCODE -ne 0) {
            throw "Falha na migration do ProposalService"
        }
        Pop-Location
        
        # ContractService
        Write-Host "  - ContractService (PostgreSQL)..." -ForegroundColor Yellow
        Push-Location "src\ContractService\ContractService.Infrastructure"
        dotnet ef database update --no-build
        if ($LASTEXITCODE -ne 0) {
            throw "Falha na migration do ContractService"
        }
        Pop-Location
    }
    
    # 5. Iniciar serviços
    if (-not $SkipServices) {
        Write-Host "[5/5] Iniciando serviços..." -ForegroundColor Green
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "Serviços sendo iniciados..." -ForegroundColor Cyan
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "ProposalService: https://localhost:7001/swagger" -ForegroundColor Green
        Write-Host "ContractService: https://localhost:7002/swagger" -ForegroundColor Green
        Write-Host ""
        Write-Host "Pressione Ctrl+C para parar os serviços" -ForegroundColor Yellow
        Write-Host ""
        
        # Iniciar ProposalService
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\src\ProposalService\ProposalService.API'; dotnet run" -WindowStyle Normal
        
        Start-Sleep -Seconds 3
        
        # Iniciar ContractService
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\src\ContractService\ContractService.API'; dotnet run" -WindowStyle Normal
    }
    
    Write-Host ""
    Write-Host "Sistema iniciado com sucesso!" -ForegroundColor Green
    Write-Host "Aguarde alguns segundos para os serviços ficarem prontos." -ForegroundColor Yellow
    Write-Host ""
    
    # Mostrar status dos containers
    Write-Host "Status dos containers:" -ForegroundColor Cyan
    docker-compose ps
    
}
catch {
    Write-Host ""
    Write-Host "ERRO: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Para ajuda, execute: .\start-system.ps1 -Help" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Pressione qualquer tecla para sair..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 