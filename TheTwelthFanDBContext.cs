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
        public DbSet<FantasyTeam> FantasyTeams {get; set;} = null;
        public DbSet<FantasyLeague> FantasyLeagues {get; set;} = null;


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
        }
    }
}
