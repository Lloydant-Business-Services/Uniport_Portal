using System;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public SecurityQuestion SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public Role Role { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        [Display(Name="Signature")]
        public string SignatureUrl { get; set; }
         [Display(Name = "Profile Picture")]
        public string ProfileImageUrl { get; set; }
         public bool Activated { get; set; }
    }
}