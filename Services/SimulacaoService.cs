using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Data;
using SimulacaoCredito.Models;

namespace SimulacaoCredito.Services;

/// <summary>
/// Serviço principal para simulações de crédito
/// </summary>
public class SimulacaoService
{
    private readonly SimulacaoCreditoDbContext _context;
    private readonly CalculadoraAmortizacao _calculadora;
    private readonly ILogger<SimulacaoService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public SimulacaoService(
        SimulacaoCreditoDbContext context,
        CalculadoraAmortizacao calculadora,
        ILogger<SimulacaoService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _calculadora = calculadora ?? throw new ArgumentNullException(nameof(calculadora));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Realiza uma simulação de crédito
    /// </summary>
    /// <param name="valorDesejado">Valor desejado</param>
    /// <param name="prazoMeses">Prazo em meses</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da simulação</returns>
    public async Task<ResultadoSimulacaoService> RealizarSimulacaoAsync(decimal valorDesejado, int prazoMeses, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar parâmetros básicos
            var validacao = ValidarParametros(valorDesejado, prazoMeses);
            if (!validacao.Sucesso)
            {
                return ResultadoSimulacaoService.CriarErro(validacao.Erros);
            }

            // Encontrar produto adequado
            var produto = await ObterProdutoAdequadoAsync(valorDesejado, prazoMeses, cancellationToken);
            if (produto == null)
            {
                return ResultadoSimulacaoService.CriarErro(new[] { "Nenhum produto disponível para os parâmetros informados." });
            }

            // Determinar tipo de amortização (SAC por padrão)
            var tipoAmortizacao = TipoAmortizacao.SAC;

            // Calcular simulação
            var parcelas = _calculadora.Calcular(tipoAmortizacao, valorDesejado, produto.TaxaJuros, prazoMeses);

            // Criar entidade simulação
            var simulacao = new Simulacao(produto.CodigoProduto, valorDesejado, prazoMeses, produto.TaxaJuros, tipoAmortizacao);
            simulacao.IdSimulacao = Simulacao.GerarIdSimulacao();

            // Adicionar parcelas
            foreach (var parcelaResultado in parcelas)
            {
                var parcela = new ParcelaSimulacao(
                    parcelaResultado.Numero,
                    parcelaResultado.ValorAmortizacao,
                    parcelaResultado.ValorJuros,
                    parcelaResultado.SaldoDevedor);
                
                simulacao.AdicionarParcela(parcela);
            }

            // Calcular totais
            simulacao.CalcularTotais();

            // Persistir simulação
            _context.Simulacoes.Add(simulacao);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Simulação {IdSimulacao} criada com sucesso", simulacao.IdSimulacao);

            return ResultadoSimulacaoService.CriarSucesso(simulacao, produto, parcelas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar simulação");
            return ResultadoSimulacaoService.CriarErro(new[] { "Erro interno ao processar simulação." });
        }
    }

    /// <summary>
    /// Obtém simulações com paginação
    /// </summary>
    /// <param name="pagina">Número da página</param>
    /// <param name="tamanhoPagina">Tamanho da página</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de simulações</returns>
    public async Task<(IEnumerable<Simulacao> Simulacoes, int TotalRegistros)> ObterSimulacoesPaginadasAsync(
        int pagina, 
        int tamanhoPagina, 
        CancellationToken cancellationToken = default)
    {
        if (pagina < 1) pagina = 1;
        if (tamanhoPagina < 1) tamanhoPagina = 10;
        if (tamanhoPagina > 1000) tamanhoPagina = 1000; // Limite máximo

        var query = _context.Simulacoes
            .AsNoTracking()
            .Include(s => s.Produto)
            .OrderByDescending(s => s.DataSimulacao);

        var totalRegistros = await query.CountAsync(cancellationToken);

        var simulacoes = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return (simulacoes, totalRegistros);
    }

    /// <summary>
    /// Obtém volume simulado por produto e dia
    /// </summary>
    /// <param name="dataReferencia">Data de referência</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Volume por produto</returns>
    public async Task<IEnumerable<VolumeSimuladoPorProduto>> ObterVolumePorProdutoAsync(
        DateTime dataReferencia, 
        CancellationToken cancellationToken = default)
    {
        var dataInicio = dataReferencia.Date;
        var dataFim = dataInicio.AddDays(1);

        return await _context.Simulacoes
            .AsNoTracking()
            .Include(s => s.Produto)
            .Where(s => s.DataSimulacao >= dataInicio && s.DataSimulacao < dataFim)
            .GroupBy(s => new { s.CodigoProduto, s.Produto!.NomeProduto })
            .Select(g => new VolumeSimuladoPorProduto
            {
                CodigoProduto = g.Key.CodigoProduto,
                DescricaoProduto = g.Key.NomeProduto,
                TaxaMediaJuro = g.Average(s => s.TaxaJuros),
                ValorMedioPrestacao = g.Average(s => s.ValorTotalParcelas / s.PrazoMeses),
                ValorTotalDesejado = g.Sum(s => s.ValorDesejado),
                ValorTotalCredito = g.Sum(s => s.ValorTotalParcelas),
                QuantidadeSimulacoes = g.Count()
            })
            .OrderBy(v => v.CodigoProduto)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtém produto adequado para os parâmetros
    /// </summary>
    private async Task<Produto?> ObterProdutoAdequadoAsync(decimal valorDesejado, int prazoMeses, CancellationToken cancellationToken)
    {
        return await _context.Produtos
            .AsNoTracking()
            .Where(p => 
                valorDesejado >= p.ValorMinimo &&
                (p.ValorMaximo == null || valorDesejado <= p.ValorMaximo) &&
                prazoMeses >= p.MinimoMeses &&
                (p.MaximoMeses == null || prazoMeses <= p.MaximoMeses))
            .OrderBy(p => p.TaxaJuros) // Primeiro com menor taxa de juros
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Valida parâmetros básicos
    /// </summary>
    private static ResultadoValidacao ValidarParametros(decimal valorDesejado, int prazoMeses)
    {
        var erros = new List<string>();

        if (valorDesejado <= 0)
            erros.Add("O valor desejado deve ser maior que zero.");

        if (valorDesejado > 100_000_000)
            erros.Add("O valor desejado não pode ser superior a R$ 100.000.000,00.");

        if (prazoMeses <= 0)
            erros.Add("O prazo deve ser maior que zero.");

        if (prazoMeses > 1200)
            erros.Add("O prazo não pode ser superior a 1200 meses (100 anos).");

        return erros.Any() ? ResultadoValidacao.CriarErros(erros) : ResultadoValidacao.CriarSucesso();
    }
}

/// <summary>
/// Resultado do serviço de simulação
/// </summary>
public class ResultadoSimulacaoService
{
    public bool Sucesso { get; private set; }
    public IReadOnlyList<string> Erros { get; private set; } = new List<string>();
    public Simulacao? Simulacao { get; private set; }
    public Produto? Produto { get; private set; }
    public List<ParcelaResultado>? Parcelas { get; private set; }

    private ResultadoSimulacaoService() { }

    public static ResultadoSimulacaoService CriarSucesso(Simulacao simulacao, Produto produto, List<ParcelaResultado> parcelas)
    {
        return new ResultadoSimulacaoService
        {
            Sucesso = true,
            Simulacao = simulacao,
            Produto = produto,
            Parcelas = parcelas
        };
    }

    public static ResultadoSimulacaoService CriarErro(IEnumerable<string> erros)
    {
        return new ResultadoSimulacaoService
        {
            Sucesso = false,
            Erros = erros.ToList().AsReadOnly()
        };
    }
}

/// <summary>
/// Resultado de validação
/// </summary>
public class ResultadoValidacao
{
    public bool Sucesso { get; }
    public IReadOnlyList<string> Erros { get; }

    public ResultadoValidacao()
    {
        Sucesso = true;
        Erros = new List<string>().AsReadOnly();
    }

    public ResultadoValidacao(IEnumerable<string> erros)
    {
        Sucesso = false;
        Erros = erros.ToList().AsReadOnly();
    }

    public static ResultadoValidacao CriarSucesso() => new();
    public static ResultadoValidacao CriarErros(IEnumerable<string> erros) => new(erros);
}

/// <summary>
/// Volume simulado por produto
/// </summary>
public class VolumeSimuladoPorProduto
{
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public decimal TaxaMediaJuro { get; set; }
    public decimal ValorMedioPrestacao { get; set; }
    public decimal ValorTotalDesejado { get; set; }
    public decimal ValorTotalCredito { get; set; }
    public int QuantidadeSimulacoes { get; set; }
}
