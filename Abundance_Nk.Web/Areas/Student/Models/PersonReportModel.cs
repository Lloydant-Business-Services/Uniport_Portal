using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abundance_Nk.Web.Areas.Student.Models
{
    public class PersonReportModel
    {
        public string SessionName { get; set; }
        public string LevelName { get; set; }
        public string FullName { get; set; }
        public string MatricNo { get; set; }
        public string Department { get; set; }
        public string Date { get; set; }
        public string Programme { get; set; }
    }
}