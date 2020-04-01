using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace npcblas2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CharacterBuild> CharacterBuilds { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultContainer("pf2npc");
            builder.Entity<CharacterBuild>().OwnsMany(b => b.Choices).HasKey(ch => new { ch.CharacterBuildId, ch.Order });
        }
    }
}
