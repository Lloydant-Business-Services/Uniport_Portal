using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Transactions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class AdmissionCriteriaForOLevelSubjectLogic :
        BusinessBaseLogic<AdmissionCriteriaForOLevelSubject, ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT>
    {
        public AdmissionCriteriaForOLevelSubjectLogic()
        {
            translator = new AdmissionCriteriaForOLevelSubjectTranslator();
        }

        public bool Modify(List<AdmissionCriteriaForOLevelSubject> subjects)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (AdmissionCriteriaForOLevelSubject subject in subjects)
                    {
                        Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT, bool>> selector =
                            a => a.Admission_Criteria_For_O_Level_Subject_Id == subject.Id;
                        ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT criteria = GetEntityBy(selector);

                        if (criteria == null)
                        {
                            //AdmissionCriteriaForOLevelSubject admissionCriteriaForOLevelSubject = new Course();
                            //admissionCriteriaForOLevelSubject
                            //Create(newCourse);
                        }

                        criteria.O_Level_Subject_Id = subject.Subject.Id;
                        criteria.Is_Compulsory = subject.IsCompulsory;
                        criteria.Minimum_O_Level_Grade_Id = subject.MinimumGrade.Id;
                        int modifiedRecordCount = Save();

                        if (subject.Alternatives != null && subject.Alternatives[0].OLevelSubject.Id > 0)
                        {
                            var criteriaSubjectAlternativeLogic =
                                new AdmissionCriteriaForOLevelSubjectAlternativeLogic();

                            foreach (
                                AdmissionCriteriaForOLevelSubjectAlternative subjectAlternative in subject.Alternatives)
                            {
                                AdmissionCriteriaForOLevelSubjectAlternative criteriaAlternative =
                                    criteriaSubjectAlternativeLogic.GetModelBy(
                                        a => a.Admission_Criteria_For_O_Level_Subject_Id == subject.Id);


                                if (criteriaAlternative == null)
                                {
                                    var criteriaAlternativeSubject = new AdmissionCriteriaForOLevelSubjectAlternative();
                                    criteriaAlternativeSubject.OLevelSubject = subjectAlternative.OLevelSubject;
                                    criteriaAlternativeSubject.Alternative = subject;
                                    criteriaSubjectAlternativeLogic.Create(criteriaAlternativeSubject);
                                }
                                else
                                {
                                    criteriaAlternative.Alternative = subjectAlternative.Alternative;
                                    criteriaAlternative.OLevelSubject = subjectAlternative.OLevelSubject;

                                    modifiedRecordCount = criteriaSubjectAlternativeLogic.Save();
                                }
                            }
                        }
                    }
                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}