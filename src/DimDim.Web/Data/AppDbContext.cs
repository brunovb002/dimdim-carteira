using DimDim.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DimDim.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("CLIENTE");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nome).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Email).HasMaxLength(150).IsRequired();
            entity.HasIndex(c => c.Email).IsUnique();
            entity.Property(c => c.Telefone).HasMaxLength(20);
        });

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.ToTable("TRANSACAO");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Descricao).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Tipo).HasMaxLength(20).HasConversion<string>();
            entity.Property(t => t.Status).HasMaxLength(20).HasConversion<string>();

            entity.HasOne(t => t.Cliente)
                  .WithMany(c => c.Transacoes)
                  .HasForeignKey(t => t.ClienteId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
