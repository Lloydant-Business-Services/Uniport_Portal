using System;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Session : Setup
    {
        [Display(Name = "Session")]
        public override int Id
        {
            get { return base.Id; }
            set { base.Id = value; }
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? Activated { get; set; }
        public bool ActiveCourseRegistration { get; set; }
        public bool ActiveApplication { get; set; }
        public bool ActiveHostelApplication { get; set; }
    }
}