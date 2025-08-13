#!/bin/bash

# Sistema de Seguros - Inicializacao Bash
# Executar como: ./start-system.sh

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Função para spinner
spinner() {
    local pid=$1
    local delay=0.1
    local spinstr='|/-\'
    while [ "$(ps a | awk '{print $1}' | grep $pid)" ]; do
        local temp=${spinstr#?}
        printf " [%c]  " "$spinstr"
        local spinstr=$temp${spinstr%"$temp"}
        sleep $delay
        printf "\b\b\b\b\b\b"
    done
    printf "    \b\b\b\b"
}

# Função para aguardar com spinner
wait_with_spinner() {
    local seconds=$1
    local message=$2
    echo -n "$message "
    sleep $seconds &
    spinner $!
    echo "Concluído!"
}

# Função para verificar se comando existe
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Função para mostrar erro e sair
error_exit() {
    echo -e "${RED}ERRO: $1${NC}" >&2
    exit 1
}

# Função para mostrar sucesso
success_msg() {
    echo -e "${GREEN}$1${NC}"
}

# Função para mostrar info
info_msg() {
    echo -e "${CYAN}$1${NC}"
}

# Função para mostrar warning
warning_msg() {
    echo -e "${YELLOW}$1${NC}"
}

echo -e "${CYAN}========================================"
echo -e "Sistema de Seguros - Inicializacao"
echo -e "========================================${NC}"
echo ""

# 1. Verificar Docker
info_msg "[1/5] Verificando Docker..."

if ! command_exists docker; then
    error_exit "Docker não encontrado. Instale o Docker."
fi

# Verificar se Docker está rodando
if ! docker info >/dev/null 2>&1; then
    error_exit "Docker não está rodando. Inicie o Docker."
fi

success_msg "Docker encontrado e rodando."

# 2. Iniciar containers
info_msg "[2/5] Iniciando containers Docker..."

if ! docker-compose up -d; then
    error_exit "Falha ao iniciar containers Docker."
fi

success_msg "Containers iniciados com sucesso."

# 3. Aguardar bancos ficarem prontos
info_msg "[3/5] Aguardando bancos ficarem prontos..."
wait_with_spinner 15 "Aguardando SQL Server e PostgreSQL..."

# 4. Executar migrations
info_msg "[4/5] Executando migrations..."

# ProposalService
warning_msg "  - ProposalService (SQL Server)..."
cd src/ProposalService/ProposalService.Infrastructure || error_exit "Diretório não encontrado"
if ! dotnet ef database update --no-build; then
    error_exit "Falha na migration do ProposalService"
fi

# ContractService
warning_msg "  - ContractService (PostgreSQL)..."
cd ../../../ContractService/ContractService.Infrastructure || error_exit "Diretório não encontrado"
if ! dotnet ef database update --no-build; then
    error_exit "Falha na migration do ContractService"
fi

cd ../../..

success_msg "Migrations executadas com sucesso."

# 5. Iniciar serviços
info_msg "[5/5] Iniciando serviços..."
echo ""
echo -e "${CYAN}========================================"
echo -e "Serviços sendo iniciados..."
echo -e "========================================${NC}"
echo ""
success_msg "ProposalService: https://localhost:7001/swagger"
success_msg "ContractService: https://localhost:7002/swagger"
echo ""
warning_msg "Pressione Ctrl+C para parar os serviços"
echo ""

# Função para iniciar serviço em background
start_service() {
    local service_name=$1
    local service_path=$2
    local port=$3
    
    echo -e "${YELLOW}Iniciando $service_name na porta $port...${NC}"
    cd "$service_path" || error_exit "Diretório $service_path não encontrado"
    
    # Iniciar em background e salvar PID
    dotnet run > "../${service_name}.log" 2>&1 &
    local pid=$!
    echo $pid > "../${service_name}.pid"
    
    # Aguardar um pouco para o serviço inicializar
    sleep 3
    
    # Verificar se o processo ainda está rodando
    if kill -0 $pid 2>/dev/null; then
        success_msg "$service_name iniciado com PID $pid"
    else
        error_exit "Falha ao iniciar $service_name"
    fi
    
    cd ../..
}

# Iniciar ProposalService
start_service "ProposalService" "src/ProposalService/ProposalService.API" "7001"

# Iniciar ContractService
start_service "ContractService" "src/ContractService/ContractService.API" "7002"

echo ""
success_msg "Sistema iniciado com sucesso!"
warning_msg "Aguarde alguns segundos para os serviços ficarem prontos."
echo ""

# Mostrar status dos containers
info_msg "Status dos containers:"
docker-compose ps

echo ""
info_msg "Logs dos serviços:"
echo "  ProposalService: tail -f src/ProposalService/ProposalService.log"
echo "  ContractService: tail -f src/ContractService/ContractService.log"
echo ""

# Função para limpeza ao sair
cleanup() {
    echo ""
    warning_msg "Parando serviços..."
    
    # Parar serviços
    if [ -f "src/ProposalService/ProposalService.pid" ]; then
        kill $(cat src/ProposalService/ProposalService.pid) 2>/dev/null
        rm src/ProposalService/ProposalService.pid
    fi
    
    if [ -f "src/ContractService/ContractService.pid" ]; then
        kill $(cat src/ContractService/ContractService.pid) 2>/dev/null
        rm src/ContractService/ContractService.pid
    fi
    
    echo "Serviços parados."
    exit 0
}

# Capturar Ctrl+C
trap cleanup SIGINT

echo -e "${YELLOW}Pressione Ctrl+C para parar todos os serviços${NC}"
echo ""

# Manter script rodando
while true; do
    sleep 1
done 