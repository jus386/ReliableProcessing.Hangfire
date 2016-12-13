using System.Data.Entity;

namespace DataAccess.EF
{
    public class ProcessingDbContext : DbContext
    {
        public virtual DbSet<CalculationJob> Jobs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalculationJob>()
                .HasKey(x => x.Id);
        }
    }
}