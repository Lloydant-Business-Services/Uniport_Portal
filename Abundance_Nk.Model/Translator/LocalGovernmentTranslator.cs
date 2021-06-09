using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class LocalGovernmentTranslator : TranslatorBase<LocalGovernment, LOCAL_GOVERNMENT>
    {
        private readonly StateTranslator stateTranslator = new StateTranslator();

        public override LocalGovernment TranslateToModel(LOCAL_GOVERNMENT localGovernmentEntity)
        {
            try
            {
                LocalGovernment localGovernment = null;
                if (localGovernmentEntity != null)
                {
                    localGovernment = new LocalGovernment();
                    localGovernment.Id = localGovernmentEntity.Local_Government_Id;
                    localGovernment.Name = localGovernmentEntity.Local_Government_Name;
                    localGovernment.State = stateTranslator.TranslateToModel(localGovernmentEntity.STATE);
                }

                return localGovernment;
            }
            catch (Exception)
            {
                throw;
            }
            ;
        }

        public override LOCAL_GOVERNMENT TranslateToEntity(LocalGovernment localGovernment)
        {
            try
            {
                LOCAL_GOVERNMENT localGovernmentEntity = null;
                if (localGovernment != null)
                {
                    localGovernmentEntity = new LOCAL_GOVERNMENT();
                    localGovernmentEntity.Local_Government_Id = localGovernment.Id;
                    localGovernmentEntity.Local_Government_Name = localGovernment.Name;
                    localGovernmentEntity.State_Id = localGovernment.State.Id;
                }

                return localGovernmentEntity;
            }
            catch (Exception)
            {
                throw;
            }
            ;
        }
    }
}