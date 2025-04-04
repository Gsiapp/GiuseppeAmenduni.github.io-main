using Microsoft.EntityFrameworkCore;
using GestioneOrdini.Models;

namespace GestioneOrdini.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Carrello> Carrelli { get; set; }
        public DbSet<CarrelloItem> CarrelloItems { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Definizione delle tabelle
        public DbSet<Prodotto> Prodotti { get; set; }
        public DbSet<Cliente> Clienti { get; set; }
        public DbSet<Ordine> Ordini { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurazione relazione Cliente-Ordini
            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Ordini)
                .WithOne(o => o.Cliente)
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
        
    }
}