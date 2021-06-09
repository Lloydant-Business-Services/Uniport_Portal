using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class UserAudit
    {
        public long Id { get; set; }
        public long User_Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public SecurityQuestion SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public Role Role { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        [Display(Name = "Signature")]
        public string SignatureUrl { get; set; }
        [Display(Name = "Profile Picture")]
        public string ProfileImageUrl { get; set; }

        public User User { get; set; }
        public string Operatiion { get; set; }
        public string Action { get; set; }
        public string Client { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}
