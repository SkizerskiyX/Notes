namespace NoteDatabase.DbContext
   
{
using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using NoteModels.Models;
    using System.Data.Common;

public class NoteDbContext : DbContext
    {
        public NoteDbContext(DbContextOptions options) : base(options) { }
        
        public DbSet<Note> Notes => Set<Note>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configuration.NoteConfiguration());
        }

    }
}
