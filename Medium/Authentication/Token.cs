using System;

namespace Medium.Authentication
{
    public class Token
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public Scope[] Scope { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}