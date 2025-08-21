# Conversão do PDF para Markdown (com OCR)

- Arquivo de origem: `desafio.pdf`
- Páginas detectadas: 8
- OCR aplicado nas páginas: 1, 2, 3, 4, 5, 6, 7

---

# Página 1

f=] Back-end

Desafio Técnico - Simulagéo de Crédito

Para realizar o desafio, vocé terd acesso a alguns recursos, e precisard de algumas
referéncias, todos seréo apresentados na sequéncia. O trabalho de pesquisa, leitura e
compreensdo do desafio, além da elaboragdo de proposta de solugdo fazem parte da
avaliagao.

Em um contexto cada vez mais tecnolégico, todas as relagdes — inclusive as bancdrias — tem
se tornado mais frequentes por meio dos canais digitais. Diversas tecnologias, servigos e
processos esto evoluindo para ndo sé acompanhar, mas também para tracionar este
movimento, como nos casos do PIX, das contas digitais, do pagamento de beneficios sociais
e da contratagdo de servigos bancdrios por meio de canais que estdo “na mao do cliente”.

E este o cendrio deste desafio, vocé precisa disponil ar para todos os brasileiros a
possibilidade de simulacao de empréstimo. Por meio dessa solugdo, qualquer pessoa ou
sistema pode descobrir quais sGo as condigées oferecidas para uma negociagao.

Desafio perfil Back-end: API Simulador

Uma das ferramentas mais potentes neste aspecto, e fundamental para disponibilizagdo de
servigos nos canais tradicionais ou digitais, é a API, sigla em inglés para Interface de
Programagdo de Aplicagdo, que estabelece um protocolo para troca de informagées entre
sistemas ou entre processos e camadas de um mesmo sistema.

Vamos desenvolver uma API em linguagem de programagdo Java 17+ ou C# (Dotnet) 8+
que tera como requisitos:

. Receber um envelope JSON, via chamada 4 API, contendo uma solicitagao de
simulagdo de empréstimo.

. Consultar um conjunto de informagées parametrizadas em uma tabela de banco de
dados SQL Server.
. Validar os dados de entrada da API com base nos pardmetros de produtos

retornados no banco de dados.

. Filtrar qual produto se adequa aos pardmetros de entrada.

---

# Página 2

. Realizar os cdlculos para os sistemas de amortizagdéo SAC e PRICE de acordo com
dados validados.

. Retornar um envelope JSON contendo 0 nome do produto validado, e o resultado da
simulag&o utilizando dois sistemas de amortizagao (SAC e Price), gravando este mesmo
envelope JSON no Eventhub. A gravagéo no Eventhub visa simular uma possibilidade de
integragdo com a Grea de relacionamento com o cliente da empresa, que receberia em
poucos segundos este evento de simulacdo, e estaria apta 4 execucdo de estratégia negocial
com base na interagGo do cliente.

. Persistir em banco local a simulagéo realizada.

. Criar um endpoint para retornar todas as simulagées realizadas.

. Criar um endpoint para retornar os valores simulados para cada produto em cada
dia.

. Criar um endpoint para retornar dados de telemetria com volumes e tempos de

resposta para cada servigo.

. Disponibilizar 0 cédigo fonte, com todas as evidéncias no formato zip.
. Incluir no projeto todos os arquivos para execug&o via container (dockerfile / Docker
compose)

Links e Refer€ncias

1. O que é API: https://www.redhat.com/pt-br/topics/api/what-is-a-rest-api
2. Calculadora SAC e Price: https://calculojuridico.com.br/calculadora-price-sac/
3. O que é EventHub: https://learn.microsoft.com/pt-br/azure/event-hubs/event-hubs-
about
4. SQL Server: https://learn.microsoft.com/pt-br/sql/sql-server/?view=sql-server-ver 16
5. Dados para conexdo com banco de dados:

a. URL: dbhackathon.database.windows.net

b. Porta: 1433

c DB: hack

---

# Página 3

d. Login: hack

e. Senha: Password23

f Tabela: dbo.Produto

PRODUTO (dbo)
Column Name Data Type Allow Nulls

% CO_PRODUTO int oO
NO_PRODUTO varchar(200) Oo
PC_TAXA_JUROS numeric(10, 9) oO
NU_MINIMO_MESES smallint o
NU_MAXIMO_MESES smallint
VR_MINIMO numeric(18, 2) oO

VR. MAXIMO numeric(18, 2)

o

CREATE TABLE dbo.PRODUTO (
CO_PRODUTO int NOT NULL primary key,
NO_PRODUTO varchar(20@) NOT NULL,
PC_TAXA_JUROS numeric(1@, 9) NOT NULL,
NU_MINIMO_MESES smallint NOT NULL,
NU_MAXIMO_MESES smallint NULL,
VR_MINIMO numeric(18, 2) NOT NULL,
VR_MAXIMO numeric(18, 2) NULL

)3

INSERT INTO dbo.PRODUTO (CO_PRODUTO, NO_PRODUTO, PC_TAXA_JUROS,
NU_MINIMO_MESES, NU_MAXIMO_MESES, VR_MINIMO, VR_MAXIMO)
VALUES (1, 'Produto 1', @.017900000, @, 24, 200.00, 10000.00)

INSERT INTO dbo.PRODUTO (CO_PRODUTO, NO_PRODUTO, PC_TAXA_JUROS ,
NU_MINIMO_MESES, NU_MAXIMO_MESES, VR_MINIMO, VR_MAXIMO)
VALUES (2, ‘Produto 2', @.017500000, 25, 48, 10001.00, 100000.00)

INSERT INTO dbo.PRODUTO (CO_PRODUTO, NO_PRODUTO, PC_TAXA_JUROS,
NU_MINIMO_MESES, NU_MAXIMO_MESES, VR_MINIMO, VR_MAXIMO)
VALUES (3, ‘Produto 3', @.018200000, 49, 96, 100000.01, 1000000.00)

INSERT INTO dbo.PRODUTO (CO_PRODUTO, NO_PRODUTO, PC_TAXA_JUROS,
NU_MINIMO_MESES, NU_MAXIMO_MESES, VR_MINIMO, VR_MAXIMO)
VALUES (4, 'Produto 4', @.015100000, 96, null, 100¢@00.01, null)

---

# Página 4

2.

1. Arquitetura da Solugéo

Base de dados

AP
‘Seu desenvolvimento

Eventhub

Dados para conexéo com EventHub:

Endpoint=sb://eventhack.servicebus.windows.net/;SharedAccessKeyName=hack;SharedAcce
ssKey=HeHeVaVayVkntO2FnjQcs2Ilh/4MUDo4y+AEhKp8z+g=;EntityPath=simulacoes

3.

Modelo de Envelope para Simulagao:

“idSimulacao": 20180702,

“codigoProduto": 1
"descricaoProduto
“taxaJuros": @.0179
"resultadoSimulacao

[

"tipo":
“parcelas

{

## Descrição estruturada — Diagrama de Arquitetura (best‑effort a partir do OCR)

- 2.
- 1. Arquitetura da Solugéo
- Base de dados
- AP
- ‘Seu desenvolvimento
- Eventhub
- Dados para conexéo com EventHub:
- Endpoint=sb://eventhack.servicebus.windows.net/;SharedAccessKeyName=hack;SharedAcce
- ssKey=HeHeVaVayVkntO2FnjQcs2Ilh/4MUDo4y+AEhKp8z+g=;EntityPath=simulacoes
- 3.
- Modelo de Envelope para Simulagao:
- “idSimulacao": 20180702,
- “codigoProduto": 1
- "descricaoProduto
- “taxaJuros": @.0179
- "resultadoSimulacao
- [
- "tipo":
- “parcelas
- {

---

# Página 5

a 2S

"numero": 1,
“valorAmortizacao": 180.00,
“valorjuros": 16.11,
“valorPrestacao": 196.11

"numero": 2,
“valorAmortizacao": 180.00,
“valorjuros": 12.89,
"valorPrestacao": 192.89

"numero": 3,
“valorAmortizacao": 180.00,
“valorjuros": 9.67,
“valorPrestacao": 189.67

"numero": 4,
"valorAmortizacao": 180.00,
“valorjuros": 6.44,
"valorPrestacao": 186.44

"numero": 5,
“valorAmortizacao": 180.00,
“valorjuros": 3.22,
“valorPrestacao": 183.22

"tipo": "PRICE",
“parcelas": [

{

“numero”: 1,
“valorAmortizacao": 173.67,
“valorjuros": 16.11,

---

# Página 6

a 2S

"valorPrestacao":

"numero": 2,
“valorAmortizacao": 176.78,
valorjuros": 13.00,
"valorPrestacao": 189.78

"numero": 3,
“valorAmortizacao": 179.94,
“valorjuros": 9.84,
"valorPrestacao": 189.78

"numero": 4,
“valorAmortizacao": 183.16,
"valorJuros": 6.62,

"valorPrestacao": 189.78

"numero": 5,
“valorAmortizacao": 186.44,
"valorJuros": 3.34,
"valorPrestacao": 189.78

Modelo de chamada para listar simulagdes

“pagina":1,
"qtdRegistros": 404,
“qtdRegistrosPagina": 200,
“registros": [

---

# Página 7

idSimulaca 20180702,
"valorDesejado": 900.00,
“prazo": 5,

"valorTotalParcelas": 124

3.28

6. Modelo de chamada para retornar o volume simulado por produto e por dia

“dataReferencia":

“simulacoes": [{
"“codigoProduto":1,
"descricaoProduto":
“taxaMediaJuro": 0.189,
"valorMedioPrestacao":
“valorTotalDesejado": 12047.47,
"valorTotalCredito": 16750.00

"dataReferencia
“listaEndpoints

[

“nomeAp
“qtdRequisicoes": 135,
“tempoMedi: 150,
“tempoMinimo 23,
“tempoMaximo": 860,

---

# Página 8

_Sem texto extraível nesta página._