using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContractService.Infrastructure.Data;

public class ContractDbContextFactory : IDesignTimeDbContextFactory<ContractDbContext>
{
    public ContractDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContractDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=insurance_contracts;Username=postgres;Password=postgres123;");

        return new ContractDbContext(optionsBuilder.Options);
    }
} 