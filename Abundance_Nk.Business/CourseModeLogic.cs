using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseModeLogic : BusinessBaseLogic<CourseMode, COURSE_MODE>
    {
        public CourseModeLogic()
        {
            translator = new CourseModeTranslator();
        }
    }
}