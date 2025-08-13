@echo off
echo 🔍 Iniciando análise de cobertura de código...

echo 📊 Executando testes com cobertura...
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults

echo 📋 Verificando ReportGenerator...
reportgenerator --version >nul 2>&1
if errorlevel 1 (
    echo 📦 Instalando ReportGenerator...
    dotnet tool install -g dotnet-reportgenerator-globaltool
)

echo 📈 Gerando relatório HTML...
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html

if exist "coverage-report\index.html" (
    echo ✅ Relatório de cobertura gerado com sucesso!
    echo 📁 Localização: coverage-report\index.html
    echo 🌐 Abrindo relatório no navegador...
    start coverage-report\index.html
) else (
    echo ❌ Erro ao gerar relatório de cobertura
)

echo 🎉 Análise de cobertura concluída!
pause 