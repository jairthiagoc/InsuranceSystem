# Sistema de Seguros - Parando Servicos PowerShell
# Executar como: .\stop-system.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Sistema de Seguros - Parando Servicos" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

try {
    # 1. Parar serviços .NET
    Write-Host "[1/3] Parando serviços .NET..." -ForegroundColor Green
    
    $dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
    if ($dotnetProcesses) {
        $dotnetProcesses | Stop-Process -Force
        Write-Host "Serviços .NET parados." -ForegroundColor Green
    } else {
        Write-Host "Nenhum serviço .NET encontrado." -ForegroundColor Yellow
    }
    
    # 2. Parar containers Docker
    Write-Host "[2/3] Parando containers Docker..." -ForegroundColor Green
    docker-compose down
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Containers parados." -ForegroundColor Green
    } else {
        Write-Host "Erro ao parar containers." -ForegroundColor Red
    }
    
    # 3. Limpar processos específicos
    Write-Host "[3/3] Limpando processos..." -ForegroundColor Green
    
    $processesToKill = @("ProposalService", "ContractService")
    foreach ($processName in $processesToKill) {
        $processes = Get-Process -Name $processName -ErrorAction SilentlyContinue
        if ($processes) {
            $processes | Stop-Process -Force
            Write-Host "Processo $processName parado." -ForegroundColor Green
        }
    }
    
    # 4. Limpar arquivos temporários
    $tempFiles = @(
        "src\ProposalService\ProposalService.pid",
        "src\ContractService\ContractService.pid",
        "src\ProposalService\ProposalService.log",
        "src\ContractService\ContractService.log"
    )
    
    foreach ($file in $tempFiles) {
        if (Test-Path $file) {
            Remove-Item $file -Force
            Write-Host "Arquivo temporário removido: $file" -ForegroundColor Yellow
        }
    }
    
    Write-Host ""
    Write-Host "Sistema parado com sucesso!" -ForegroundColor Green
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Host "ERRO: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "Pressione qualquer tecla para sair..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 