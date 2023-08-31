using AvitoBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AvitoBot.Database
{
    public class BotDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public BotDbContext(DbContextOptions<BotDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Id).ValueGeneratedOnAdd();

                e.HasIndex(x => x.TGId).IsUnique();
            }).Entity<Request>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Id).ValueGeneratedOnAdd();

                e.Property(x => x.Uri).HasConversion(x => x.ToString(), x => new Uri(x));

                e.HasOne(x => x.Owner).WithMany(x => x.Requests).HasForeignKey(x => x.OwnerId);

                e.HasIndex(x => new { x.SearchRequest, x.OwnerId }).IsUnique();
            });
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Request> Requests => Set<Request>();
    }
}