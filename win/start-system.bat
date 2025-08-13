@echo off
echo ========================================
echo Sistema de Seguros - Inicializacao
echo ========================================
echo.

echo [1/5] Verificando Docker Desktop...
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERRO: Docker nao encontrado. Instale o Docker Desktop.
    pause
    exit /b 1
)

echo [2/5] Iniciando containers Docker...
docker-compose up -d
if %errorlevel% neq 0 (
    echo ERRO: Falha ao iniciar containers Docker.
    echo Verifique se o Docker Desktop esta rodando.
    pause
    exit /b 1
)

echo [3/5] Aguardando bancos ficarem prontos...
timeout /t 10 /nobreak >nul

echo [4/5] Executando migrations...
echo - ProposalService (SQL Server)...
cd src\ProposalService\ProposalService.Infrastructure
dotnet ef database update --no-build
if %errorlevel% neq 0 (
    echo ERRO: Falha na migration do ProposalService
    pause
    exit /b 1
)

echo - ContractService (PostgreSQL)...
cd ..\..\..\ContractService\ContractService.Infrastructure
dotnet ef database update --no-build
if %errorlevel% neq 0 (
    echo ERRO: Falha na migration do ContractService
    pause
    exit /b 1
)

cd ..\..\..

echo [5/5] Iniciando servicos...
echo.
echo ========================================
echo Servicos sendo iniciados...
echo ========================================
echo.
echo ProposalService: https://localhost:7001/swagger
echo ContractService: https://localhost:7002/swagger
echo.
echo Pressione Ctrl+C para parar os servicos
echo.

start "ProposalService" cmd /k "cd src\ProposalService\ProposalService.API && dotnet run"
timeout /t 3 /nobreak >nul
start "ContractService" cmd /k "cd src\ContractService\ContractService.API && dotnet run"

echo.
echo Sistema iniciado com sucesso!
echo Aguarde alguns segundos para os servicos ficarem prontos.
echo.
pause 