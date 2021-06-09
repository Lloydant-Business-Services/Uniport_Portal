﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Transactions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class AdmissionCriteriaForOLevelSubjectAlternativeLogic :
        BusinessBaseLogic
            <AdmissionCriteriaForOLevelSubjectAlternative, ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE>
    {
        public AdmissionCriteriaForOLevelSubjectAlternativeLogic()
        {
            translator = new AdmissionCriteriaForOLevelSubjectAlternativeTranslator();
        }

        public bool Modify(List<AdmissionCriteriaForOLevelSubject> subjects)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (AdmissionCriteriaForOLevelSubject subject in subjects)
                    {
                        foreach (AdmissionCriteriaForOLevelSubjectAlternative subjectAlternative in subject.Alternatives
                            )
                        {
                            Expression<Func<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE, bool>> selector =
                                a => a.Admission_Criteria_For_O_Level_Subject_Id == subject.Id;
                            ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE criteriaAlternative =
                                GetEntityBy(selector);


                            if (criteriaAlternative == null)
                            {
                                var criteriaAlternativeSubject = new AdmissionCriteriaForOLevelSubjectAlternative();
                                criteriaAlternativeSubject.OLevelSubject.Id = subjectAlternative.OLevelSubject.Id;
                                criteriaAlternativeSubject.Alternative.Id = subject.Id;
                                Create(criteriaAlternativeSubject);
                            }
                            else
                            {
                                criteriaAlternative.O_Level_Subject_Id = subjectAlternative.OLevelSubject.Id;
                                int modifiedRecordCount = Save();
                            }
                        }
                    }
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}