using Microsoft.EntityFrameworkCore;

namespace CART_DECISION_TREE
{
    // ENTITY FRAMEWORK CONFIGURATION WITH MSSQL DATABASE
    public class DBContext : DbContext
    {
        private readonly IConfiguration config;

        // DB CONTEXT OPTIONS
        public DBContext(DbContextOptions<DBContext> dbContextOptions)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public DbSet<trainingSet> trainingSet { get; set; }
        public DbSet<testSet> testSet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(connectionConfiguration.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<trainingSet>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<testSet>(entity =>
            {
                entity.HasNoKey();
            });



        }
    }
}
