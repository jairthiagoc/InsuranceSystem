using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProposalService.Domain.Entities;

namespace ProposalService.Infrastructure.Data;

[ExcludeFromCodeCoverage]
public class ProposalDbContext : DbContext
{
    public ProposalDbContext(DbContextOptions<ProposalDbContext> options) : base(options)
    {
    }

    public DbSet<Proposal> Proposals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.InsuranceType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CoverageAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PremiumAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
        });
    }
} 