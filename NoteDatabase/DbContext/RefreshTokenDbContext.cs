namespace NoteDatabase.DbContext
{
using System;
using System.Collections.Generic;
    using NoteModels.Entities;
using System.Text;
using Microsoft.EntityFrameworkCore;

    public class RefreshTokenDbContext : DbContext
    {
        public RefreshTokenDbContext(DbContextOptions<RefreshTokenDbContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configuration.RefreshTokenConfiguration());
        }

    }
}
