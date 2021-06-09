using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.PGStudent.ViewModels
{

    public class PGFormViewModel : StudentFormViewModelBase
    {
        private const int START_YEAR = 1920;
        private readonly AbilityLogic abilityLogic;
        private readonly BloodGroupLogic bloodGroupLogic;
        private readonly DepartmentOptionLogic departmentOptionLogic;
        private readonly EducationalQualificationLogic educationalQualificationLogic;
        private readonly GenotypeLogic genotypeLogic;
        private readonly ITDurationLogic iTDurationLogic;
        private readonly InstitutionChoiceLogic institutionChoiceLogic;
        private readonly LevelLogic levelLogic;
        private readonly LocalGovernmentLogic lgaLogic;
        private readonly MaritalStatusLogic maritalStatusLogic;
        private readonly ModeOfEntryLogic modeOfEntryLogic;
        private readonly ModeOfFinanceLogic modeOfFinanceLogic;
        private readonly ModeOfStudyLogic modeOfStudyLogic;
        private readonly OLevelGradeLogic oLevelGradeLogic;
        private readonly OLevelSubjectLogic oLevelSubjectLogic;
        private readonly OLevelTypeLogic oLevelTypeLogic;
        private readonly ProgrammeLogic programmeLogic;
        private readonly RelationshipLogic relationshipLogic;
        private readonly ReligionLogic religionLogic;
        private readonly ResultGradeLogic resultGradeLogic;
        private readonly SexLogic sexLogic;
        private readonly StateLogic stateLogic;
        private readonly StudentCategoryLogic studentCategoryLogic;
        private readonly StudentStatusLogic studentStatusLogic;
        private readonly StudentTypeLogic studentTypeLogic;
        private readonly TitleLogic titleLogic;
        private AppliedCourseLogic appliedCourseLogic;
        private DepartmentLogic departmentLogic;

        public PGFormViewModel()
        {
            sexLogic = new SexLogic();
            stateLogic = new StateLogic();
            lgaLogic = new LocalGovernmentLogic();
            relationshipLogic = new RelationshipLogic();
            programmeLogic = new ProgrammeLogic();
            departmentLogic = new DepartmentLogic();
            oLevelTypeLogic = new OLevelTypeLogic();
            oLevelGradeLogic = new OLevelGradeLogic();
            oLevelSubjectLogic = new OLevelSubjectLogic();
            resultGradeLogic = new ResultGradeLogic();
            educationalQualificationLogic = new EducationalQualificationLogic();
            iTDurationLogic = new ITDurationLogic();
            institutionChoiceLogic = new InstitutionChoiceLogic();
            abilityLogic = new AbilityLogic();
            religionLogic = new ReligionLogic();

            Programme = new Programme();


            Payment = new Payment();
            ApplicationProgrammeFee = new ApplicationProgrammeFee();

            InitialiseOLevelResult();

            levelLogic = new LevelLogic();
            titleLogic = new TitleLogic();
            maritalStatusLogic = new MaritalStatusLogic();
            bloodGroupLogic = new BloodGroupLogic();
            genotypeLogic = new GenotypeLogic();
            modeOfEntryLogic = new ModeOfEntryLogic();
            modeOfStudyLogic = new ModeOfStudyLogic();
            studentTypeLogic = new StudentTypeLogic();
            studentStatusLogic = new StudentStatusLogic();
            modeOfFinanceLogic = new ModeOfFinanceLogic();
            relationshipLogic = new RelationshipLogic();
            departmentOptionLogic = new DepartmentOptionLogic();
            appliedCourseLogic = new AppliedCourseLogic();
            studentCategoryLogic = new StudentCategoryLogic();

            PopulateAllApplicantDropDowns();
            PopulateAllStudentDropDowns();
        }

        public string ApplicationFormNumber { get; set; }
        public bool ApplicationAlreadyExist { get; set; }
        public bool StudentAlreadyExist { get; set; }
        public HttpPostedFileBase MyFile { get; set; }

        //public Person Person { get; set; }
        public ApplicationFormSetting ApplicationFormSetting { get; set; }

        public ApplicationProgrammeFee ApplicationProgrammeFee { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public Payment Payment { get; set; }


        public List<Sex> Genders { get; set; }
        public List<State> States { get; set; }
        public List<LocalGovernment> Lgas { get; set; }
        public List<Religion> Religions { get; set; }

        public List<Programme> Programmes { get; set; }

        public List<Department> Departments { get; set; }
        public List<Value> ExamYears { get; set; }
        public List<OLevelType> OLevelTypes { get; set; }
        public List<OLevelGrade> OLevelGrades { get; set; }
        public List<OLevelSubject> OLevelSubjects { get; set; }
        public List<Value> GraduationYears { get; set; }
        public List<ResultGrade> ResultGrades { get; set; }
        public List<EducationalQualification> EducationalQualifications { get; set; }
        public List<Ability> Abilities { get; set; }
        public List<Relationship> Relationships { get; set; }
        public List<ITDuration> ITDurations { get; set; }
        public List<InstitutionChoice> InstitutionChoices { get; set; }

        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> SexSelectList { get; set; }
        public List<SelectListItem> FacultySelectList { get; set; }
        public List<SelectListItem> RelationshipSelectList { get; set; }
        public List<SelectListItem> GraduationYearSelectList { get; set; }
        public List<SelectListItem> GraduationMonthSelectList { get; set; }
        public List<SelectListItem> ExamYearSelectList { get; set; }
        public List<SelectListItem> OLevelTypeSelectList { get; set; }
        public List<SelectListItem> OLevelGradeSelectList { get; set; }
        public List<SelectListItem> OLevelSubjectSelectList { get; set; }
        public List<SelectListItem> ResultGradeSelectList { get; set; }
        public List<SelectListItem> LocalGovernmentSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public List<SelectListItem> ReligionSelectList { get; set; }
        public List<SelectListItem> AbilitySelectList { get; set; }
        public List<SelectListItem> EducationalQualificationSelectList { get; set; }
        public List<SelectListItem> ITDurationSelectList { get; set; }
        public List<SelectListItem> JambScoreSelectList { get; set; }
        public List<SelectListItem> InstitutionChoiceSelectList { get; set; }
        public List<SelectListItem> DayOfBirthSelectList { get; set; }
        public List<SelectListItem> MonthOfBirthSelectList { get; set; }
        public List<SelectListItem> YearOfBirthSelectList { get; set; }
        public List<SelectListItem> PreviousEducationStartDaySelectList { get; set; }
        public List<SelectListItem> PreviousEducationStartMonthSelectList { get; set; }
        public List<SelectListItem> PreviousEducationStartYearSelectList { get; set; }
        public List<SelectListItem> PreviousEducationEndDaySelectList { get; set; }
        public List<SelectListItem> PreviousEducationEndMonthSelectList { get; set; }
        public List<SelectListItem> PreviousEducationEndYearSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }

        public List<Title> Titles { get; set; }
        public List<MaritalStatus> MaritalStatuses { get; set; }
        public List<BloodGroup> BloodGroups { get; set; }
        public List<Genotype> Genotypes { get; set; }
        public List<ModeOfEntry> ModeOfEntries { get; set; }
        public List<ModeOfStudy> ModeOfStudies { get; set; }
        public List<StudentType> StudentTypes { get; set; }
        public List<StudentCategory> StudentCategories { get; set; }
        public List<StudentStatus> StudentStatuses { get; set; }
        public List<Level> Levels { get; set; }
        public List<ModeOfFinance> ModeOfFinances { get; set; }
        public List<DepartmentOption> DepartmentOptions { get; set; }

        public List<SelectListItem> TitleSelectList { get; set; }
        public List<SelectListItem> MaritalStatusSelectList { get; set; }
        public List<SelectListItem> BloodGroupSelectList { get; set; }
        public List<SelectListItem> GenotypeSelectList { get; set; }
        public List<SelectListItem> ModeOfEntrySelectList { get; set; }
        public List<SelectListItem> ModeOfStudySelectList { get; set; }
        public List<SelectListItem> StudentTypeSelectList { get; set; }
        public List<SelectListItem> StudentStatusSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> ModeOfFinanceSelectList { get; set; }
        public List<SelectListItem> DepartmentOptionSelectList { get; set; }
        public List<SelectListItem> AdmissionYearSelectList { get; set; }
        public List<SelectListItem> StudentCategorySelectList { get; set; }

        public List<SelectListItem> StudentNdResultDayAwardedSelectList { get; set; }
        public List<SelectListItem> StudentNdResultMonthAwardedSelectList { get; set; }
        public List<SelectListItem> StudentNdResultYearAwardedSelectList { get; set; }

        public List<SelectListItem> StudentLastEmploymentStartDaySelectList { get; set; }
        public List<SelectListItem> StudentLastEmploymentStartMonthSelectList { get; set; }
        public List<SelectListItem> StudentLastEmploymentStartYearSelectList { get; set; }
        public List<SelectListItem> StudentLastEmploymentEndDaySelectList { get; set; }
        public List<SelectListItem> StudentLastEmploymentEndMonthSelectList { get; set; }
        public List<SelectListItem> StudentLastEmploymentEndYearSelectList { get; set; }
        public AdmissionList AdmissionList { get; set; }
        public int dayDate { get; set; }
        public int monthDate { get; set; }
        public int yearDate { get; set; }

        private void PopulateAllStudentDropDowns()
        {
            try
            {
                Titles = titleLogic.GetAll();
                MaritalStatuses = maritalStatusLogic.GetAll();
                BloodGroups = bloodGroupLogic.GetAll();
                Genotypes = genotypeLogic.GetAll();
                ModeOfEntries = modeOfEntryLogic.GetAll();
                ModeOfStudies = modeOfStudyLogic.GetAll();
                StudentTypes = studentTypeLogic.GetAll();
                StudentStatuses = studentStatusLogic.GetAll();
                Levels = levelLogic.GetAll();
                ModeOfFinances = modeOfFinanceLogic.GetAll();
                Relationships = relationshipLogic.GetAll();
                DepartmentOptions = departmentOptionLogic.GetAll();
                StudentCategories = studentCategoryLogic.GetAll();

                TitleSelectList = Utility.PopulateTitleSelectListItem();
                MaritalStatusSelectList = Utility.PopulateMaritalStatusSelectListItem();
                BloodGroupSelectList = Utility.PopulateBloodGroupSelectListItem();
                GenotypeSelectList = Utility.PopulateGenotypeSelectListItem();
                ModeOfEntrySelectList = Utility.PopulateModeOfEntrySelectListItem();
                ModeOfStudySelectList = Utility.PopulateModeOfStudySelectListItem();
                StudentTypeSelectList = Utility.PopulateStudentTypeSelectListItem();
                StudentStatusSelectList = Utility.PopulateStudentStatusSelectListItem();
                LevelSelectList = Utility.PopulateLevelSelectListItem();
                GenotypeSelectList = Utility.PopulateGenotypeSelectListItem();
                ModeOfFinanceSelectList = Utility.PopulateModeOfFinanceSelectListItem();
                RelationshipSelectList = Utility.PopulateRelationshipSelectListItem();

                StudentCategorySelectList = Utility.PopulateStudentCategorySelectListItem();
                AdmissionYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, true);

                StudentNdResultMonthAwardedSelectList = Utility.PopulateMonthSelectListItem();
                StudentNdResultYearAwardedSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);

                StudentLastEmploymentStartMonthSelectList = Utility.PopulateMonthSelectListItem();
                StudentLastEmploymentStartYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);

                StudentLastEmploymentEndMonthSelectList = Utility.PopulateMonthSelectListItem();
                StudentLastEmploymentEndYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void PopulateAllApplicantDropDowns()
        {
            const int ONE = 1;
            const int FOUR_HUNDRED = 400;

            try
            {
                Genders = sexLogic.GetAll();
                States = stateLogic.GetAll();
                Lgas = lgaLogic.GetAll();
                Relationships = relationshipLogic.GetAll();
                Programmes = programmeLogic.GetModelsBy(f=>f.Programme_Name.Contains("PG"));
                ITDurations = iTDurationLogic.GetAll();
                InstitutionChoices = institutionChoiceLogic.GetAll();
                Abilities = abilityLogic.GetAll();
                Religions = religionLogic.GetAll();

                GraduationYears = Utility.CreateYearListFrom(START_YEAR);
                ExamYears = GraduationYears;
                OLevelTypes = oLevelTypeLogic.GetAll();
                OLevelGrades = oLevelGradeLogic.GetAll();
                OLevelSubjects = oLevelSubjectLogic.GetAll();
                ResultGrades = resultGradeLogic.GetAll();
                EducationalQualifications = educationalQualificationLogic.GetAll();

                ReligionSelectList = Utility.PopulateReligionSelectListItem();
                SexSelectList = Utility.PopulateSexSelectListItem();
                StateSelectList = Utility.PopulateStateSelectListItem();
                FacultySelectList = Utility.PopulateFacultySelectListItem();

                RelationshipSelectList = Utility.PopulateRelationshipSelectListItem();
                GraduationYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, true);
                GraduationMonthSelectList = Utility.PopulateGraduationMonthSelectListItem();
                ExamYearSelectList = Utility.PopulateExamYearSelectListItem(START_YEAR);
                OLevelTypeSelectList = Utility.PopulateOLevelTypeSelectListItem();
                OLevelGradeSelectList = Utility.PopulateOLevelGradeSelectListItem();
                OLevelSubjectSelectList = Utility.PopulateOLevelSubjectSelectListItem();
                ResultGradeSelectList = Utility.PopulateResultGradeSelectListItem();
                LocalGovernmentSelectList = Utility.PopulateLocalGovernmentSelectListItem();
                AbilitySelectList = Utility.PopulateAbilitySelectListItem();
                EducationalQualificationSelectList = Utility.PopulateEducationalQualificationSelectListItem();
                ITDurationSelectList = Utility.PopulateITDurationSelectListItem();
                JambScoreSelectList = Utility.PopulateJambScoreSelectListItem(ONE, FOUR_HUNDRED);
                InstitutionChoiceSelectList = Utility.PopulateInstitutionChoiceSelectListItem();
                ProgrammeSelectList = Utility.PopulatePostGraduateProgrammeSelectListItem();

                MonthOfBirthSelectList = Utility.PopulateMonthSelectListItem();
                YearOfBirthSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);

                PreviousEducationStartMonthSelectList = Utility.PopulateMonthSelectListItem();
                PreviousEducationStartYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);
                PreviousEducationEndMonthSelectList = Utility.PopulateMonthSelectListItem();
                PreviousEducationEndYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR, false);
            }
            catch (Exception)
            {
                throw;
            }
        }

       
    }
}