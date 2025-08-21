using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Data;
using SimulacaoCredito.Models;
using SimulacaoCredito.Models.DTOs;
using SimulacaoCredito.Services;
using System.ComponentModel.DataAnnotations;

namespace SimulacaoCredito.Controllers;

/// <summary>
/// Controller para simulações de crédito
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SimulacaoController : ControllerBase
{
    private readonly SimulacaoCreditoDbContext _context;
    private readonly CalculadoraAmortizacao _calculadora;
    private readonly ILogger<SimulacaoController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public SimulacaoController(
        SimulacaoCreditoDbContext context,
        CalculadoraAmortizacao calculadora,
        ILogger<SimulacaoController> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _calculadora = calculadora ?? throw new ArgumentNullException(nameof(calculadora));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Realiza uma simulação de empréstimo
    /// </summary>
    /// <param name="request">Dados da simulação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da simulação</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SimulacaoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SimulacaoResponseDto>> RealizarSimulacao(
        [FromBody] SimulacaoRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando simulação para valor {Valor} e prazo {Prazo} meses", 
                request.ValorDesejado, request.Prazo);

            // Validar parâmetros básicos
            if (request.ValorDesejado <= 0 || request.Prazo <= 0)
            {
                var problemDetails = new ValidationProblemDetails();
                if (request.ValorDesejado <= 0)
                    problemDetails.Errors.Add("ValorDesejado", new[] { "O valor desejado deve ser maior que zero." });
                if (request.Prazo <= 0)
                    problemDetails.Errors.Add("Prazo", new[] { "O prazo deve ser maior que zero." });
                
                return BadRequest(problemDetails);
            }

            // Buscar produto adequado na tabela dbo.Produto
            var produto = await _context.Produtos
                .AsNoTracking()
                .Where(p => 
                    request.ValorDesejado >= p.ValorMinimo &&
                    (p.ValorMaximo == null || request.ValorDesejado <= p.ValorMaximo) &&
                    request.Prazo >= p.MinimoMeses &&
                    (p.MaximoMeses == null || request.Prazo <= p.MaximoMeses))
                .OrderBy(p => p.TaxaJuros) // Primeiro com menor taxa de juros
                .FirstOrDefaultAsync(cancellationToken);

            if (produto == null)
            {
                var problemDetails = new ValidationProblemDetails();
                problemDetails.Errors.Add("simulacao", new[] { "Nenhum produto disponível para os parâmetros informados." });
                return BadRequest(problemDetails);
            }

            // Calcular simulação usando SAC
            var tipoAmortizacao = TipoAmortizacao.SAC;
            var parcelas = _calculadora.Calcular(tipoAmortizacao, request.ValorDesejado, produto.TaxaJuros, request.Prazo);

            // Gerar ID único para a simulação
            var idSimulacao = long.Parse($"{DateTime.Now:yyyyMMddHHmmss}");

            var response = new SimulacaoResponseDto
            {
                IdSimulacao = idSimulacao,
                CodigoProduto = produto.CodigoProduto,
                DescricaoProduto = produto.NomeProduto,
                TaxaJuros = produto.TaxaJuros,
                PercentualSucesso = 0.98m, // Valor padrão conforme documentação
                ResultadoSimulacao = new ResultadoSimulacaoDto
                {
                    Tipo = tipoAmortizacao.ToString(),
                    Parcelas = parcelas.Select(p => new ParcelaDto
                    {
                        Numero = p.Numero,
                        ValorAmortizacao = p.ValorAmortizacao,
                        ValorJuros = p.ValorJuros,
                        ValorPrestacao = p.ValorPrestacao
                    }).ToList()
                }
            };

            _logger.LogInformation("Simulação {IdSimulacao} realizada com sucesso", response.IdSimulacao);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar simulação");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Erro interno do servidor",
                Detail = "Ocorreu um erro inesperado ao processar a simulação.",
                Status = 500
            });
        }
    }
}
