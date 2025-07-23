using Microsoft.EntityFrameworkCore;
using TheTwelthFan.Models;

namespace TheTwelthFan.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!; // Add Players DbSet
        public DbSet<FantasyTeam> FantasyTeams { get; set; } = null;
        public DbSet<FantasyLeague> FantasyLeagues { get; set; } = null;
        public DbSet<Draft> Drafts { get; set; } = null;
        public DbSet<DraftOrderEntry> DraftOrderEntries { get; set; } = null;
        public DbSet<DraftPickRequest> DraftPickRequestEntries { get; set; } = null;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(u => u.id);
                entity.Property(u => u.username).IsRequired();
                entity.Property(u => u.password).IsRequired();
            });

            // Configure Player entity
            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("player"); // Set table name to "player"
                entity.HasKey(p => p.id); // Set primary key
                entity.Property(p => p.name).IsRequired();
                entity.Property(p => p.position).IsRequired();
                entity.Property(p => p.jerseynumber).IsRequired();
                entity.Property(p => p.team).IsRequired();
                entity.Property(p => p.fantasyteamid).IsRequired();
                entity.Property(p => p.fantasyleagueid).IsRequired();
                entity.Property(p => p.userId).IsRequired();

            });
            modelBuilder.Entity<FantasyTeam>(entity =>
            {
                entity.ToTable("fantasyteam");
                entity.HasKey(ft => ft.id);
                entity.Property(ft => ft.name).IsRequired();
                entity.Property(ft => ft.userid).IsRequired();
                entity.Property(ft => ft.fantasyleagueid).IsRequired();
            });
            modelBuilder.Entity<FantasyLeague>(entity =>
            {
                entity.ToTable("fantasyleague");
                entity.HasKey(fl => fl.id);
                entity.Property(fl => fl.name).IsRequired();
                entity.Property(fl => fl.owneruserid).IsRequired();
            });
            // Configure Draft entity
            modelBuilder.Entity<Draft>(entity =>
            {
                entity.ToTable("draft");
                entity.HasKey(d => d.Id);

                entity.Property(d => d.FantasyLeagueId).IsRequired();
                entity.Property(d => d.PickNumber).IsRequired();
                entity.Property(d => d.IsComplete).IsRequired();
                entity.Property(d => d.StartedAt);
                entity.Property(d => d.CompletedAt);

                entity.HasMany(d => d.DraftOrder)
                      .WithOne()
                      .HasForeignKey(de => de.DraftId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DraftOrderEntry entity
            modelBuilder.Entity<DraftOrderEntry>(entity =>
            {
                entity.ToTable("draftorderentry");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DraftId).IsRequired();
                entity.Property(e => e.PickNumber).IsRequired();
                entity.Property(e => e.FantasyTeamId).IsRequired();
            });

            // Configure DraftPickRequest entity
            modelBuilder.Entity<DraftPickRequest>(entity =>
            {
                entity.ToTable("draftpickrequest");

                // No primary key configured here because it might not be persisted.
                // If it's intended to be stored, add a primary key like:
                entity.HasKey(dpr => new { dpr.LeagueId, dpr.TeamId, dpr.PlayerId });

                entity.Property(dpr => dpr.LeagueId).IsRequired();
                entity.Property(dpr => dpr.TeamId).IsRequired();
                entity.Property(dpr => dpr.UserId).IsRequired();
                entity.Property(dpr => dpr.PlayerId).IsRequired();
            });

        }
    }
}
