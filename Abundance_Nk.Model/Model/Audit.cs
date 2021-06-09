using System;

namespace Abundance_Nk.Model.Model
{
    public class Audit
    {
        public User User { get; set; }
        public string Operation { get; set; }
        public string Action { get; set; }
        public DateTime Time { get; set; }
        public string Client { get; set; }
    }
}