using CineApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CineApi.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        // Injeção de dependência para acessar a configuração
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<User> Usuarios { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<FilmeProgramacao> Programacoes { get; set; }
        public DbSet<Assinatura> Assinaturas { get; set; }
        public DbSet<Ingresso> Ingressos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assinatura>().HasData(
                new Assinatura { Id = 1, Nome = "Iniciante", Preco = 0, Desconto = 0 },
                new Assinatura { Id = 2, Nome = "Cinéfilo", Preco = 20, Desconto = 0.15m },
                new Assinatura { Id = 3, Nome = "Fanático", Preco = 50, Desconto = 0.25m },
                new Assinatura { Id = 4, Nome = "Diretor", Preco = 100, Desconto = 0.50m }
            );
        }
    }
}
