using System;
using System.Collections.Generic;
using System.Text;

namespace NoteModels.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        
        public string Email { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

        public User(string email, string passwordHash)
        {
            Id = Guid.CreateVersion7();
            Email = email;
            PasswordHash = passwordHash;
        }

        private User() { }
    }
}
