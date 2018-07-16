namespace AutomobileMaintenanceTracker.Models
{
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;

    public class AMTDbContext : DbContext
    {
        public AMTDbContext()
            : base("name=AMTDbContext")
        {
        }

        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<MaintainceTask> MaintainceTasks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasMany(e => e.MaintainceTasks)
                .WithRequired(e => e.Car)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<MaintainceTask>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); ;
            modelBuilder.Entity<MaintainceTask>()
                .Property(e => e.Type)
                .IsFixedLength();
        }
    }
}
