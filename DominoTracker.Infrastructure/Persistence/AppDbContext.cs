using Microsoft.EntityFrameworkCore;
using DominoTracker.Domain.Entities;
using DominoTracker.Application.Interfaces;

namespace DominoTracker.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly ICurrentSessionAccessor _currentSessionAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentSessionAccessor currentSessionAccessor)
        : base(options)
    {
        _currentSessionAccessor = currentSessionAccessor;
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<War> Wars => Set<War>();
    public DbSet<WarMatch> WarMatches => Set<WarMatch>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Team config (evitar duplicados)
        modelBuilder.Entity<Team>()
            .HasIndex(t => new { t.SessionId, t.Player1Id, t.Player2Id })
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

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Season)
            .WithMany()
            .HasForeignKey(m => m.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

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

        modelBuilder.Entity<War>()
            .HasOne(w => w.Season)
            .WithMany()
            .HasForeignKey(w => w.SeasonId)
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

        // Relaciones *.SessionId -> Session (sin navegación inversa)
        modelBuilder.Entity<Player>()
            .HasOne<Session>()
            .WithMany()
            .HasForeignKey(p => p.SessionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Team>()
            .HasOne<Session>()
            .WithMany()
            .HasForeignKey(t => t.SessionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne<Session>()
            .WithMany()
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<War>()
            .HasOne<Session>()
            .WithMany()
            .HasForeignKey(w => w.SessionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Season>()
            .HasOne<Session>()
            .WithMany()
            .HasForeignKey(s => s.SessionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Season>()
            .HasIndex(s => new { s.SessionId, s.Alias })
            .IsUnique();

        // User/Role: sin filtro global (super_admin gestiona usuarios de cualquier sesión)
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne<Role>()
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne<Session>()
            .WithMany()
            .HasForeignKey(u => u.SessionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Filtros globales: cada sesión solo ve su propia data
        modelBuilder.Entity<Player>().HasQueryFilter(p => p.SessionId == _currentSessionAccessor.SessionId);
        modelBuilder.Entity<Team>().HasQueryFilter(t => t.SessionId == _currentSessionAccessor.SessionId);
        modelBuilder.Entity<Match>().HasQueryFilter(m => m.SessionId == _currentSessionAccessor.SessionId);
        modelBuilder.Entity<War>().HasQueryFilter(w => w.SessionId == _currentSessionAccessor.SessionId);
        modelBuilder.Entity<Season>().HasQueryFilter(s => s.SessionId == _currentSessionAccessor.SessionId);

        // Sesión sembrada: contiene toda la data existente antes de este feature
        modelBuilder.Entity<Session>().HasData(
            new Session { Id = 1, Name = "Sesión principal", CreatedAt = new DateTime(2026, 4, 1) }
        );

        // Seasons: calendario fijo de temporadas (sesión 1)
        modelBuilder.Entity<Season>().HasData(
            new Season { Id = 1, StartDate = new DateTime(2026, 4, 1), EndDate = new DateTime(2026, 10, 2), SessionId = 1, Alias = 1 },
            new Season { Id = 2, StartDate = new DateTime(2026, 10, 3), EndDate = new DateTime(2027, 3, 31), SessionId = 1, Alias = 2 }
        );

        // Roles fijos
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "super_admin" },
            new Role { Id = 2, Name = "session_admin" },
            new Role { Id = 3, Name = "user" }
        );

        // Usuario semilla: super_admin (password: Dom_SA_AChA, hash precalculado con PasswordHasher)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("8f3b1a2c-6b7e-4e5f-9a2d-1c4f6e7a8b9c"),
                Username = "Domino_SupAd",
                PasswordHash = "/Jx5Yu58AibaTyi8XdLUGBINFcHigxQUmZJuIuWbP4/sbf78aNixNoAzchndAo0P",
                RoleId = 1,
                SessionId = null
            }
        );
    }
}
