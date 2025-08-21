using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Models.DTOs;
using SimulacaoCredito.Services;
using System.ComponentModel.DataAnnotations;

namespace SimulacaoCredito.Controllers;

/// <summary>
/// Controller para listagem de simulações
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SimulacoesController : ControllerBase
{
    private readonly SimulacaoService _simulacaoService;
    private readonly ILogger<SimulacoesController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public SimulacoesController(SimulacaoService simulacaoService, ILogger<SimulacoesController> logger)
    {
        _simulacaoService = simulacaoService ?? throw new ArgumentNullException(nameof(simulacaoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Lista simulações com paginação
    /// </summary>
    /// <param name="pagina">Número da página (padrão: 1)</param>
    /// <param name="tamanhoPagina">Tamanho da página (padrão: 200, máximo: 1000)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de simulações</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ListaSimulacoesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListaSimulacoesResponseDto>> ListarSimulacoes(
        [FromQuery] [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior que zero.")] int pagina = 1,
        [FromQuery] [Range(1, 1000, ErrorMessage = "O tamanho da página deve estar entre 1 e 1000.")] int tamanhoPagina = 200,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Listando simulações - Página: {Pagina}, Tamanho: {TamanhoPagina}", 
                pagina, tamanhoPagina);

            var (simulacoes, totalRegistros) = await _simulacaoService.ObterSimulacoesPaginadasAsync(
                pagina, 
                tamanhoPagina, 
                cancellationToken);

            var response = new ListaSimulacoesResponseDto
            {
                Pagina = pagina,
                QtdRegistros = totalRegistros,
                QtdRegistrosPagina = tamanhoPagina,
                Registros = simulacoes.Select(s => new SimulacaoResumoDto
                {
                    IdSimulacao = s.IdSimulacao,
                    CodigoProduto = s.CodigoProduto,
                    NomeProduto = s.Produto?.NomeProduto ?? "N/A",
                    ValorDesejado = s.ValorDesejado,
                    PrazoMeses = s.PrazoMeses,
                    TaxaJuros = s.TaxaJuros,
                    TipoAmortizacao = s.TipoAmortizacao,
                    ValorTotalParcelas = s.ValorTotalParcelas,
                    DataSimulacao = s.DataSimulacao
                }).ToList()
            };

            _logger.LogInformation("Retornando {Quantidade} simulações de um total de {Total}", 
                response.Registros.Count, totalRegistros);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar simulações");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Erro interno do servidor",
                Detail = "Ocorreu um erro inesperado ao listar as simulações.",
                Status = 500
            });
        }
    }
}
