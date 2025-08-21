using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace SimulacaoCredito.Controllers;

/// <summary>
/// Controller para telemetria da API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TelemetriaController : ControllerBase
{
    private readonly ILogger<TelemetriaController> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public TelemetriaController(ILogger<TelemetriaController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtém dados de telemetria da API
    /// </summary>
    /// <param name="dataReferencia">Data de referência no formato YYYY-MM-DD (opcional, padrão: hoje)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados de telemetria</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ObterTelemetria(
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

            _logger.LogInformation("Obtendo telemetria para data {Data}", data.ToString("yyyy-MM-dd"));

            // TODO: Implementar coleta real de métricas de telemetria
            // Por enquanto, retornando estrutura vazia (dados reais viriam de sistema de monitoramento)
            var response = new
            {
                DataReferencia = data.ToString("yyyy-MM-dd"),
                ListaEndpoints = new object[0] // Array vazio - dados reais viriam de sistema de telemetria
            };

            _logger.LogInformation("Retornando telemetria para {Quantidade} endpoints", response.ListaEndpoints.Length);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter telemetria");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Erro interno do servidor",
                Detail = "Ocorreu um erro inesperado ao obter a telemetria.",
                Status = 500
            });
        }
    }
}
