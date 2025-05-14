using infoX.api.Controllers;
using infoX.api.Models;
using Microsoft.EntityFrameworkCore;

namespace infoX.api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<User2FACode> User2FACodes { get; set; }
        public DbSet<ConfigColumn> ConfigColumns { get; set; }
        public DbSet<ConfigTable> ConfigTables { get; set; }
        public DbSet<Abandoned> Abandoned { get; set; }
        public DbSet<AgentKPI> AgentKPI { get; set; }
        public DbSet<ActiveUsers> ActiveUsers { get; set; }
        public DbSet<CallBack> CallBack { get; set; }
        public DbSet<AgentProductivity> AgentProductivity { get; set; }
        public DbSet<DebitOrderArrangements> DebitOrderArrangements { get; set; }
        public DbSet<dashboardWhatsApp> dashboardWhatsApp { get; set; }
        public DbSet<DynamicResult> DynamicResults { get; set; }
        public DbSet<VwMessages> VwMessages { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<DocumentUpload> DocumentUpload { get; set; }
        public DbSet<Arrangements> Arrangements { get; set; }
        public DbSet<TotalUsers> TotalUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>().HasKey(u => u.ID);

            modelBuilder.Entity<User2FACode>().ToTable("User2FACodes");

            modelBuilder.Entity<ConfigColumn>().ToTable("ConfigColumn");
            modelBuilder.Entity<ConfigTable>().ToTable("ConfigTable");

            modelBuilder.Entity<Abandoned>().HasNoKey();
            modelBuilder.Entity<AgentKPI>().HasNoKey();
            modelBuilder.Entity<AgentProductivity>().HasNoKey();
            modelBuilder.Entity<ActiveUsers>().HasNoKey();
            modelBuilder.Entity<CallBack>().HasNoKey();
            modelBuilder.Entity<DebitOrderArrangements>().HasNoKey();
            modelBuilder.Entity<dashboardWhatsApp>().HasNoKey();
            modelBuilder.Entity<VwMessages>().HasNoKey();
            modelBuilder.Entity<Payment>().HasNoKey();
            modelBuilder.Entity<DocumentUpload>().HasNoKey();
            modelBuilder.Entity<Arrangements>().HasNoKey();
            modelBuilder.Entity<TotalUsers>().HasNoKey();

            modelBuilder.Entity<DynamicResult>().HasNoKey().ToView(null);
        }
    }
}
