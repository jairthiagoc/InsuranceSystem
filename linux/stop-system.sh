#!/bin/bash

# Sistema de Seguros - Parando Servicos Bash
# Executar como: ./stop-system.sh

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}========================================"
echo -e "Sistema de Seguros - Parando Servicos"
echo -e "========================================${NC}"
echo ""

# 1. Parar serviços .NET
echo -e "${CYAN}[1/3] Parando serviços .NET...${NC}"

# Encontrar e parar processos dotnet
dotnet_pids=$(pgrep -f "dotnet.*run" 2>/dev/null)
if [ -n "$dotnet_pids" ]; then
    echo "$dotnet_pids" | xargs kill -9
    echo -e "${GREEN}Serviços .NET parados.${NC}"
else
    echo -e "${YELLOW}Nenhum serviço .NET encontrado.${NC}"
fi

# 2. Parar containers Docker
echo -e "${CYAN}[2/3] Parando containers Docker...${NC}"
if docker-compose down; then
    echo -e "${GREEN}Containers parados.${NC}"
else
    echo -e "${RED}Erro ao parar containers.${NC}"
fi

# 3. Limpar processos específicos
echo -e "${CYAN}[3/3] Limpando processos...${NC}"

# Parar processos pelos PIDs salvos
if [ -f "src/ProposalService/ProposalService.pid" ]; then
    pid=$(cat src/ProposalService/ProposalService.pid)
    if kill -0 $pid 2>/dev/null; then
        kill -9 $pid
        echo -e "${GREEN}Processo ProposalService parado.${NC}"
    fi
    rm src/ProposalService/ProposalService.pid
fi

if [ -f "src/ContractService/ContractService.pid" ]; then
    pid=$(cat src/ContractService/ContractService.pid)
    if kill -0 $pid 2>/dev/null; then
        kill -9 $pid
        echo -e "${GREEN}Processo ContractService parado.${NC}"
    fi
    rm src/ContractService/ContractService.pid
fi

# 4. Limpar arquivos temporários
echo -e "${CYAN}[4/4] Limpando arquivos temporários...${NC}"

temp_files=(
    "src/ProposalService/ProposalService.log"
    "src/ContractService/ContractService.log"
)

for file in "${temp_files[@]}"; do
    if [ -f "$file" ]; then
        rm "$file"
        echo -e "${YELLOW}Arquivo temporário removido: $file${NC}"
    fi
done

echo ""
echo -e "${GREEN}Sistema parado com sucesso!${NC}"
echo "" 