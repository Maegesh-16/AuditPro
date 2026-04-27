using Microsoft.EntityFrameworkCore;
using ActionService.API.Models;

namespace ActionService.API.Data
{
    public class ActionDbContext : DbContext
    {
        public ActionDbContext(DbContextOptions<ActionDbContext> options) : base(options)
        {
        }

        public DbSet<CorrectiveAction> CorrectiveActions { get; set; }
        public DbSet<ActionHistory> ActionHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CorrectiveAction>(entity =>
            {
                entity.HasKey(e => e.ActionId);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            modelBuilder.Entity<ActionHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.CorrectiveAction)
                      .WithMany(c => c.ActionHistories)
                      .HasForeignKey(e => e.ActionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
