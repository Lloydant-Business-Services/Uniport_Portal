using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseTypeLogic : BusinessBaseLogic<CourseType, COURSE_TYPE>
    {
        public CourseTypeLogic()
        {
            translator = new CourseTypeTranslator();
        }
    }
}