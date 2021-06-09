using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    public class StudentBaseController :Controller
    {
        public static Model.Model.Student StudentDetail { get; set; }

        protected void SetMessage(string message,Message.Category messageType)
        {
            var msg = new Message(message,(int)messageType);
            TempData["Message"] = msg;
        }

        public bool ValidateStudent(string MatricNumber,string password)
        {
            try
            {
                var studentLogic = new StudentLogic();
                StudentDetail = studentLogic.GetBy(MatricNumber,password);
                if(StudentDetail != null)
                {
                    return true;
                }
            }
            catch(Exception)
            {
                throw;
            }
            return false;
        }
    }
}