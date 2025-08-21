namespace SimulacaoCredito.Models.DTOs;

/// <summary>
/// DTO para resposta de listagem de simulações
/// </summary>
public class ListaSimulacoesResponseDto
{
    /// <summary>
    /// Número da página atual
    /// </summary>
    public int Pagina { get; set; }

    /// <summary>
    /// Quantidade total de registros
    /// </summary>
    public int QtdRegistros { get; set; }

    /// <summary>
    /// Quantidade de registros por página
    /// </summary>
    public int QtdRegistrosPagina { get; set; }

    /// <summary>
    /// Lista de simulações
    /// </summary>
    public List<SimulacaoResumoDto> Registros { get; set; } = new();
}

/// <summary>
/// DTO para resumo de simulação na listagem
/// </summary>
public class SimulacaoResumoDto
{
    /// <summary>
    /// ID da simulação
    /// </summary>
    public long IdSimulacao { get; set; }

    /// <summary>
    /// Código do produto
    /// </summary>
    public int CodigoProduto { get; set; }

    /// <summary>
    /// Nome do produto
    /// </summary>
    public string NomeProduto { get; set; } = string.Empty;

    /// <summary>
    /// Valor desejado
    /// </summary>
    public decimal ValorDesejado { get; set; }

    /// <summary>
    /// Prazo em meses
    /// </summary>
    public int PrazoMeses { get; set; }

    /// <summary>
    /// Taxa de juros
    /// </summary>
    public decimal TaxaJuros { get; set; }

    /// <summary>
    /// Tipo de amortização
    /// </summary>
    public string TipoAmortizacao { get; set; } = string.Empty;

    /// <summary>
    /// Valor total das parcelas
    /// </summary>
    public decimal ValorTotalParcelas { get; set; }

    /// <summary>
    /// Data da simulação
    /// </summary>
    public DateTime DataSimulacao { get; set; }
}
