# Build stage
FROM node:20-alpine AS deps
WORKDIR /app

# Copiar arquivos de dependências
COPY package*.json ./
COPY yarn.lock* ./
COPY pnpm-lock.yaml* ./

# Instalar dependências com cache otimizado
RUN npm ci --only=production --frozen-lockfile || \
    yarn install --frozen-lockfile --production || \
    pnpm install --frozen-lockfile --prod || \
    npm install --only=production

# Build stage
FROM node:20-alpine AS builder
WORKDIR /app

# Copiar dependências do stage anterior
COPY --from=deps /app/node_modules ./node_modules
COPY package*.json ./

# Copiar código fonte
COPY . .

# Build da aplicação
RUN npm run build

# Production stage
FROM nginx:1.25-alpine AS production

# Instalar curl para health checks
RUN apk add --no-cache curl

# Copiar build da aplicação
COPY --from=builder /app/build /usr/share/nginx/html

# Copiar configuração do nginx
COPY nginx.conf /etc/nginx/nginx.conf

# Expor porta
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Iniciar nginx
CMD ["nginx", "-g", "daemon off;"] 