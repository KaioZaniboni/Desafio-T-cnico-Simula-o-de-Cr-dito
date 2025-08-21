using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Models;

namespace SimulacaoCredito.Data;

/// <summary>
/// Contexto do Entity Framework para o sistema de simulação de crédito
/// </summary>
public class SimulacaoCreditoDbContext : DbContext
{
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="options">Opções de configuração do contexto</param>
    public SimulacaoCreditoDbContext(DbContextOptions<SimulacaoCreditoDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet para produtos
    /// </summary>
    public DbSet<Produto> Produtos { get; set; } = null!;

    /// <summary>
    /// DbSet para simulações
    /// </summary>
    public DbSet<Simulacao> Simulacoes { get; set; } = null!;

    /// <summary>
    /// DbSet para parcelas de simulação
    /// </summary>
    public DbSet<ParcelaSimulacao> ParcelasSimulacao { get; set; } = null!;

    /// <summary>
    /// Configuração do modelo
    /// </summary>
    /// <param name="modelBuilder">Builder do modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.CodigoProduto);
            
            entity.Property(e => e.NomeProduto)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.TaxaJuros)
                .IsRequired()
                .HasColumnType("numeric(18,9)");

            entity.Property(e => e.ValorMinimo)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.ValorMaximo)
                .HasColumnType("numeric(18,2)");

            // Índices para otimização de consultas
            entity.HasIndex(e => new { e.ValorMinimo, e.ValorMaximo, e.MinimoMeses, e.MaximoMeses })
                .HasDatabaseName("IX_Produto_Parametros");
        });

        // Configuração da entidade Simulacao
        modelBuilder.Entity<Simulacao>(entity =>
        {
            entity.HasKey(e => e.IdSimulacao);

            entity.Property(e => e.ValorDesejado)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.TaxaJuros)
                .IsRequired()
                .HasColumnType("numeric(18,9)");

            entity.Property(e => e.TipoAmortizacao)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.ValorTotalParcelas)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.ValorTotalJuros)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.PercentualSucesso)
                .IsRequired()
                .HasColumnType("numeric(5,2)");

            entity.Property(e => e.DataSimulacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relacionamento com Produto
            entity.HasOne(e => e.Produto)
                .WithMany()
                .HasForeignKey(e => e.CodigoProduto)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com Parcelas
            entity.HasMany(e => e.Parcelas)
                .WithOne(p => p.Simulacao)
                .HasForeignKey(p => p.IdSimulacao)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para otimização
            entity.HasIndex(e => e.DataSimulacao)
                .HasDatabaseName("IX_Simulacao_DataSimulacao");

            entity.HasIndex(e => e.CodigoProduto)
                .HasDatabaseName("IX_Simulacao_CodigoProduto");

            entity.HasIndex(e => new { e.DataSimulacao, e.CodigoProduto })
                .HasDatabaseName("IX_Simulacao_Data_Produto");
        });

        // Configuração da entidade ParcelaSimulacao
        modelBuilder.Entity<ParcelaSimulacao>(entity =>
        {
            entity.HasKey(e => e.IdParcela);

            entity.Property(e => e.ValorAmortizacao)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.ValorJuros)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.ValorPrestacao)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            entity.Property(e => e.SaldoDevedor)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            // Relacionamento com Simulacao
            entity.HasOne(e => e.Simulacao)
                .WithMany(s => s.Parcelas)
                .HasForeignKey(e => e.IdSimulacao)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice para otimização
            entity.HasIndex(e => new { e.IdSimulacao, e.NumeroParcela })
                .HasDatabaseName("IX_ParcelaSimulacao_Simulacao_Numero")
                .IsUnique();
        });

        // Configurações globais
        ConfigurarConvencoesGlobais(modelBuilder);
    }

    /// <summary>
    /// Configurações globais do modelo
    /// </summary>
    /// <param name="modelBuilder">Builder do modelo</param>
    private static void ConfigurarConvencoesGlobais(ModelBuilder modelBuilder)
    {
        // Configurar todas as propriedades string para não serem unicode por padrão
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string))
                {
                    property.SetIsUnicode(false);
                }
            }
        }
    }
}
