using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentStatusLogic : BusinessBaseLogic<StudentStatus, STUDENT_STATUS>
    {
        public StudentStatusLogic()
        {
            translator = new StudentStatusTranslator();
        }
    }
}