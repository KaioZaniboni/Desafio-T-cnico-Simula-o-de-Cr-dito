namespace SimulacaoCredito.Models.DTOs;

/// <summary>
/// DTO para resposta de simulação de crédito
/// </summary>
public class SimulacaoResponseDto
{
    /// <summary>
    /// ID único da simulação
    /// </summary>
    public long IdSimulacao { get; set; }

    /// <summary>
    /// Código do produto utilizado
    /// </summary>
    public int CodigoProduto { get; set; }

    /// <summary>
    /// Descrição do produto
    /// </summary>
    public string DescricaoProduto { get; set; } = string.Empty;

    /// <summary>
    /// Taxa de juros aplicada
    /// </summary>
    public decimal TaxaJuros { get; set; }

    /// <summary>
    /// Resultado da simulação
    /// </summary>
    public ResultadoSimulacaoDto ResultadoSimulacao { get; set; } = new();

    /// <summary>
    /// Percentual de sucesso da simulação
    /// </summary>
    public decimal PercentualSucesso { get; set; }
}

/// <summary>
/// DTO para resultado da simulação
/// </summary>
public class ResultadoSimulacaoDto
{
    /// <summary>
    /// Tipo de amortização (SAC ou PRICE)
    /// </summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// Lista de parcelas
    /// </summary>
    public List<ParcelaDto> Parcelas { get; set; } = new();
}

/// <summary>
/// DTO para parcela da simulação
/// </summary>
public class ParcelaDto
{
    /// <summary>
    /// Número da parcela
    /// </summary>
    public int Numero { get; set; }

    /// <summary>
    /// Valor da amortização
    /// </summary>
    public decimal ValorAmortizacao { get; set; }

    /// <summary>
    /// Valor dos juros
    /// </summary>
    public decimal ValorJuros { get; set; }

    /// <summary>
    /// Valor da prestação
    /// </summary>
    public decimal ValorPrestacao { get; set; }
}
