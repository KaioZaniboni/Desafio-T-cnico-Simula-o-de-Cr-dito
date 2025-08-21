using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulacaoCredito.Models;

/// <summary>
/// Entidade que representa uma simulação de crédito realizada
/// </summary>
[Table("SIMULACAO", Schema = "dbo")]
public class Simulacao
{
    /// <summary>
    /// Identificador único da simulação
    /// </summary>
    [Key]
    [Column("ID_SIMULACAO")]
    public long IdSimulacao { get; set; }

    /// <summary>
    /// Código do produto utilizado na simulação
    /// </summary>
    [Column("CO_PRODUTO")]
    [Required]
    public int CodigoProduto { get; set; }

    /// <summary>
    /// Valor desejado pelo cliente
    /// </summary>
    [Column("VR_DESEJADO", TypeName = "numeric(18,2)")]
    [Required]
    public decimal ValorDesejado { get; set; }

    /// <summary>
    /// Prazo em meses solicitado
    /// </summary>
    [Column("NU_PRAZO_MESES")]
    [Required]
    public int PrazoMeses { get; set; }

    /// <summary>
    /// Taxa de juros aplicada na simulação
    /// </summary>
    [Column("PC_TAXA_JUROS", TypeName = "numeric(18,9)")]
    [Required]
    public decimal TaxaJuros { get; set; }

    /// <summary>
    /// Data e hora da simulação
    /// </summary>
    [Column("DT_SIMULACAO")]
    [Required]
    public DateTime DataSimulacao { get; set; }

    /// <summary>
    /// Tipo de amortização utilizada (SAC ou PRICE)
    /// </summary>
    [Column("TP_AMORTIZACAO")]
    [Required]
    [MaxLength(10)]
    public string TipoAmortizacao { get; set; } = string.Empty;

    /// <summary>
    /// Valor total das parcelas
    /// </summary>
    [Column("VR_TOTAL_PARCELAS", TypeName = "numeric(18,2)")]
    [Required]
    public decimal ValorTotalParcelas { get; set; }

    /// <summary>
    /// Valor total dos juros
    /// </summary>
    [Column("VR_TOTAL_JUROS", TypeName = "numeric(18,2)")]
    [Required]
    public decimal ValorTotalJuros { get; set; }

    /// <summary>
    /// Percentual de sucesso da simulação
    /// </summary>
    [Column("PC_SUCESSO", TypeName = "numeric(5,2)")]
    [Required]
    public decimal PercentualSucesso { get; set; }

    /// <summary>
    /// Navegação para o produto
    /// </summary>
    [ForeignKey(nameof(CodigoProduto))]
    public virtual Produto? Produto { get; set; }

    /// <summary>
    /// Lista de parcelas da simulação
    /// </summary>
    public virtual ICollection<ParcelaSimulacao> Parcelas { get; set; } = new List<ParcelaSimulacao>();

    /// <summary>
    /// Construtor padrão
    /// </summary>
    public Simulacao()
    {
        DataSimulacao = DateTime.UtcNow;
        PercentualSucesso = 0.98m; // Valor padrão conforme documentação
    }

    /// <summary>
    /// Construtor com parâmetros principais
    /// </summary>
    /// <param name="codigoProduto">Código do produto</param>
    /// <param name="valorDesejado">Valor desejado</param>
    /// <param name="prazoMeses">Prazo em meses</param>
    /// <param name="taxaJuros">Taxa de juros</param>
    /// <param name="tipoAmortizacao">Tipo de amortização</param>
    public Simulacao(int codigoProduto, decimal valorDesejado, int prazoMeses, decimal taxaJuros, TipoAmortizacao tipoAmortizacao)
        : this()
    {
        CodigoProduto = codigoProduto;
        ValorDesejado = valorDesejado;
        PrazoMeses = prazoMeses;
        TaxaJuros = taxaJuros;
        TipoAmortizacao = tipoAmortizacao.ToString();
    }

    /// <summary>
    /// Adiciona uma parcela à simulação
    /// </summary>
    /// <param name="parcela">Parcela a ser adicionada</param>
    public void AdicionarParcela(ParcelaSimulacao parcela)
    {
        parcela.IdSimulacao = IdSimulacao;
        Parcelas.Add(parcela);
    }

    /// <summary>
    /// Calcula o valor total das parcelas
    /// </summary>
    public void CalcularTotais()
    {
        ValorTotalParcelas = Parcelas.Sum(p => p.ValorPrestacao);
        ValorTotalJuros = Parcelas.Sum(p => p.ValorJuros);
    }

    /// <summary>
    /// Gera um ID único para a simulação baseado na data atual
    /// </summary>
    /// <returns>ID único da simulação</returns>
    public static long GerarIdSimulacao()
    {
        // Formato: YYYYMMDDHHMM (ano, mês, dia, hora, minuto)
        var agora = DateTime.Now;
        return long.Parse($"{agora:yyyyMMddHHmm}");
    }
}
