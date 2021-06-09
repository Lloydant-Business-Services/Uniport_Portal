using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseEvaluationQuestionLogic : BusinessBaseLogic<CourseEvaluationQuestion, COURSE_EVALUATION_QUESTION>
    {
        public CourseEvaluationQuestionLogic()
        {
            translator = new CourseEvaluationQuestionTranslator();
        }
    }
}