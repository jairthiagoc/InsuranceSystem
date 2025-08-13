@echo off
echo ========================================
echo Sistema de Seguros - Parando Servicos
echo ========================================
echo.

echo [1/3] Parando servicos .NET...
taskkill /f /im dotnet.exe >nul 2>&1
if %errorlevel% equ 0 (
    echo Servicos .NET parados.
) else (
    echo Nenhum servico .NET encontrado.
)

echo [2/3] Parando containers Docker...
docker-compose down
if %errorlevel% equ 0 (
    echo Containers parados.
) else (
    echo Erro ao parar containers.
)

echo [3/3] Limpando processos...
taskkill /f /im "ProposalService.exe" >nul 2>&1
taskkill /f /im "ContractService.exe" >nul 2>&1

echo.
echo Sistema parado com sucesso!
echo.
pause 