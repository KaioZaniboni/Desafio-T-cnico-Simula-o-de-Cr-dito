namespace SimulacaoCredito.Models;

/// <summary>
/// Enumeração que representa os tipos de amortização disponíveis
/// </summary>
public enum TipoAmortizacao
{
    /// <summary>
    /// Sistema de Amortização Constante - Amortização fixa, juros decrescentes
    /// </summary>
    SAC,

    /// <summary>
    /// Tabela Price - Prestação fixa, amortização crescente
    /// </summary>
    PRICE
}

/// <summary>
/// Extensões para o enum TipoAmortizacao
/// </summary>
public static class TipoAmortizacaoExtensions
{
    /// <summary>
    /// Converte string para TipoAmortizacao
    /// </summary>
    /// <param name="tipo">String representando o tipo</param>
    /// <returns>TipoAmortizacao correspondente</returns>
    /// <exception cref="ArgumentException">Quando o tipo não é reconhecido</exception>
    public static TipoAmortizacao FromString(string tipo)
    {
        return tipo.ToUpper() switch
        {
            "SAC" => TipoAmortizacao.SAC,
            "PRICE" => TipoAmortizacao.PRICE,
            _ => throw new ArgumentException($"Tipo de amortização '{tipo}' não é válido. Use 'SAC' ou 'PRICE'.")
        };
    }

    /// <summary>
    /// Obtém a descrição do tipo de amortização
    /// </summary>
    /// <param name="tipo">Tipo de amortização</param>
    /// <returns>Descrição do tipo</returns>
    public static string GetDescricao(this TipoAmortizacao tipo)
    {
        return tipo switch
        {
            TipoAmortizacao.SAC => "Sistema de Amortização Constante",
            TipoAmortizacao.PRICE => "Tabela Price",
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };
    }

    /// <summary>
    /// Verifica se o tipo é válido
    /// </summary>
    /// <param name="tipo">String a ser validada</param>
    /// <returns>True se o tipo é válido</returns>
    public static bool IsValid(string tipo)
    {
        return tipo.ToUpper() is "SAC" or "PRICE";
    }
}
