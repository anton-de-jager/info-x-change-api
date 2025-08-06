using infoX.api.Controllers;
using infoX.api.Models;
using Microsoft.EntityFrameworkCore;

namespace infoX.api.Data
{
    public class WhatsAppDbContext : DbContext
    {
        public WhatsAppDbContext(DbContextOptions<WhatsAppDbContext> options) : base(options) { }

        public DbSet<dashboardWhatsApp> dashboardWhatsApp { get; set; }
        public DbSet<VwMessages> VwMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<dashboardWhatsApp>().HasNoKey();
            modelBuilder.Entity<VwMessages>().HasNoKey();
        }
    }
}
