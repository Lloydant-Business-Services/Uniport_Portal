using System;

namespace Abundance_Nk.Model.Model
{
    public class DepartmentalSchoolFees
    {
        public Department department { get; set; }
        public Programme programme { get; set; }
        public Decimal Amount { get; set; }
        public FeeType feetype { get; set; }
        public Level level { get; set; }
    }
}