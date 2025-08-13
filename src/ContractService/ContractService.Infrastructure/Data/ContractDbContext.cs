using Microsoft.EntityFrameworkCore;
using ContractService.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace ContractService.Infrastructure.Data;

[ExcludeFromCodeCoverage]
public class ContractDbContext : DbContext
{
    public ContractDbContext(DbContextOptions<ContractDbContext> options) : base(options)
    {
    }

    public DbSet<Contract> Contracts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProposalId).IsRequired();
            entity.Property(e => e.ContractNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PremiumAmount).HasColumnType("numeric(18,2)");
            entity.Property(e => e.ContractDate).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // Índice único para ProposalId
            entity.HasIndex(e => e.ProposalId).IsUnique();
            
            // Configurações específicas do PostgreSQL
            entity.ToTable("contracts");
        });
    }
} 