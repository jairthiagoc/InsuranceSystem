# 🎨 Frontend - Sistema de Seguros

## 📋 Visão Geral

Frontend moderno desenvolvido em **React + TypeScript** para o Sistema de Seguros, com interface intuitiva e responsiva para gerenciamento de propostas e contratos.

## 🚀 Tecnologias Utilizadas

### Core
- **React 18** - Biblioteca principal
- **TypeScript** - Tipagem estática
- **React Router DOM** - Roteamento
- **React Query** - Gerenciamento de estado e cache

### UI/UX
- **Tailwind CSS** - Framework CSS utilitário
- **Framer Motion** - Animações fluidas
- **Lucide React** - Ícones modernos
- **React Hook Form** - Formulários com validação
- **React Hot Toast** - Notificações elegantes

### Comunicação
- **Axios** - Cliente HTTP
- **Socket.io Client** - WebSockets para tempo real

## 🏗️ Estrutura do Projeto

```
frontend/
├── public/
│   └── index.html
├── src/
│   ├── components/
│   │   ├── Dashboard.tsx          # Dashboard principal
│   │   ├── ProposalForm.tsx       # Formulário de propostas
│   │   ├── ProposalList.tsx       # Lista de propostas
│   │   ├── ContractList.tsx       # Lista de contratos
│   │   └── Sidebar.tsx            # Navegação lateral
│   ├── services/
│   │   ├── api.ts                 # Serviços de API
│   │   └── websocket.ts           # Serviço WebSocket
│   ├── types/
│   │   └── index.ts               # Tipos TypeScript
│   ├── App.tsx                    # Componente principal
│   ├── index.tsx                  # Ponto de entrada
│   ├── App.css                    # Estilos do App
│   └── index.css                  # Estilos globais
├── package.json                   # Dependências
├── tailwind.config.js             # Configuração Tailwind
└── README.md                      # Documentação
```

## 🛠️ Instalação e Execução

### Pré-requisitos
- **Node.js 16+** e **npm**
- Backend rodando (APIs na porta 7001 e 7002)

### 1. Instalar Dependências
```bash
cd frontend
npm install
```

### 2. Executar em Desenvolvimento
```bash
npm start
```

### 3. Build para Produção
```bash
npm run build
```

## 🎯 Funcionalidades Implementadas

### 📊 Dashboard
- **Estatísticas em tempo real**
- **Cards animados** com métricas
- **Atividade recente**
- **Gráficos de performance**

### 📝 Gestão de Propostas
- **Criar nova proposta** com validação
- **Listar propostas** com filtros
- **Busca avançada** por cliente/tipo
- **Filtros por status** (Draft, UnderReview, Approved, Rejected)
- **Ações em lote**

### 📋 Gestão de Contratos
- **Visualizar contratos** ativos
- **Busca por número/cliente**
- **Detalhes completos** do contrato
- **Histórico de alterações**

### 🔄 Comunicação em Tempo Real
- **WebSockets** para atualizações
- **Notificações push** de eventos
- **Sincronização automática** de dados
- **Indicadores de status** em tempo real

## 🎨 Design System

### Cores
- **Primary**: Azul (#3B82F6)
- **Success**: Verde (#22C55E)
- **Warning**: Amarelo (#F59E0B)
- **Danger**: Vermelho (#EF4444)

### Componentes
- **Cards** com sombras e bordas arredondadas
- **Botões** com estados hover/focus
- **Formulários** com validação visual
- **Tabelas** responsivas
- **Modais** e **tooltips**

### Animações
- **Fade In/Out** para transições
- **Slide animations** para listas
- **Loading spinners** personalizados
- **Hover effects** suaves

## 🔧 Configuração

### Variáveis de Ambiente
```bash
# .env
REACT_APP_API_URL=http://localhost:7001
REACT_APP_CONTRACT_API_URL=http://localhost:7002
REACT_APP_WS_URL=http://localhost:7001
```

### Proxy Configuration
O projeto está configurado para fazer proxy das requisições para o backend:
```json
{
  "proxy": "http://localhost:7001"
}
```

## 📱 Responsividade

- **Mobile First** design
- **Breakpoints** otimizados
- **Grid system** flexível
- **Touch-friendly** interfaces

## 🔒 Segurança

- **Validação client-side** robusta
- **Sanitização** de inputs
- **CORS** configurado
- **Error boundaries** implementados

## 🧪 Testes

### Executar Testes
```bash
npm test
```

### Cobertura de Testes
```bash
npm run test:coverage
```

## 🚀 Deploy

### Build de Produção
```bash
npm run build
```

### Servir Build
```bash
npx serve -s build
```

## 🔄 Integração com Backend

### APIs Consumidas
- **ProposalService** (porta 7001)
  - `GET /api/proposals` - Listar propostas
  - `POST /api/proposals` - Criar proposta
  - `PUT /api/proposals/{id}/status` - Atualizar status

- **ContractService** (porta 7002)
  - `GET /api/contracts` - Listar contratos
  - `POST /api/contracts` - Criar contrato

### WebSocket Events
- `proposal_created` - Nova proposta criada
- `proposal_updated` - Proposta atualizada
- `contract_created` - Novo contrato criado
- `status_changed` - Status alterado


## 🐛 Troubleshooting

### Problemas Comuns

1. **Erro de CORS**
   ```bash
   # Verificar se o backend está rodando
   curl http://localhost:7001/api/proposals
   ```

2. **WebSocket não conecta**
   ```bash
   # Verificar se o WebSocket está habilitado no backend
   # Verificar firewall/portas
   ```

3. **Build falha**
   ```bash
   # Limpar cache
   npm run build -- --reset-cache
   ```

## 📚 Referências

- [React Documentation](https://reactjs.org/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Tailwind CSS](https://tailwindcss.com/)
- [Framer Motion](https://www.framer.com/motion/)
- [React Query](https://tanstack.com/query/latest)

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

---

**Desenvolvido com ❤️ para o Sistema de Seguros** 