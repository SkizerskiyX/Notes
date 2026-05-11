namespace NoteDatabase.DbContext
{ 

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
    using NoteModels.Entities;

    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configuration.UserConfiguration());
        }
    }
}

