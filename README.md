# Sistema E-commerce - .NET 6 + Angular 18

Sistema completo de e-commerce com backend em .NET 6 e frontend em Angular 18, seguindo boas práticas de arquitetura e testes.

## 📹 Demonstração em Vídeo

### Visualização do Sistema
Assista à demonstração completa do sistema e-commerce:

[![Demonstração do Sistema E-commerce](https://img.youtube.com/vi/bYwI7J5qpjM/0.jpg)](https://www.youtube.com/watch?v=bYwI7J5qpjM)

**🎬 [Clique aqui para assistir no YouTube](https://www.youtube.com/watch?v=bYwI7J5qpjM)**

---

## 📋 Requisitos do Sistema

### Backend
- **.NET 6 SDK** ou superior
- **PostgreSQL 12+** 
- **Entity Framework Core Tools** (`dotnet tool install --global dotnet-ef`)

### Frontend
- **Node.js 18+** e **npm**
- **Angular CLI 18** (`npm install -g @angular/cli@18`)

### Ferramentas de Desenvolvimento
- **Visual Studio Code** ou **Visual Studio 2022**
- **pgAdmin** (opcional - interface gráfica para PostgreSQL)

## 📥 Clone do Repositório

Para começar, clone o repositório do projeto e instale as dependências:

```bash
git clone https://github.com/Jacksons357/ecommerce-net.git
cd ecommerce-net
```

## 🗄️ Configuração do Banco de Dados (PostgreSQL)

### 1. Instalação do PostgreSQL

**Windows:**
1. Baixe o PostgreSQL em: https://www.postgresql.org/download/windows/
2. Execute o instalador e configure:
   - Usuário: `postgres`
   - Senha: `postgres` (ou sua preferência)
   - Porta: `5432`

**Linux (Ubuntu/Debian):**
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo -u postgres psql
ALTER USER postgres PASSWORD 'postgres';
\q
```

**macOS:**
```bash
brew install postgresql
brew services start postgresql
psql postgres
ALTER USER postgres PASSWORD 'postgres';
\q
```

### 2. Criar o Banco de Dados

```sql
-- Conectar ao PostgreSQL
psql -U postgres -h localhost

-- Criar o banco de dados
CREATE DATABASE ecommerce_db;

-- Sair
\q
```

### 3. Configurar Connection String

No arquivo `ecommerce-api/src/Ecommerce.Api/appsettings.json`, ajuste a connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ecommerce_db;Username=postgres;Password=SUA_SENHA_AQUI"
  }
}
```

## 🚀 Instruções de Instalação

### 1. Backend (.NET 6)

```bash
# Navegar para a pasta do backend
cd ecommerce-api

# Restaurar dependências
dotnet restore

# Aplicar migrations (criar tabelas)
dotnet ef database update --project src/Ecommerce.Infrastructure --startup-project src/Ecommerce.Api

# Executar a API
dotnet run --project src/Ecommerce.Api --urls http://localhost:5000
```

A API estará disponível em:
- **Swagger**: http://localhost:5000
- **API Base**: http://localhost:5000/api

⚠️ **Importante**: O backend está configurado para rodar apenas em **HTTP na porta 5000**. Não tente acessar via HTTPS (porta 5001), pois isso causará problemas de CORS e conexão com o frontend Angular.

### 2. Frontend (Angular 18)

```bash
# Navegar para a pasta do frontend
cd ecommerce-front

# Instalar dependências
npm install

# Executar o desenvolvimento
ng serve --port 4200
```

O frontend estará disponível em: http://localhost:4200

## 🔐 Dados de Acesso do Admin

### Usuário Administrador (Criado automaticamente pelo seeder)
- **Email**: `admin@local`
- **Senha**: `Admin@123`

### Dados Iniciais
O sistema inicia com:
- ✅ **1 usuário admin** para fazer login
- 🗃️ **Tabelas vazias** para clientes, produtos e pedidos
- 📊 **Banco limpo** pronto para uso

## 🧪 Instruções para Executar Testes

### Testes do Backend (.NET)

```bash
# Navegar para a pasta do backend
cd ecommerce-api

# Executar todos os testes
dotnet test tests/Ecommerce.Tests/Ecommerce.Tests.csproj

# Executar testes com detalhes
dotnet test tests/Ecommerce.Tests/Ecommerce.Tests.csproj --verbosity normal

# Ou usando a solução principal
dotnet test Ecommerce.sln
```

**Resultado esperado**: 15 testes aprovados, 1 com falha (problema conhecido com transações no banco InMemory)

### Testes do Frontend (Angular)

```bash
# Navegar para a pasta do frontend
cd ecommerce-front

# Executar todos os testes (modo single-run)
npm test -- --watch=false --browsers=ChromeHeadless

# Executar testes em modo watch (desenvolvimento)
ng test

# Executar testes com coverage
ng test --code-coverage
```

**Resultado esperado**: 3 testes aprovados, 4 com falha (falta de mocks para HttpClient e ActivatedRoute)

## 🎯 Funcionalidades

- ✅ **Autenticação JWT** com usuário padrão
- ✅ **Gestão de Clientes** (CRUD completo)
- ✅ **Gestão de Produtos** (CRUD completo) 
- ✅ **Gestão de Pedidos** (Criar, Pagar, Cancelar)
- ✅ **Histórico de Eventos** (Auditoria completa)
- ✅ **Filtros e Busca** em todas as listagens
- ✅ **Validações** no frontend e backend
- ✅ **Testes Unitários** nos services principais

## 🔄 Instruções de Reset do Sistema

### Reset Completo - Método 1: Script PowerShell (Windows)

```powershell
# Navegar para a pasta do backend
cd ecommerce-api

# Executar script de reset (se disponível)
./reset-database.ps1
```

### Reset Completo - Método 2: Comandos Manuais

```bash
# 1. Navegar para a pasta do backend
cd ecommerce-api

# 2. Parar aplicação (se estiver rodando)
# Windows PowerShell:
taskkill /f /im dotnet.exe
# Linux/macOS:
pkill dotnet

# 3. Remover banco de dados
dotnet ef database drop --force --project src/Ecommerce.Infrastructure --startup-project src/Ecommerce.Api

# 4. Recriar banco com todas as migrations
dotnet ef database update --project src/Ecommerce.Infrastructure --startup-project src/Ecommerce.Api

# 5. Executar a aplicação (o seeder criará o usuário admin automaticamente)
dotnet run --project src/Ecommerce.Api --urls http://localhost:5000
```

### Reset Apenas dos Dados (manter estrutura)

```sql
-- Conectar ao banco PostgreSQL
psql -U postgres -d ecommerce_db

-- Limpar dados das tabelas (ordem importante devido às foreign keys)
DELETE FROM "PedidoItens";
DELETE FROM "Pedidos";
DELETE FROM "HistoricoEventos";
DELETE FROM "Clientes";
DELETE FROM "Produtos";
DELETE FROM "Usuarios";

-- Sair
\q
```

⚠️ **Importante**: Após limpar os dados manualmente, execute a aplicação para que o seeder recrie o usuário admin.

### Após o Reset

Depois do reset, o banco terá apenas:
- ✅ **Usuário Admin**: `admin@local` / `Admin@123`
- ✅ **Tabelas vazias**: Clientes, Produtos, Pedidos
- ✅ **Estrutura completa**: Todas as tabelas e relacionamentos

## 🚀 Execução Completa do Sistema

Para executar o sistema completo pela primeira vez, siga esta ordem:

### 1. Preparação Inicial
```bash
# Clone e navegue para o projeto
git clone https://github.com/Jacksons357/ecommerce-net.git
cd ecommerce-net

# Instale as dependências do Angular
cd ecommerce-front
npm install
cd ..
```

### 2. Configure o Banco de Dados
1. **Instale o PostgreSQL** (conforme instruções acima)
2. **Crie o banco**: `CREATE DATABASE ecommerce_db;`
3. **Configure a connection string** no `appsettings.json`

### 3. Execute o Backend (Terminal 1)
```bash
cd ecommerce-api

# Restaure dependências e aplique migrations
dotnet restore
dotnet ef database update --project src/Ecommerce.Infrastructure --startup-project src/Ecommerce.Api

# Execute a API (mantenha rodando)
dotnet run --project src/Ecommerce.Api --urls http://localhost:5000
```

### 4. Execute o Frontend (Terminal 2)
```bash
cd ecommerce-front

# Execute o Angular (mantenha rodando)
ng serve --port 4200
```

### 5. Acesse o Sistema
- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5000/api
- **Swagger**: http://localhost:5000

### 6. Faça Login
- **Email**: `admin@local`
- **Senha**: `Admin@123`

⚠️ **Importante**: Mantenha ambos os terminais abertos - um para o backend (.NET) e outro para o frontend (Angular).

---