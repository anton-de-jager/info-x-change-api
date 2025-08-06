using infoX.api.Controllers;
using infoX.api.Models;
using Microsoft.EntityFrameworkCore;

namespace infoX.api.Data
{
    public class PegasusConfigurationDbContext : DbContext
    {
        public PegasusConfigurationDbContext(DbContextOptions<PegasusConfigurationDbContext> options) : base(options) { }

        public DbSet<Dynamic> Dynamic { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<VwUser> VwUser { get; set; }
        public DbSet<User2FACode> User2FACodes { get; set; }
        public DbSet<ConfigColumn> ConfigColumns { get; set; }
        public DbSet<ConfigTable> ConfigTables { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Bot> Bot { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Segment> Segment { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Platform> Platform { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<PlatformUser> PlatformUser { get; set; }
        public DbSet<GroupDepartment> GroupDepartment { get; set; }
        public DbSet<AuditTrail> AuditTrail { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<VwUser>().ToView("vw_user");
            //modelBuilder.Entity<User>().ToTable(tb => tb.HasTrigger("trg_user_Audit"));
            //modelBuilder.Entity<User>().HasKey(u => u.Id);

            modelBuilder.Entity<Dynamic>().ToTable("dynamic");

            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<VwUser>().ToView("vw_user");

            //modelBuilder.Entity<User2FACode>().ToTable(tb => tb.HasTrigger("trg_user2FACodes_Audit"));
            modelBuilder.Entity<User2FACode>().ToTable("User2FACodes");

            //modelBuilder.Entity<ConfigColumn>().ToTable(tb => tb.HasTrigger("trg_configColumn_Audit"));
            //modelBuilder.Entity<ConfigTable>().ToTable(tb => tb.HasTrigger("trg_configTable_Audit"));
            modelBuilder.Entity<ConfigColumn>().ToTable("ConfigColumn");
            modelBuilder.Entity<ConfigTable>().ToTable("ConfigTable");

            //modelBuilder.Entity<Company>().ToTable(tb => tb.HasTrigger("trg_company_Audit"));
            //modelBuilder.Entity<Department>().ToTable(tb => tb.HasTrigger("trg_department_Audit"));
            //modelBuilder.Entity<Role>().ToTable(tb => tb.HasTrigger("trg_role_Audit"));
            //modelBuilder.Entity<Platform>().ToTable(tb => tb.HasTrigger("trg_platform_Audit"));
            //modelBuilder.Entity<Group>().ToTable(tb => tb.HasTrigger("trg_group_Audit"));
            //modelBuilder.Entity<PlatformUser>().ToTable(tb => tb.HasTrigger("trg_platformUser_Audit"));
            //modelBuilder.Entity<GroupDepartment>().ToTable(tb => tb.HasTrigger("trg_GgroupDepartment_Audit"));
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Bot>().ToTable("Bot");
            modelBuilder.Entity<Book>().ToTable("Book");
            modelBuilder.Entity<Segment>().ToTable("Segment");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Platform>().ToTable("Platform");
            modelBuilder.Entity<Group>().ToTable("Group");
            modelBuilder.Entity<PlatformUser>().ToTable("PlatformUser");
            modelBuilder.Entity<GroupDepartment>().ToTable("GroupDepartment");

            //modelBuilder.Entity<ConfigTable>().ToTable(tb => tb.HasTrigger("trg_configTable_Audit"));
            modelBuilder.Entity<AuditTrail>().ToTable("AuditTrail");

            modelBuilder.Entity<Message>().ToTable("Message");
        }
    }
}
