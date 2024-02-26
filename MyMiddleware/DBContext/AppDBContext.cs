
using Microsoft.EntityFrameworkCore;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }
}