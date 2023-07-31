﻿using InsuranceAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.DAL.DBContexts
{
    public class ApplicationDBContexts : DbContext

    {
        public ApplicationDBContexts(DbContextOptions options) : base(options){}

        public DbSet<Users> users { get; set; }
        public DbSet<PolicyDocuments> documents { get; set; }
        public DbSet<HtmlTemplate> templates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>();
            modelBuilder.Entity<HtmlTemplate>();
            modelBuilder.Entity<PolicyDocuments>().HasKey("id");
        }

    }
}
