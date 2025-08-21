using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulacaoCredito.Models;

/// <summary>
/// Entidade que representa uma parcela de uma simulação de crédito
/// </summary>
[Table("PARCELA_SIMULACAO", Schema = "dbo")]
public class ParcelaSimulacao
{
    /// <summary>
    /// Identificador único da parcela
    /// </summary>
    [Key]
    [Column("ID_PARCELA")]
    public long IdParcela { get; set; }

    /// <summary>
    /// Identificador da simulação à qual a parcela pertence
    /// </summary>
    [Column("ID_SIMULACAO")]
    [Required]
    public long IdSimulacao { get; set; }

    /// <summary>
    /// Número da parcela (1, 2, 3, ...)
    /// </summary>
    [Column("NU_PARCELA")]
    [Required]
    public int NumeroParcela { get; set; }

    /// <summary>
    /// Valor da amortização da parcela
    /// </summary>
    [Column("VR_AMORTIZACAO", TypeName = "numeric(18,2)")]
    [Required]
    public decimal ValorAmortizacao { get; set; }

    /// <summary>
    /// Valor dos juros da parcela
    /// </summary>
    [Column("VR_JUROS", TypeName = "numeric(18,2)")]
    [Required]
    public decimal ValorJuros { get; set; }

    /// <summary>
    /// Valor total da prestação (amortização + juros)
    /// </summary>
    [Column("VR_PRESTACAO", TypeName = "numeric(18,2)")]
    [Required]
    public decimal ValorPrestacao { get; set; }

    /// <summary>
    /// Saldo devedor após o pagamento desta parcela
    /// </summary>
    [Column("VR_SALDO_DEVEDOR", TypeName = "numeric(18,2)")]
    [Required]
    public decimal SaldoDevedor { get; set; }

    /// <summary>
    /// Navegação para a simulação
    /// </summary>
    [ForeignKey(nameof(IdSimulacao))]
    public virtual Simulacao? Simulacao { get; set; }

    /// <summary>
    /// Construtor padrão
    /// </summary>
    public ParcelaSimulacao()
    {
    }

    /// <summary>
    /// Construtor com parâmetros
    /// </summary>
    /// <param name="numeroParcela">Número da parcela</param>
    /// <param name="valorAmortizacao">Valor da amortização</param>
    /// <param name="valorJuros">Valor dos juros</param>
    /// <param name="saldoDevedor">Saldo devedor após pagamento</param>
    public ParcelaSimulacao(int numeroParcela, decimal valorAmortizacao, decimal valorJuros, decimal saldoDevedor)
    {
        NumeroParcela = numeroParcela;
        ValorAmortizacao = valorAmortizacao;
        ValorJuros = valorJuros;
        ValorPrestacao = valorAmortizacao + valorJuros;
        SaldoDevedor = saldoDevedor;
    }

    /// <summary>
    /// Atualiza o valor da prestação baseado na amortização e juros
    /// </summary>
    public void CalcularPrestacao()
    {
        ValorPrestacao = ValorAmortizacao + ValorJuros;
    }

    /// <summary>
    /// Valida se os valores da parcela estão consistentes
    /// </summary>
    /// <returns>True se os valores estão válidos</returns>
    public bool ValidarParcela()
    {
        // Verifica se a prestação é igual à soma de amortização + juros
        var prestacaoCalculada = ValorAmortizacao + ValorJuros;
        var diferenca = Math.Abs(ValorPrestacao - prestacaoCalculada);
        
        // Permite uma diferença mínima devido a arredondamentos
        return diferenca <= 0.01m;
    }
}
