using access_control.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace access_control.infrastructure
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Lock> Locks {  get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
        public DbSet<Permission> Permissions { get; set; }
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
