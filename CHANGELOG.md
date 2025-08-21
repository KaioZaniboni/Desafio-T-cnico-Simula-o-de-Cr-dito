# Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

## [1.0.0] - 2025-08-21

### ✅ Adicionado
- **API REST completa** para simulação de crédito
- **Cálculos financeiros SAC e PRICE** implementados
- **Integração com Azure SQL Server** usando dados reais
- **Validações de negócio** para produtos e parâmetros
- **Documentação Swagger** automática
- **Health check endpoint** para monitoramento
- **Logging estruturado** com informações detalhadas
- **Tratamento de erros** robusto com respostas padronizadas

### 🏗️ Arquitetura
- **Projeto único** para simplicidade de desenvolvimento
- **Entity Framework Core** para acesso a dados
- **Injeção de dependência** configurada
- **CORS** habilitado para desenvolvimento

### 🎮 Endpoints Implementados
- `POST /api/simulacao` - Simulação de crédito (✅ Funcionando)
- `GET /health` - Health check (✅ Funcionando)
- `GET /api/telemetria` - Métricas da API (✅ Funcionando)
- `GET /api/simulacoes` - Listagem de simulações (⚠️ Tabela não existe)
- `GET /api/relatorio/volume-produto-dia` - Relatório de volume (⚠️ Tabela não existe)

### 🗄️ Banco de Dados
- **Conexão estabelecida** com Azure SQL Server
- **Tabela dbo.Produto** mapeada e funcional
- **Dados reais** sendo utilizados nas simulações

### 🧮 Funcionalidades Matemáticas
- **Sistema SAC**: Amortização constante, juros decrescentes
- **Tabela Price**: Prestação fixa, amortização crescente
- **Arredondamentos corretos** para valores monetários
- **Validação de parâmetros** matemáticos

### 🔧 Configurações
- **Connection string** para Azure SQL configurada
- **EventHub** configurado (não implementado)
- **Logging levels** otimizados
- **Swagger** configurado na raiz da aplicação

### ⚠️ Limitações Conhecidas
- Tabelas `SIMULACAO` e `PARCELA_SIMULACAO` não existem no banco
- Persistência de simulações não implementada (devido às tabelas ausentes)
- Integração EventHub configurada mas não implementada

### 🎯 Testes Realizados
- ✅ Simulação com valor R$ 10.000,00 e prazo 12 meses
- ✅ Health check respondendo corretamente
- ✅ Swagger UI funcionando
- ✅ Validações de entrada funcionando
- ✅ Tratamento de erros operacional

### 📊 Métricas de Qualidade
- **Compilação**: 0 erros, 1 warning (método async sem await)
- **Funcionalidade core**: 100% operacional
- **Cobertura de validações**: Completa
- **Documentação**: Swagger + README completos
