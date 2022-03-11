using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Task2.Models;

namespace Task2.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<House> Houses { get; set; }
        public DbSet<Flat> Flats { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> User { get; set; }
    }
}
