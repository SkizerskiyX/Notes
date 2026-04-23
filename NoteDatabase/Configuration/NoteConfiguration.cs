using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoteModels.Models;
using System; 
using System.Collections.Generic;
using System.Text;

namespace NoteDatabase.Configuration
{
    internal class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Header).IsRequired().HasMaxLength(250);
            builder.Property(b => b.Text).IsRequired();
            builder.HasQueryFilter(b => !b.IsDeleted);
            
           
        }
        

    }
}
