# Teste Tecnico FGV Conhecimento - Sistema de Vendas

Aplicação fullstack desenvolvida como parte do processo seletivo para a vaga de **Analista Desenvolvedor de Sistemas Fullstack Pleno** da FGV Conhecimento.

O sistema permite o gerenciamento de clientes e produtos, além da criação e manutenção de pedidos com controle de estoque e cálculo automático do valor total.

## Stack

### Backend

- ASP.NET Core 8
- C#
- REST API
- Dapper
- SQL Server 2022
- Swagger / OpenAPI

### Frontend

- Next.js 14
- React
- TypeScript
- App Router
- Tailwind CSS

### Banco de dados

- Microsoft SQL Server 2022
- Scripts SQL para criação das tabelas e seed inicial

### Infraestrutura

- Docker
- Docker Compose

---

## Arquitetura

O backend foi estruturado em camadas, buscando separação de responsabilidades, baixo acoplamento e facilidade de manutenção.

### Responsabilidades das camadas

**Sales.Api**

Responsável pela exposição dos endpoints HTTP, configuração da aplicação, middleware de tratamento de exceções e documentação da API.

**Sales.Application**

Contém os casos de uso da aplicação, DTOs, interfaces e services responsáveis pela orquestração das regras de negócio.

**Sales.Domain**

Contém as entidades e regras relacionadas ao domínio da aplicação, mantendo-as independentes de detalhes de infraestrutura.

**Sales.Infrastructure**

Responsável pela persistência dos dados, acesso ao SQL Server utilizando Dapper, implementação dos repositories e gerenciamento das transações.

---

## Funcionalidades

### Clientes

- Listagem de clientes
- Cadastro de clientes
- Validação de CNPJ
- Garantia de CNPJ único

### Produtos

- Listagem de produtos
- Cadastro de produtos
- Atualização de produtos
- Controle de estoque

### Pedidos

- Listagem de pedidos
- Filtro por cliente
- Filtro por intervalo de datas
- Criação de pedidos
- Inclusão de produtos no pedido
- Alteração da quantidade de itens
- Alteração do preço unitário
- Remoção de itens
- Cálculo automático do valor total
- Validação de estoque disponível
- Atualização do estoque durante as operações do pedido
- Restauração do estoque ao remover itens
- Visualização detalhada do pedido

---

## Principais endpoints

### Clientes

```text
GET    /api/clients
GET    /api/clients/{id}
POST   /api/clients
```

### Produtos

```text
GET    /api/products
GET    /api/products/{id}
POST   /api/products
PUT    /api/products/{id}
```

### Pedidos

```text
GET    /api/orders
GET    /api/orders/{id}
POST   /api/orders
POST   /api/orders/{id}/items
PUT    /api/orders/{id}/items/{productId}
DELETE /api/orders/{id}/items/{productId}
```

Os pedidos também permitem filtragem por cliente e intervalo de datas:

```text
GET /api/orders?clientId={id}&dateFrom={date}&dateTo={date}
```

---

---

# Executando com Docker

## Pré-requisitos

- Docker
- Docker Compose

Na raiz do projeto:

```bash
docker compose up --build
```

Após a inicialização:

- Frontend: http://localhost:3000
- API / Swagger: http://localhost:5000/swagger
- SQL Server: `localhost:1433`

Para executar em segundo plano:

```bash
docker compose up --build -d
```

Para verificar os containers:

```bash
docker compose ps
```

Para interromper a aplicação:

```bash
docker compose down
```

---

# Executando localmente

## 1. Banco de dados

Suba o SQL Server e o serviço responsável pela inicialização:

```bash
docker compose up sqlserver db-init
```

## 2. Backend

```bash
cd backend

dotnet restore
dotnet build

dotnet run --project Sales.Api/Sales.Api.csproj
```

A documentação da API estará disponível no Swagger.

## 3. Frontend

Em outro terminal:

```bash
cd frontend

npm install
npm run dev
```

O frontend estará disponível em:

```text
http://localhost:3000
```

---

## Uso de IA no desenvolvimento

Ferramentas de inteligência artificial foram utilizadas como apoio durante o desenvolvimento, principalmente para consultas, pesquisas de documentação, esclarecimento de dúvidas técnicas e discussão de possíveis abordagens de implementação.

As decisões de arquitetura, implementação, integração entre as camadas, testes e validação do funcionamento da aplicação foram realizadas e devidamente revisadas durante o desenvolvimento do projeto.

---

## Autor

Desenvolvido por **Gabriel Torres da Costa**
[https://linkedin.com/in/gabriel-t-costa]