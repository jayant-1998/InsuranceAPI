using InsuranceAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.DAL.DBContext
{
    public class ApplicationDBContext : DbContext

    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {}

        public DbSet<UsersEntity> users { get; set; }
        public DbSet<PolicyDocumentEntity> documents { get; set; }
        public DbSet<HtmlTemplateEntity> templates { get; set; }
        public DbSet<EmailEntity> email { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsersEntity>();
            modelBuilder.Entity<HtmlTemplateEntity>();
            modelBuilder.Entity<PolicyDocumentEntity>();
            modelBuilder.Entity<EmailEntity>();
        }
    }
}
