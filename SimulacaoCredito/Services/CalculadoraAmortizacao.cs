using SimulacaoCredito.Models;

namespace SimulacaoCredito.Services;

/// <summary>
/// Serviço para cálculo de amortização SAC e PRICE
/// </summary>
public class CalculadoraAmortizacao
{
    /// <summary>
    /// Calcula as parcelas usando o sistema SAC (Sistema de Amortização Constante)
    /// </summary>
    /// <param name="valorFinanciado">Valor a ser financiado</param>
    /// <param name="taxaJurosMensal">Taxa de juros mensal (decimal)</param>
    /// <param name="numeroParcelas">Número de parcelas</param>
    /// <returns>Lista de parcelas calculadas</returns>
    public List<ParcelaResultado> CalcularSAC(decimal valorFinanciado, decimal taxaJurosMensal, int numeroParcelas)
    {
        if (!ValidarParametros(valorFinanciado, taxaJurosMensal, numeroParcelas))
        {
            return new List<ParcelaResultado>();
        }

        var parcelas = new List<ParcelaResultado>();
        var amortizacaoConstante = valorFinanciado / numeroParcelas;
        var saldoDevedor = valorFinanciado;

        for (int i = 1; i <= numeroParcelas; i++)
        {
            // No SAC, a amortização é constante
            var valorAmortizacao = amortizacaoConstante;
            
            // Os juros são calculados sobre o saldo devedor atual
            var valorJuros = saldoDevedor * taxaJurosMensal;
            
            // Atualizar saldo devedor
            saldoDevedor -= valorAmortizacao;
            
            // Garantir que o saldo não fique negativo devido a arredondamentos
            if (saldoDevedor < 0)
            {
                valorAmortizacao += saldoDevedor; // Ajustar a última amortização
                saldoDevedor = 0;
            }

            var parcela = new ParcelaResultado(i, valorAmortizacao, valorJuros, saldoDevedor);
            parcelas.Add(parcela);
        }

        return parcelas;
    }

    /// <summary>
    /// Calcula as parcelas usando a Tabela Price
    /// </summary>
    /// <param name="valorFinanciado">Valor a ser financiado</param>
    /// <param name="taxaJurosMensal">Taxa de juros mensal (decimal)</param>
    /// <param name="numeroParcelas">Número de parcelas</param>
    /// <returns>Lista de parcelas calculadas</returns>
    public List<ParcelaResultado> CalcularPrice(decimal valorFinanciado, decimal taxaJurosMensal, int numeroParcelas)
    {
        if (!ValidarParametros(valorFinanciado, taxaJurosMensal, numeroParcelas))
        {
            return new List<ParcelaResultado>();
        }

        var parcelas = new List<ParcelaResultado>();
        
        // Calcular prestação fixa usando a fórmula da Tabela Price
        // PMT = PV * [(1 + i)^n * i] / [(1 + i)^n - 1]
        var fatorJuros = (decimal)Math.Pow((double)(1 + taxaJurosMensal), numeroParcelas);
        var prestacaoFixa = valorFinanciado * (fatorJuros * taxaJurosMensal) / (fatorJuros - 1);
        
        var saldoDevedor = valorFinanciado;

        for (int i = 1; i <= numeroParcelas; i++)
        {
            // Os juros são calculados sobre o saldo devedor atual
            var valorJuros = saldoDevedor * taxaJurosMensal;
            
            // A amortização é a diferença entre a prestação fixa e os juros
            var valorAmortizacao = prestacaoFixa - valorJuros;
            
            // Atualizar saldo devedor
            saldoDevedor -= valorAmortizacao;
            
            // Ajustar a última parcela para zerar o saldo devedor
            if (i == numeroParcelas && saldoDevedor != 0)
            {
                valorAmortizacao += saldoDevedor;
                saldoDevedor = 0;
            }
            
            // Garantir que o saldo não fique negativo
            if (saldoDevedor < 0)
            {
                valorAmortizacao += saldoDevedor;
                saldoDevedor = 0;
            }

            var parcela = new ParcelaResultado(i, valorAmortizacao, valorJuros, saldoDevedor);
            parcelas.Add(parcela);
        }

        return parcelas;
    }

    /// <summary>
    /// Calcula as parcelas usando o tipo de amortização especificado
    /// </summary>
    /// <param name="tipoAmortizacao">Tipo de amortização</param>
    /// <param name="valorFinanciado">Valor a ser financiado</param>
    /// <param name="taxaJurosMensal">Taxa de juros mensal (decimal)</param>
    /// <param name="numeroParcelas">Número de parcelas</param>
    /// <returns>Lista de parcelas calculadas</returns>
    public List<ParcelaResultado> Calcular(TipoAmortizacao tipoAmortizacao, decimal valorFinanciado, decimal taxaJurosMensal, int numeroParcelas)
    {
        return tipoAmortizacao switch
        {
            TipoAmortizacao.SAC => CalcularSAC(valorFinanciado, taxaJurosMensal, numeroParcelas),
            TipoAmortizacao.PRICE => CalcularPrice(valorFinanciado, taxaJurosMensal, numeroParcelas),
            _ => throw new ArgumentException($"Tipo de amortização '{tipoAmortizacao}' não é suportado.")
        };
    }

    /// <summary>
    /// Valida se os parâmetros de cálculo são válidos
    /// </summary>
    /// <param name="valorFinanciado">Valor a ser financiado</param>
    /// <param name="taxaJurosMensal">Taxa de juros mensal</param>
    /// <param name="numeroParcelas">Número de parcelas</param>
    /// <returns>True se os parâmetros são válidos</returns>
    public bool ValidarParametros(decimal valorFinanciado, decimal taxaJurosMensal, int numeroParcelas)
    {
        return valorFinanciado > 0 && 
               taxaJurosMensal >= 0 && 
               numeroParcelas > 0 && 
               numeroParcelas <= 1200; // Limite máximo de 100 anos
    }
}

/// <summary>
/// Classe que representa uma parcela no resultado da simulação
/// </summary>
public class ParcelaResultado
{
    /// <summary>
    /// Número da parcela
    /// </summary>
    public int Numero { get; }

    /// <summary>
    /// Valor da amortização
    /// </summary>
    public decimal ValorAmortizacao { get; }

    /// <summary>
    /// Valor dos juros
    /// </summary>
    public decimal ValorJuros { get; }

    /// <summary>
    /// Valor da prestação
    /// </summary>
    public decimal ValorPrestacao { get; }

    /// <summary>
    /// Saldo devedor após o pagamento
    /// </summary>
    public decimal SaldoDevedor { get; }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="numero">Número da parcela</param>
    /// <param name="valorAmortizacao">Valor da amortização</param>
    /// <param name="valorJuros">Valor dos juros</param>
    /// <param name="saldoDevedor">Saldo devedor</param>
    public ParcelaResultado(int numero, decimal valorAmortizacao, decimal valorJuros, decimal saldoDevedor)
    {
        Numero = numero;
        ValorAmortizacao = Math.Round(valorAmortizacao, 2, MidpointRounding.AwayFromZero);
        ValorJuros = Math.Round(valorJuros, 2, MidpointRounding.AwayFromZero);
        ValorPrestacao = Math.Round(ValorAmortizacao + ValorJuros, 2, MidpointRounding.AwayFromZero);
        SaldoDevedor = Math.Round(saldoDevedor, 2, MidpointRounding.AwayFromZero);
    }
}
