using Microsoft.EntityFrameworkCore;
using InternshipDB.Models;

namespace InternshipDB.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
    }
}