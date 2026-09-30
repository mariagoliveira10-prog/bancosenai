using BancoSENAIAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Agencia> Agencia => Set<Agencia>();
        public DbSet<Carteira> Carteiras => Set<Carteira>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<DocumentoMetaDado> Documentos => Set<DocumentoMetaDado>();
    }
}
