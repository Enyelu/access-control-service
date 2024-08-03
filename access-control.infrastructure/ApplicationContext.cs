using access_control.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace access_control.infrastructure
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Lock>()
                .HasIndex(l => l.SerialNumber)
                .IsUnique();
        }
    }
}
