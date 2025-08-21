using Microsoft.AspNetCore.Mvc;
using SimulacaoCredito.Services;
using System.Globalization;

namespace SimulacaoCredito.Controllers;

/// <summary>
/// Controller para relatórios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RelatorioController : ControllerBase
{
    private readonly SimulacaoService _simulacaoService;
    private readonly ILogger<RelatorioController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioController(SimulacaoService simulacaoService, ILogger<RelatorioController> logger)
    {
        _simulacaoService = simulacaoService ?? throw new ArgumentNullException(nameof(simulacaoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtém volume simulado por produto e dia
    /// </summary>
    /// <param name="dataReferencia">Data de referência no formato YYYY-MM-DD (opcional, padrão: hoje)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Volume simulado por produto</returns>
    [HttpGet("volume-produto-dia")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ObterVolumePorProdutoDia(
        [FromQuery] string? dataReferencia = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar e parsear data de referência
            DateTime data;
            if (string.IsNullOrEmpty(dataReferencia))
            {
                data = DateTime.Today;
            }
            else
            {
                if (!DateTime.TryParseExact(dataReferencia, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
                {
                    var problemDetails = new ValidationProblemDetails();
                    problemDetails.Errors.Add("dataReferencia", new[] { "Data deve estar no formato YYYY-MM-DD." });
                    return BadRequest(problemDetails);
                }
            }

            _logger.LogInformation("Obtendo volume por produto para data {Data}", data.ToString("yyyy-MM-dd"));

            var volumes = await _simulacaoService.ObterVolumePorProdutoAsync(data, cancellationToken);

            var response = new
            {
                DataReferencia = data.ToString("yyyy-MM-dd"),
                Simulacoes = volumes.Select(v => new
                {
                    CodigoProduto = v.CodigoProduto,
                    DescricaoProduto = v.DescricaoProduto,
                    TaxaMediaJuro = Math.Round(v.TaxaMediaJuro, 9),
                    ValorMedioPrestacao = Math.Round(v.ValorMedioPrestacao, 2),
                    ValorTotalDesejado = Math.Round(v.ValorTotalDesejado, 2),
                    ValorTotalCredito = Math.Round(v.ValorTotalCredito, 2)
                }).ToList()
            };

            _logger.LogInformation("Retornando volume para {Quantidade} produtos", response.Simulacoes.Count);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter volume por produto");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Erro interno do servidor",
                Detail = "Ocorreu um erro inesperado ao obter o volume por produto.",
                Status = 500
            });
        }
    }
}
