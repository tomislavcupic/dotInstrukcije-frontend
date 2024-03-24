using Microsoft.EntityFrameworkCore;

namespace dotInstrukcije.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Professor> Professors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<InstructionsDate> InstructionsDates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure InstructionsDate as a keyless entity type
            modelBuilder.Entity<InstructionsDate>().HasNoKey();
        }
    }
}
