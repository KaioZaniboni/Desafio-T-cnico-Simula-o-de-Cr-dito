using System.ComponentModel.DataAnnotations;

namespace SimulacaoCredito.Models.DTOs;

/// <summary>
/// DTO para requisição de simulação de crédito
/// </summary>
public class SimulacaoRequestDto
{
    /// <summary>
    /// Valor desejado para financiamento
    /// </summary>
    [Required(ErrorMessage = "O valor desejado é obrigatório.")]
    [Range(0.01, 100_000_000, ErrorMessage = "O valor desejado deve estar entre R$ 0,01 e R$ 100.000.000,00.")]
    public decimal ValorDesejado { get; set; }

    /// <summary>
    /// Prazo em meses para pagamento
    /// </summary>
    [Required(ErrorMessage = "O prazo é obrigatório.")]
    [Range(1, 1200, ErrorMessage = "O prazo deve estar entre 1 e 1200 meses.")]
    public int Prazo { get; set; }
}
