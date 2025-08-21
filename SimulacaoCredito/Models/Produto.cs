using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulacaoCredito.Models;

/// <summary>
/// Entidade que representa um produto de crédito disponível para simulação
/// </summary>
[Table("Produto", Schema = "dbo")]
public class Produto
{
    /// <summary>
    /// Código único do produto
    /// </summary>
    [Key]
    [Column("CO_PRODUTO")]
    public int CodigoProduto { get; set; }

    /// <summary>
    /// Nome/descrição do produto
    /// </summary>
    [Column("NO_PRODUTO")]
    [Required]
    [MaxLength(200)]
    public string NomeProduto { get; set; } = string.Empty;

    /// <summary>
    /// Taxa de juros mensal do produto (decimal)
    /// </summary>
    [Column("PC_TAXA_JUROS", TypeName = "numeric(18,9)")]
    [Required]
    public decimal TaxaJuros { get; set; }

    /// <summary>
    /// Número mínimo de meses para financiamento
    /// </summary>
    [Column("NU_MINIMO_MESES")]
    [Required]
    public short MinimoMeses { get; set; }

    /// <summary>
    /// Número máximo de meses para financiamento (pode ser null para sem limite)
    /// </summary>
    [Column("NU_MAXIMO_MESES")]
    public short? MaximoMeses { get; set; }

    /// <summary>
    /// Valor mínimo para financiamento
    /// </summary>
    [Column("VR_MINIMO", TypeName = "numeric(18,2)")]
    [Required]
    public decimal ValorMinimo { get; set; }

    /// <summary>
    /// Valor máximo para financiamento (pode ser null para sem limite)
    /// </summary>
    [Column("VR_MAXIMO", TypeName = "numeric(18,2)")]
    public decimal? ValorMaximo { get; set; }

    /// <summary>
    /// Verifica se o valor desejado está dentro dos limites do produto
    /// </summary>
    /// <param name="valorDesejado">Valor a ser validado</param>
    /// <returns>True se o valor está dentro dos limites</returns>
    public bool ValidarValor(decimal valorDesejado)
    {
        if (valorDesejado < ValorMinimo)
            return false;

        if (ValorMaximo.HasValue && valorDesejado > ValorMaximo.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Verifica se o prazo está dentro dos limites do produto
    /// </summary>
    /// <param name="prazoMeses">Prazo em meses a ser validado</param>
    /// <returns>True se o prazo está dentro dos limites</returns>
    public bool ValidarPrazo(int prazoMeses)
    {
        if (prazoMeses < MinimoMeses)
            return false;

        if (MaximoMeses.HasValue && prazoMeses > MaximoMeses.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Verifica se o produto é adequado para os parâmetros informados
    /// </summary>
    /// <param name="valorDesejado">Valor desejado para financiamento</param>
    /// <param name="prazoMeses">Prazo em meses</param>
    /// <returns>True se o produto é adequado</returns>
    public bool EhAdequadoPara(decimal valorDesejado, int prazoMeses)
    {
        return ValidarValor(valorDesejado) && ValidarPrazo(prazoMeses);
    }
}
