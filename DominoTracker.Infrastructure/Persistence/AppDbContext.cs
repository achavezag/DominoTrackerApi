using Microsoft.EntityFrameworkCore;
using DominoTracker.Domain.Entities;

namespace DominoTracker.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Match> Matches => Set<Match>();
    // public DbSet<Round> Rounds => Set<Round>();
    public DbSet<War> Wars => Set<War>();
    public DbSet<WarMatch> WarMatches => Set<WarMatch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Team config (evitar duplicados)
        modelBuilder.Entity<Team>()
            .HasIndex(t => new { t.Player1Id, t.Player2Id })
            .IsUnique();

        // Relaciones Team -> Player
        modelBuilder.Entity<Team>()
            .HasOne(t => t.Player1)
            .WithMany()
            .HasForeignKey(t => t.Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Player2)
            .WithMany()
            .HasForeignKey(t => t.Player2Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Match relaciones
        modelBuilder.Entity<Match>()
            .HasOne(m => m.TeamA)
            .WithMany()
            .HasForeignKey(m => m.TeamAId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.TeamB)
            .WithMany()
            .HasForeignKey(m => m.TeamBId)
            .OnDelete(DeleteBehavior.Restrict);

        // Round relación
        /* modelBuilder.Entity<Round>()
            .HasOne(r => r.Match)
            .WithMany(m => m.Rounds)
            .HasForeignKey(r => r.MatchId); */

        // War relaciones
        modelBuilder.Entity<War>()
            .HasOne(w => w.TeamA)
            .WithMany()
            .HasForeignKey(w => w.TeamAId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<War>()
            .HasOne(w => w.TeamB)
            .WithMany()
            .HasForeignKey(w => w.TeamBId)
            .OnDelete(DeleteBehavior.Restrict);

        // WarMatch relaciones
        modelBuilder.Entity<WarMatch>()
            .HasOne(wm => wm.War)
            .WithMany(w => w.WarMatches)
            .HasForeignKey(wm => wm.WarId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WarMatch>()
            .HasOne(wm => wm.Match)
            .WithMany()
            .HasForeignKey(wm => wm.MatchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WarMatch>()
            .HasIndex(wm => new { wm.WarId, wm.Order })
            .IsUnique();
    }
}