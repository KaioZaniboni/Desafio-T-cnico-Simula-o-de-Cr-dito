# 🏦 Simulação de Crédito API

API para simulação de empréstimos com cálculos SAC e PRICE, desenvolvida em .NET 8 seguindo os princípios de Clean Architecture.

## 🏗️ Arquitetura

```
SimulacaoCredito/
├── src/
│   ├── SimulacaoCredito.Api/          # Camada de apresentação (Controllers, DTOs)
│   ├── SimulacaoCredito.Application/  # Casos de uso e serviços de aplicação
│   ├── SimulacaoCredito.Domain/       # Entidades e regras de negócio
│   └── SimulacaoCredito.Infrastructure/ # Acesso a dados e integrações externas
├── tests/
│   ├── SimulacaoCredito.Api.Tests/
│   ├── SimulacaoCredito.Application.Tests/
│   └── SimulacaoCredito.Domain.Tests/
├── docker/
│   ├── Dockerfile
│   └── docker-compose.yml
└── docs/
```

## 🚀 Funcionalidades

- ✅ **Simulação de Empréstimos** - Cálculos SAC e PRICE
- ✅ **Validação de Produtos** - Baseada em parâmetros do banco de dados
- ✅ **Integração EventHub** - Envio de eventos de simulação
- ✅ **Persistência** - Armazenamento de simulações realizadas
- ✅ **Relatórios** - Volume por produto/dia e telemetria
- ✅ **Containerização** - Docker e Docker Compose

## 📋 Endpoints

### 🔄 Simulação
- `POST /api/simulacao` - Realizar simulação de empréstimo

### 📊 Consultas
- `GET /api/simulacoes` - Listar simulações (com paginação)
- `GET /api/relatorio/volume-produto-dia` - Volume simulado por produto/dia
- `GET /api/telemetria` - Dados de telemetria e performance

## 🛠️ Tecnologias

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQL Server** - Banco de dados
- **Azure EventHub** - Mensageria
- **Docker** - Containerização
- **xUnit** - Testes unitários
- **Swagger** - Documentação da API

## ⚙️ Configuração

### Pré-requisitos
- .NET 8 SDK
- Docker Desktop
- SQL Server (Azure)

### Variáveis de Ambiente

```bash
# Banco de dados
ConnectionStrings__DefaultConnection=Server=dbhackathon.database.windows.net,1433;Database=hack;User Id=hack;Password=Password123;

# EventHub
EventHub__ConnectionString=Endpoint=sb://eventhack.servicebus.windows.net/;SharedAccessKeyName=hack;SharedAccessKey=HeHeVaVayVkntO2FnjQcs2Ilh/4MUDo4y+AEhKp8z+g=;EntityPath=simulacoes
```

## 🐳 Execução com Docker

### Desenvolvimento
```bash
# Executar com docker-compose
docker-compose -f docker/docker-compose.yml up --build

# A API estará disponível em:
# HTTP: http://localhost:8080
# HTTPS: https://localhost:8443
# Swagger: http://localhost:8080/swagger
```

### Produção
```bash
# Build da imagem
docker build -f docker/Dockerfile -t simulacao-credito-api .

# Executar container
docker run -p 8080:80 simulacao-credito-api
```

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test tests/SimulacaoCredito.Domain.Tests/
```

## 📊 Estrutura do Banco de Dados

### Tabela PRODUTO
```sql
CREATE TABLE dbo.PRODUTO (
    CO_PRODUTO int NOT NULL primary key,
    NO_PRODUTO varchar(200) NOT NULL,
    PC_TAXA_JUROS numeric(18,9) NOT NULL,
    NU_MINIMO_MESES smallint NOT NULL,
    NU_MAXIMO_MESES smallint NULL,
    VR_MINIMO numeric(18,2) NOT NULL,
    VR_MAXIMO numeric(18,2) NULL
);
```

## 📈 Monitoramento

A API inclui:
- **Health Checks** - Verificação de saúde da aplicação
- **Telemetria** - Métricas de performance e uso
- **Logs estruturados** - Para debugging e monitoramento

## 🔒 Segurança

- Validação de entrada rigorosa
- Tratamento de exceções centralizado
- Logs de auditoria
- Configuração segura de conexões

## 📝 Desenvolvimento

### Executar localmente
```bash
# Restaurar dependências
dotnet restore

# Executar a API
dotnet run --project src/SimulacaoCredito.Api

# A API estará disponível em:
# HTTP: http://localhost:5000
# HTTPS: https://localhost:5001
# Swagger: http://localhost:5000/swagger
```

### Estrutura de Commits
- `feat:` - Nova funcionalidade
- `fix:` - Correção de bug
- `docs:` - Documentação
- `test:` - Testes
- `refactor:` - Refatoração

## 📞 Suporte

Para dúvidas ou problemas:
1. Verifique a documentação da API no Swagger
2. Consulte os logs da aplicação
3. Execute os testes para validar o ambiente

## 📄 Licença

Este projeto foi desenvolvido como parte de um desafio técnico.
