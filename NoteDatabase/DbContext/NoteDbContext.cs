namespace NoteDatabase.DbContext
   
{
using Microsoft.EntityFrameworkCore;
    using NoteModels.Entities;
    using NoteModels.Models;

public class NoteDbContext : DbContext
    {
        public NoteDbContext(DbContextOptions options) : base(options) { }
        
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configuration.NoteConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configuration.RefreshTokenConfiguration());
        }

    }
}
