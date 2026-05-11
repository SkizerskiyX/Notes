using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using NoteModels.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace NoteDatabase.Configuration
{
    internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Token).IsRequired();
            builder.Property(rt => rt.ExpiresAt).IsRequired();
            builder.Property(rt => rt.UserId).IsRequired();
            builder.HasIndex(rt => rt.Token).IsUnique();
            builder.HasOne(rt => rt.User)
             .WithMany(u => u.RefreshTokens)
             .HasForeignKey(rt => rt.UserId);
        }
    }
}
