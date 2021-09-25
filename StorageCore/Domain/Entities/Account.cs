using System;

namespace StorageCore.Domain.Entities
{
    public class Account 
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } 
        public DateTime Updated { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string PhoneNumber { get; set; }
    }
}