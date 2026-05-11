using System;
using System.Collections.Generic;
using System.Text;

namespace NoteModels.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; private set; } = string.Empty;
        
        public Guid UserId { get; private set; }

        public DateTime ExpiresAt { get; private set; }
  
        public bool IsRevoked { get; private set; }

        public RefreshToken(string token, Guid userId, DateTime expiresAt)
        {
            Token = token;
            UserId = userId;
            ExpiresAt = expiresAt;  
            IsRevoked = false;
        }
        public User? User { get; set; }

        private RefreshToken() { }
    }
}
