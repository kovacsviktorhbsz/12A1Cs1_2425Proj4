using _4.projektmunka.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videogames.Database
{
    class VideoGamesContext : DbContext
    {
        public VideoGamesContext() : base("name=VideoGamesContext") { }

        public DbSet<Game> Games { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Game_Platform> Game_Platforms { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Platforms)
                .WithMany(p => p.Games)
                .Map(m =>
                {
                    m.ToTable("Game_Platforms");
                    m.MapLeftKey("GameID");
                    m.MapRightKey("PlatformID");
                });
        }
    }
}
