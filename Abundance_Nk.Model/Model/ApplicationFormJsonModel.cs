using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
     public class ApplicationFormJsonModel
    {
         public bool IsError { get; set; }
         public string ImageFileUrl { get; set; }
         public string Message { get; set; }
    }
    public class JambRecordJsonModel
    {
        public bool IsError { get; set; }
        public string ImageFileUrl { get; set; }
        public string Message { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string JambNo { get; set; }
        public int LgaId { get; set; }
        public int CourseId { get; set; }
        public string StateId { get; set; }
    }
}
