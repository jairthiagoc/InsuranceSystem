using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProposalService.Infrastructure.Data;

public class ProposalDbContextFactory : IDesignTimeDbContextFactory<ProposalDbContext>
{
    public ProposalDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProposalDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=InsuranceProposals;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;");

        return new ProposalDbContext(optionsBuilder.Options);
    }
} 