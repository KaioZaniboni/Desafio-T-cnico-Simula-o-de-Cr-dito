# Checklist - Projeto Simulação de Crédito

## 📋 Visão Geral
Desenvolvimento de API para simulação de empréstimos com cálculos SAC e PRICE, integração com EventHub e persistência em SQL Server.

---

## 🏗️ 1. Configuração da Infraestrutura

### 1.1 Banco de Dados SQL Server
- [ ] Configurar conexão com SQL Server
  - **URL:** `dbhackathon.database.windows.net`
  - **Porta:** `1433`
  - **Database:** `hack`
  - **Login:** `hack`
  - **Senha:** `Password123`

### 1.2 Estrutura do Banco
- [ ] Criar tabela `dbo.PRODUTO`
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

- [ ] Inserir dados de teste na tabela PRODUTO
  - Produto 1: Taxa 0.017980000, 6-24 meses, R$ 200-10000
  - Produto 2: Taxa 0.017560000, 25-48 meses, R$ 10001-10000000
  - Produto 3: Taxa 0.015100000, 49-96 meses, R$ 100000-10000000
  - Produto 4: Taxa 0.015100000, 97-null meses, R$ null-10000000

### 1.3 EventHub
- [ ] Configurar integração com EventHub
  - **Endpoint:** `sb://eventhack.servicebus.windows.net/SharedAccessKeyName=hack;SharedAccessKey=HetVeVayWonO2PnjGcs2iIh/4MUDo4y+AEhKp8z+g=;EntityPath=simulacoes`

---

## 🔧 2. Desenvolvimento da API

### 2.1 Modelos de Dados

#### 2.1.1 Modelo de Entrada para Simulação
- [ ] Criar modelo `SimulacaoRequest`
  ```json
  {
    "valorDesejado": 900.00,
    "prazo": 5
  }
  ```

#### 2.1.2 Modelo de Resposta da Simulação
- [ ] Criar modelo `SimulacaoResponse`
  ```json
  {
    "idSimulacao": 20180702,
    "codigoProduto": 1,
    "descricaoProduto": "Produto 1",
    "taxaJuros": 0.0179,
    "resultadoSimulacao": {
      "tipo": "SAC",
      "parcelas": [
        {
          "numero": 1,
          "valorAmortizacao": 180.00,
          "valorJuros": 16.11,
          "valorPrestacao": 196.11
        }
      ]
    },
    "percentualSucesso": 0.98
  }
  ```

#### 2.1.3 Modelo de Listagem de Simulações
- [ ] Criar modelo `ListaSimulacoesResponse`
  ```json
  {
    "pagina": 1,
    "qtdRegistros": 404,
    "qtdRegistrosPagina": 200,
    "registros": [...]
  }
  ```

#### 2.1.4 Modelo de Volume por Produto/Dia
- [ ] Criar modelo `VolumeSimuladoResponse`
  ```json
  {
    "dataReferencia": "2025-07-30",
    "simulacoes": [
      {
        "codigoProduto": 1,
        "descricaoProduto": "Produto 1",
        "taxaMediaJuro": 0.189,
        "valorMediorPrestacao": 300.00,
        "valorTotalDesejado": 12047.47,
        "valorTotalCredito": 16750.00
      }
    ]
  }
  ```

#### 2.1.5 Modelo de Telemetria
- [ ] Criar modelo `TelemetriaResponse`
  ```json
  {
    "dataReferencia": "2025-07-30",
    "listaEndpoints": [
      {
        "nomeApi": "Simulacao",
        "qtdRequisicoes": 135,
        "tempoMedio": 150,
        "tempoMinimo": 23,
        "tempoMaximo": 860
      }
    ]
  }
  ```

### 2.2 Endpoints da API

#### 2.2.1 Endpoint de Simulação
- [ ] `POST /simulacao`
  - [ ] Receber envelope JSON com valorDesejado e prazo
  - [ ] Validar dados de entrada
  - [ ] Consultar produtos no banco SQL Server
  - [ ] Filtrar produto adequado aos parâmetros
  - [ ] Realizar cálculos SAC e PRICE
  - [ ] Persistir simulação no banco
  - [ ] Enviar evento para EventHub
  - [ ] Retornar resultado da simulação

#### 2.2.2 Endpoint de Listagem
- [ ] `GET /simulacoes`
  - [ ] Implementar paginação
  - [ ] Retornar lista de simulações realizadas
  - [ ] Incluir metadados de paginação

#### 2.2.3 Endpoint de Volume por Produto/Dia
- [ ] `GET /relatorio/volume-produto-dia`
  - [ ] Filtrar por data de referência
  - [ ] Agrupar por produto
  - [ ] Calcular métricas agregadas
  - [ ] Retornar dados consolidados

#### 2.2.4 Endpoint de Telemetria
- [ ] `GET /telemetria`
  - [ ] Coletar dados de performance
  - [ ] Calcular tempos de resposta
  - [ ] Contar requisições por endpoint
  - [ ] Retornar métricas de telemetria

---

## 🧮 3. Lógica de Negócio

### 3.1 Cálculos de Amortização
- [ ] Implementar cálculo SAC (Sistema de Amortização Constante)
  - [ ] Amortização constante por parcela
  - [ ] Juros decrescentes
  - [ ] Prestação decrescente

- [ ] Implementar cálculo PRICE (Tabela Price)
  - [ ] Prestação constante
  - [ ] Amortização crescente
  - [ ] Juros decrescentes

### 3.2 Validações
- [ ] Validar valor desejado dentro dos limites do produto
- [ ] Validar prazo dentro dos limites do produto
- [ ] Validar existência do produto
- [ ] Tratar casos de erro e exceções

---

## 🔗 4. Integrações

### 4.1 EventHub
- [ ] Configurar cliente EventHub
- [ ] Implementar envio de eventos de simulação
- [ ] Tratar erros de conexão
- [ ] Implementar retry policy

### 4.2 SQL Server
- [ ] Configurar Entity Framework ou ADO.NET
- [ ] Implementar repositórios
- [ ] Gerenciar conexões
- [ ] Implementar transações

---

## 🧪 5. Testes

### 5.1 Testes Unitários
- [ ] Testar cálculos SAC
- [ ] Testar cálculos PRICE
- [ ] Testar validações
- [ ] Testar modelos de dados

### 5.2 Testes de Integração
- [ ] Testar conexão com banco de dados
- [ ] Testar integração com EventHub
- [ ] Testar endpoints da API

### 5.3 Testes de Performance
- [ ] Testar tempo de resposta
- [ ] Testar carga de requisições
- [ ] Validar telemetria

---

## 📦 6. Containerização

### 6.1 Docker
- [ ] Criar Dockerfile
- [ ] Criar docker-compose.yml
- [ ] Configurar variáveis de ambiente
- [ ] Testar execução em container

---

## 📚 7. Documentação

### 7.1 Documentação da API
- [ ] Documentar endpoints com Swagger/OpenAPI
- [ ] Incluir exemplos de request/response
- [ ] Documentar códigos de erro

### 7.2 Documentação do Projeto
- [ ] README com instruções de execução
- [ ] Documentação da arquitetura
- [ ] Guia de configuração

---

## 🚀 8. Entrega

### 8.1 Código Fonte
- [ ] Código fonte completo
- [ ] Estrutura organizada
- [ ] Comentários no código
- [ ] Versionamento Git

### 8.2 Arquivos de Execução
- [ ] Dockerfile funcional
- [ ] docker-compose.yml
- [ ] Scripts de inicialização
- [ ] Arquivo de configuração

### 8.3 Evidências
- [ ] Screenshots da API funcionando
- [ ] Logs de execução
- [ ] Resultados dos testes
- [ ] Arquivo ZIP com todo o projeto

---

## 📖 Referências

1. **API REST:** http://www.redhat.com/pt-br/topics/api/what-is-a-rest-api
2. **Calculadora SAC e Price:** https://calculojuridico.com.br/calculadora-price-sac/
3. **EventHub:** https://learn.microsoft.com/pt-br/azure/event-hubs/event-hubs-about
4. **SQL Server:** https://learn.microsoft.com/pt-br/sql/sql-server/?view=sql-server-ver16

---

## ✅ Status do Projeto

- [ ] **Configuração da Infraestrutura** (0/3)
- [ ] **Desenvolvimento da API** (0/8)
- [ ] **Lógica de Negócio** (0/2)
- [ ] **Integrações** (0/2)
- [ ] **Testes** (0/3)
- [ ] **Containerização** (0/1)
- [ ] **Documentação** (0/2)
- [ ] **Entrega** (0/3)

**Progresso Total: 0/22 seções concluídas**
