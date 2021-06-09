using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Models
{
    public class Utility
    {
        public const string ID = "Id";
        public const string NAME = "Name";
        public const string VALUE = "Value";
        public const string TEXT = "Text";
        public const string Select = "-- Select --";
        public const string DEFAULT_AVATAR = "/Content/Images/default_avatar.png";
        public const string SelectDepartment = "-- Select Department --";
        public const string SelectAdmissiontype = "-- Select Admission Type --";
        public const string SelectSession = "-- Select Session --";
        public const string SelectSemester = "-- Select Semester --";
        public const string SelectLevel = "-- Select Level --";
        public const string SelectFeeType = "-- Select Fee Type --";
        public const string SelectProgramme = "-- Select Programme --";
        public const string SelectCourse = "-- Select Course --";

        public const string FIRST_SITTING = "FIRST SITTING";
        public const string SECOND_SITTING = "SECOND SITTING";

        public static void BindDropdownItem<T>(DropDownList dropDownList, T items, string dataValueField,
            string dataTextField)
        {
            dropDownList.Items.Clear();

            dropDownList.DataValueField = dataValueField;
            dropDownList.DataTextField = dataTextField;

            dropDownList.DataSource = items;
            dropDownList.DataBind();
        }

        public static List<Value> CreateYearListFrom(int startYear)
        {
            var years = new List<Value>();

            try
            {
                DateTime currentDate = DateTime.Now;
                int currentYear = currentDate.Year;

                for (int i = startYear; i <= currentYear; i++)
                {
                    var value = new Value();
                    value.Id = i;
                    value.Name = i.ToString();
                    years.Add(value);
                }

                //years.Insert(0, new Value() { Id = 0, Name = Select });
                return years;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Value> CreateYearListFrom(int startYear, int endYear)
        {
            var years = new List<Value>();

            try
            {
                for (int i = startYear; i <= endYear; i++)
                {
                    var value = new Value();
                    value.Id = i;
                    value.Name = i.ToString();
                    years.Add(value);
                }

                //years.Insert(0, new Value() { Id = 0, Name = Select });
                return years;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Value> CreateNumberListFrom(int startValue, int endValue)
        {
            var values = new List<Value>();

            try
            {
                for (int i = startValue; i <= endValue; i++)
                {
                    var value = new Value();
                    value.Id = i;
                    value.Name = i.ToString();
                    values.Add(value);
                }

                //values.Insert(0, new Value() { Id = 0, Name = Select });
                return values;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Value> GetMonthsInYear()
        {
            var values = new List<Value>();

            try
            {
                string[] names = DateTimeFormatInfo.CurrentInfo.MonthNames;

                for (int i = 0; i < names.Length; i++)
                {
                    int j = i + 1;
                    var value = new Value();
                    value.Id = j;
                    value.Name = names[i];
                    values.Add(value);
                }

                //values.Insert(0, new Value() { Id = 0, Name = Select });
                return values;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateGraduationMonthSelectListItem()
        {
            try
            {
                List<Value> months = GetMonthsInYear();
                if (months == null || months.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var monthList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                monthList.Add(list);

                foreach (Value month in months)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = month.Id.ToString();
                    selectList.Text = month.Name;

                    monthList.Add(selectList);
                }

                return monthList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateApplicantStatusSelectListItem()
        {
            try
            {
                var studentStatusLogic = new ApplicantStatusLogic();
                List<ApplicantStatus> studentStatuses = studentStatusLogic.GetAll();

                if (studentStatuses == null || studentStatuses.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var studentStatusList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                studentStatusList.Add(list);

                foreach (ApplicantStatus studentStatus in studentStatuses)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = studentStatus.Id.ToString();
                    selectList.Text = studentStatus.Name;

                    studentStatusList.Add(selectList);
                }

                return studentStatusList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateExamYearSelectListItem(int startYear)
        {
            try
            {
                List<Value> years = CreateYearListFrom(startYear);
                if (years == null || years.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var yearList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "0";
                list.Text = Select;
                yearList.Add(list);

                foreach (Value year in years)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = year.Id.ToString();
                    selectList.Text = year.Name;

                    yearList.Add(selectList);
                }

                return yearList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateJambScoreSelectListItem(int startScore, int endScore)
        {
            try
            {
                List<Value> scores = CreateNumberListFrom(startScore, endScore);
                if (scores == null || scores.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var yearList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                yearList.Add(list);

                foreach (Value score in scores)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = score.Id.ToString();
                    selectList.Text = score.Name;

                    yearList.Add(selectList);
                }

                return yearList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateYearSelectListItem(int startYear, bool withSelect)
        {
            try
            {
                int END_YEAR = DateTime.Now.Year + 6;
                List<Value> years = CreateYearListFrom(startYear, END_YEAR);
                if (years == null || years.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var yearList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                if (withSelect)
                {
                    list.Text = Select;
                }
                else
                {
                    list.Text = "--YY--";
                }

                yearList.Add(list);

                foreach (Value year in years)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = year.Id.ToString();
                    selectList.Text = year.Name;

                    yearList.Add(selectList);
                }

                return yearList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateYearofGraduationSelectListItem(int startYear, int endYear,
            bool withSelect)
        {
            try
            {
                List<Value> years = CreateYearListFrom(startYear, endYear);
                if (years == null || years.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var yearList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                if (withSelect)
                {
                    list.Text = Select;
                }
                else
                {
                    list.Text = "--YY--";
                }

                yearList.Add(list);

                foreach (Value year in years)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = year.Id.ToString();
                    selectList.Text = year.Name;

                    yearList.Add(selectList);
                }

                return yearList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateSexSelectListItem()
        {
            try
            {
                var sexLogic = new SexLogic();
                List<Sex> genders = sexLogic.GetAll();
                if (genders == null || genders.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var sexList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                sexList.Add(list);

                foreach (Sex sex in genders)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = sex.Id.ToString();
                    selectList.Text = sex.Name;

                    sexList.Add(selectList);
                }

                return sexList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateVerificationFeeTypeSelectListItem()
        {
            try
            {
                var feeTypeLogic = new FeeTypeLogic();
                Expression<Func<FEE_TYPE, bool>> selector =
                    f => f.Fee_Type_Id == 13 || f.Fee_Type_Id == 50 || f.Fee_Type_Id == 54;
                List<FeeType> feeTypes = feeTypeLogic.GetModelsBy(selector);
                if (feeTypes == null || feeTypes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var sexList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                sexList.Add(list);

                foreach (FeeType feeType in feeTypes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = feeType.Id.ToString();
                    selectList.Text = feeType.Name;

                    sexList.Add(selectList);


                }

                return new List<SelectListItem>(sexList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateFeeTypeSelectListItem()
        {
            try
            {
                var feeTypeLogic = new FeeTypeLogic();
                List<FeeType> feeTypes = feeTypeLogic.GetAll();
                if (feeTypes == null || feeTypes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var sexList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                sexList.Add(list);

                foreach (FeeType feeType in feeTypes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = feeType.Id.ToString();
                    selectList.Text = feeType.Name;

                    sexList.Add(selectList);
                }

                return new List<SelectListItem>(sexList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }   

        public static List<SelectListItem> PopulateFeeTypeActiveSelectListItem()
        {
            try
            {
                var feeTypeLogic = new FeeTypeLogic();
                List<FeeType> feeTypes = feeTypeLogic.GetModelsBy(a => a.Active == true );
                if (feeTypes == null || feeTypes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var sexList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                sexList.Add(list);

                foreach (FeeType feeType in feeTypes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = feeType.Id.ToString();
                    selectList.Text = feeType.Name;

                    sexList.Add(selectList);
                }

                return new List<SelectListItem>(sexList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateFeeSelectListItem()
        {
            try
            {
                var feeLogic = new FeeLogic();
                List<Fee> fees = feeLogic.GetAll();
                if (fees == null || fees.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var feeList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                feeList.Add(list);

                foreach (Fee fee in fees)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = fee.Id.ToString();
                    selectList.Text = fee.Name + " - " + fee.Amount;

                    feeList.Add(selectList);
                }

                return new List<SelectListItem>(feeList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }
       

        public static List<SelectListItem> PopulateEducationalQualificationSelectListItem()
        {
            try
            {
                var qualificationLogic = new EducationalQualificationLogic();
                List<EducationalQualification> educationalQualifications = qualificationLogic.GetAll();
                if (educationalQualifications == null || educationalQualifications.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var educationalQualificationList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                educationalQualificationList.Add(list);

                foreach (EducationalQualification educationalQualification in educationalQualifications)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = educationalQualification.Id.ToString();
                    selectList.Text = educationalQualification.ShortName;

                    educationalQualificationList.Add(selectList);
                }

                return new List<SelectListItem>(educationalQualificationList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateAbilitySelectListItem()
        {
            try
            {
                var abilityLogic = new AbilityLogic();
                List<Ability> abilities = abilityLogic.GetAll();
                if (abilities == null && abilities.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var abilityList = new List<SelectListItem>();

                if (abilities != null || abilities.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    abilityList.Add(list);

                    foreach (Ability ability in abilities)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = ability.Id.ToString();
                        selectList.Text = ability.Name;

                        abilityList.Add(selectList);
                    }
                }

                return new List<SelectListItem>(abilityList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateReligionSelectListItem()
        {
            try
            {
                var religionLogic = new ReligionLogic();
                List<Religion> religions = religionLogic.GetAll();
                if (religions == null || religions.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var religionList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                religionList.Add(list);

                foreach (Religion religion in religions)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = religion.Id.ToString();
                    selectList.Text = religion.Name;

                    religionList.Add(selectList);
                }

                return new List<SelectListItem>(religionList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateStateSelectListItem()
        {
            try
            {
                var stateLogic = new StateLogic();
                List<State> states = stateLogic.GetAll();
                if (states == null || states.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                stateList.Add(list);

                foreach (State state in states)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = state.Id;
                    selectList.Text = state.Name;

                    stateList.Add(selectList);
                }

                return new List<SelectListItem>(stateList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateLocalGovernmentSelectListItem()
        {
            try
            {
                var localGovernmentLogic = new LocalGovernmentLogic();
                List<LocalGovernment> lgas = localGovernmentLogic.GetAll();

                if (lgas == null || lgas.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                stateList.Add(list);

                foreach (LocalGovernment lga in lgas)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = lga.Id.ToString();
                    selectList.Text = lga.Name;

                    stateList.Add(selectList);
                }

                return new List<SelectListItem>(stateList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateLocalGovernmentSelectListItemByStateId(string id)
        {
            try
            {
                var localGovernmentLogic = new LocalGovernmentLogic();
                List<LocalGovernment> lgas = localGovernmentLogic.GetModelsBy(l => l.State_Id == id);

                if (lgas == null || lgas.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                stateList.Add(list);

                foreach (LocalGovernment lga in lgas)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = lga.Id.ToString();
                    selectList.Text = lga.Name;

                    stateList.Add(selectList);
                }

                return new List<SelectListItem>(stateList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateFacultySelectListItem()
        {
            try
            {
                var facultyLogic = new FacultyLogic();
                List<Faculty> faculties = facultyLogic.GetAll();
                if (faculties == null || faculties.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                stateList.Add(list);

                foreach (Faculty faculty in faculties)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = faculty.Id.ToString();
                    selectList.Text = faculty.Name;

                    stateList.Add(selectList);
                }

                return new List<SelectListItem>(stateList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateAllDepartmentSelectListItem()
        {
            try
            {
                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetAll();

                if (departments == null || departments.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();
                if (departments != null && departments.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    stateList.Add(list);

                    foreach (Department department in departments)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = department.Id.ToString();
                        selectList.Text = department.Name;
                        stateList.Add(selectList);
                    }
                }

                return new List<SelectListItem>(stateList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateDepartmentSelectListItem(Programme programme)
        {
            try
            {
                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                if (departments == null || departments.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();
                if (departments != null && departments.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    stateList.Add(list);

                    foreach (Department department in departments)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = department.Id.ToString();
                        selectList.Text = department.Name;

                        stateList.Add(selectList);
                    }
                }

                return new List<SelectListItem>(stateList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateDepartmentSelectListItem()
        {
            try
            {
                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetAll();

                if (departments == null || departments.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();
                if (departments != null && departments.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    stateList.Add(list);

                    foreach (Department department in departments)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = department.Id.ToString();
                        selectList.Text = department.Name;

                        stateList.Add(selectList);
                    }
                }

                return new List<SelectListItem>(stateList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateDepartmentSelectListItemByFacultyId(int id)
        {
            try
            {
                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetModelsBy(l => l.Faculty_Id == id);

                if (departments == null || departments.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                stateList.Add(list);

                foreach (Department department in departments)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = department.Id.ToString();
                    selectList.Text = department.Name;

                    stateList.Add(selectList);
                }

                return new List<SelectListItem>(stateList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Department> PopulateDepartmentByFacultyId(int id)
        {
            try
            {
                var departmentLogic = new DepartmentLogic();
                return departmentLogic.GetModelsBy(l => l.Faculty_Id == id);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Department> PopulateDepartmentById(int id)
        {
            try
            {
                var departmentLogic = new DepartmentLogic();
                return departmentLogic.GetModelsBy(l => l.Department_Id == id);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateRelationshipSelectListItem()
        {
            try
            {
                var relationshipLogic = new RelationshipLogic();
                List<Relationship> relationships = relationshipLogic.GetAll();

                if (relationships == null || relationships.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                stateList.Add(list);

                foreach (Relationship relationship in relationships)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = relationship.Id.ToString();
                    selectList.Text = relationship.Name;

                    stateList.Add(selectList);
                }

                return new List<SelectListItem>(stateList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateOLevelTypeSelectListItem()
        {
            try
            {
                var oLevelTypeLogic = new OLevelTypeLogic();
                List<OLevelType> oLevelTypes = oLevelTypeLogic.GetAll();

                if (oLevelTypes == null || oLevelTypes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "0";
                list.Text = Select;
                stateList.Add(list);

                foreach (OLevelType oLevelType in oLevelTypes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = oLevelType.Id.ToString();
                    selectList.Text = oLevelType.Name;

                    stateList.Add(selectList);
                }

                return new List<SelectListItem>(stateList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateOLevelGradeSelectListItem()
        {
            try
            {
                var oLevelGradeLogic = new OLevelGradeLogic();
                List<OLevelGrade> oLevelGrades = oLevelGradeLogic.GetAll();

                if (oLevelGrades == null || oLevelGrades.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "0";
                list.Text = Select;
                selectItemList.Add(list);

                foreach (OLevelGrade oLevelGrade in oLevelGrades)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = oLevelGrade.Id.ToString();
                    selectList.Text = oLevelGrade.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateOLevelSubjectSelectListItem()
        {
            try
            {
                var oLevelSubjectLogic = new OLevelSubjectLogic();
                List<OLevelSubject> oLevelSubjects = oLevelSubjectLogic.GetAll();

                if (oLevelSubjects == null || oLevelSubjects.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "0";
                list.Text = Select;
                selectItemList.Add(list);

                foreach (OLevelSubject oLevelSubject in oLevelSubjects)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = oLevelSubject.Id.ToString();
                    selectList.Text = oLevelSubject.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateResultGradeSelectListItem()
        {
            try
            {
                var resultGradeLogic = new ResultGradeLogic();
                List<ResultGrade> resultGrades = resultGradeLogic.GetAll();

                if (resultGrades == null || resultGrades.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                selectItemList.Add(list);

                foreach (ResultGrade resultGrade in resultGrades)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = resultGrade.Id.ToString();
                    selectList.Text = resultGrade.LevelOfPass;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateAllProgrammeSelectListItem()
        {
            try
            {
                var programmeLogic = new ProgrammeLogic();
                List<Programme> programmes = programmeLogic.GetAll().OrderBy(a => a.Name).ToList();

                if (programmes == null || programmes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = SelectProgramme;
                selectItemList.Add(list);

                foreach (Programme programme in programmes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = programme.Id.ToString();
                    selectList.Text = programme.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateProgrammeSelectListItem()
        {
            try
            {
                var programmeLogic = new ProgrammeLogic();
                List<Programme> programmes =
                    programmeLogic.GetModelsBy(p => p.Activated == true).OrderBy(a => a.Name).ToList();

                if (programmes == null || programmes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = SelectProgramme;
                selectItemList.Add(list);

                foreach (Programme programme in programmes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = programme.Id.ToString();
                    selectList.Text = programme.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SelectListItem> PopulatePostGraduateProgrammeSelectListItem()
        {
            try
            {
                var programmeLogic = new ProgrammeLogic();
                List<Programme> programmes =
                    programmeLogic.GetModelsBy(p => p.Activated == true && p.Programme_Name.Contains("PG")).OrderBy(a => a.Name).ToList();

                if (programmes == null || programmes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = SelectProgramme;
                selectItemList.Add(list);

                foreach (Programme programme in programmes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = programme.Id.ToString();
                    selectList.Text = programme.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateUndergraduateProgrammeSelectListItem()
        {
            try
            {
                var programmeLogic = new ProgrammeLogic();
                List<Programme> programmes =
                    programmeLogic.GetModelsBy(p => p.Programme_Id == 1).OrderBy(a => a.Name).ToList();

                if (programmes == null || programmes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = SelectProgramme;
                selectItemList.Add(list);

                foreach (Programme programme in programmes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = programme.Id.ToString();
                    selectList.Text = programme.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateInstitutionChoiceSelectListItem()
        {
            try
            {
                var institutionChoiceLogic = new InstitutionChoiceLogic();
                List<InstitutionChoice> institutionChoices = institutionChoiceLogic.GetAll();

                if (institutionChoices == null || institutionChoices.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                selectItemList.Add(list);

                foreach (InstitutionChoice institutionChoice in institutionChoices)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = institutionChoice.Id.ToString();
                    selectList.Text = institutionChoice.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateITDurationSelectListItem()
        {
            try
            {
                var itDurationLogic = new ITDurationLogic();
                List<ITDuration> iTDurations = itDurationLogic.GetAll();

                if (iTDurations == null || iTDurations.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                selectItemList.Add(list);

                foreach (ITDuration iTDuration in iTDurations)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = iTDuration.Id.ToString();
                    selectList.Text = iTDuration.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateDepartmentListItem(long programmeId)
        {
            try
            {
                List<SelectListItem> DepartmentSL = new List<SelectListItem>();
                var programmeDepartmentLogic = new ProgrammeDepartmentLogic();
                var departmentList = programmeDepartmentLogic.GetModelsBy(s => s.Programme_Id == programmeId && s.Activate == true)
                                                     .Select(s => new SelectListItem()
                                                     {
                                                         Value = s.Department.Id.ToString(),
                                                         Text = s.Department.Name
                                                     })
                                                     .ToList();

                if (departmentList?.Count() > 0)
                {
                    DepartmentSL.Add(new SelectListItem() { Value = "", Text = "----Select Department----" });
                    DepartmentSL.AddRange(departmentList);
                }
                return DepartmentSL;
            }
            catch (Exception ex) { throw ex; }
        }

        public static List<SelectListItem> PopulateActiveSessionListItem()
        {
            try
            {
                List<SelectListItem> sessions = new List<SelectListItem>();
                var sessionLogic = new SessionLogic();
                var activeSessions = sessionLogic.GetModelsBy(s => s.Activated == true)
                                                 .Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Name })
                                                 .ToList();

                sessions.Add(new SelectListItem() { Value = "", Text = "---Select Session----" });
                if (activeSessions?.Count() > 0)
                {   
                    sessions.AddRange(activeSessions);
                }

                return sessions;
            }
            catch (Exception ex) { throw ex; }
        }

        public static List<SelectListItem> PopulateSessionSelectListItem()
        {
            try
            {
                var sessionLogic = new SessionLogic();
                List<Session> sessions = sessionLogic.GetAll();

                if (sessions == null || sessions.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = SelectSession;
                selectItemList.Add(list);

                foreach (Session session in sessions)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = session.Id.ToString();
                    selectList.Text = session.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public static List<SelectListItem> PopulateAllGstAnswerSelectListItem()
        //{
        //    try
        //    {

        //        //var GstScanAnswerLogic = new GstScanAnswerLogic();
        //        //var scanAnswers = GstScanAnswerLogic.GetAll().Select(s => s.description).Distinct();
        //        //if(scanAnswers == null || scanAnswers.Count() <= 0)
        //        //{
        //        //    return new List<SelectListItem>();
        //        //}

        //        //var selectItemList = new List<SelectListItem>();

        //        //var list = new SelectListItem();
        //        //list.Value = "";
        //        //list.Text = "Select Answer";
        //        //selectItemList.Add(list);

        //        //foreach(var answer in scanAnswers)
        //        //{
        //        //    var selectList = new SelectListItem();
        //        //    selectList.Value = answer;
        //        //    selectList.Text = answer;

        //        //    selectItemList.Add(selectList);
        //        //}

        //        //return selectItemList;
        //    }
        //    catch(Exception)
        //    {
        //        throw;
        //    }
        //}

        public static List<SelectListItem> PopulateAllSessionSelectListItem()
        {
            try
            {
                var sessionLogic = new SessionLogic();
                List<Session> sessions = sessionLogic.GetAll().OrderByDescending(k => k.Name).ToList();

                if (sessions == null || sessions.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = SelectSession;
                selectItemList.Add(list);

                foreach (Session session in sessions)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = session.Id.ToString();
                    selectList.Text = session.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateApplicationFormSetting(Session session)
        {
            try
            {
                var applicationFormSettingLogic = new ApplicationFormSettingLogic();
                List<ApplicationFormSetting> applicationFormSettings =
                    applicationFormSettingLogic.GetModelsBy(a => a.End_Date >= DateTime.Now).ToList();

                if (applicationFormSettings == null || applicationFormSettings.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                selectItemList.Add(list);

                foreach (ApplicationFormSetting applicationFormSetting in applicationFormSettings)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = applicationFormSetting.Id.ToString();
                    selectList.Text = applicationFormSetting.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateHostels()
        {
            try
            {
                var hostelLogic = new HostelLogic();
                List<Hostel> hostels = hostelLogic.GetModelsBy(a => a.Activated).OrderByDescending(k => k.Name).ToList();

                if (hostels == null || hostels.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                selectItemList.Add(list);

                foreach (Hostel hostel in hostels)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = hostel.Id.ToString();
                    selectList.Text = hostel.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateHostelSeries(Hostel hosetl)
        {
            try
            {
                var hostelSeriesLogic = new HostelSeriesLogic();
                List<HostelSeries> hostelSeries =
                    hostelSeriesLogic.GetModelsBy(a => a.Hostel_Id == hosetl.Id && a.Activated)
                        .OrderByDescending(k => k.Name)
                        .ToList();

                if (hostelSeries == null || hostelSeries.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                selectItemList.Add(list);

                foreach (HostelSeries series in hostelSeries)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = series.Id.ToString();
                    selectList.Text = series.Name;

                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateHostelRooms(HostelSeries HostelSeries)
        {
            try
            {
                var hostelRoomLogic = new HostelRoomLogic();
                List<HostelRoom> hostelRooms =
                    hostelRoomLogic.GetModelsBy(a => a.Series_Id == HostelSeries.Id && a.Activated)
                        .OrderByDescending(k => k.Number)
                        .ToList();

                if (hostelRooms == null || hostelRooms.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var selectItemList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                selectItemList.Add(list);

                foreach (HostelRoom room in hostelRooms)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = room.Id.ToString();
                    selectList.Text = room.Number;
                    selectItemList.Add(selectList);
                }

                return new List<SelectListItem>(selectItemList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public static List<SelectListItem> PopulateHostels()
        //{
        //    try
        //    {
        //        HostelLogic hostelLogic = new HostelLogic();
        //        List<Hostel> hostels = hostelLogic.GetModelsBy(a=> a.Activated == true).OrderByDescending(k => k.Name).ToList();

        //        if (hostels == null || hostels.Count <= 0)
        //        {
        //            return new List<SelectListItem>();
        //        }

        //        List<SelectListItem> selectItemList = new List<SelectListItem>();

        //        SelectListItem list = new SelectListItem();
        //        list.Value = "";
        //        list.Text = Select;
        //        selectItemList.Add(list);

        //        foreach (Hostel hostel in hostels)
        //        {
        //            SelectListItem selectList = new SelectListItem();
        //            selectList.Value = hostel.Id.ToString();
        //            selectList.Text = hostel.Name;

        //            selectItemList.Add(selectList);
        //        }

        //        return selectItemList;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //public static List<SelectListItem> PopulateHostelSeries(Hostel hosetl)
        //{
        //    try
        //    {
        //        HostelSeriesLogic hostelSeriesLogic = new HostelSeriesLogic();
        //        List<HostelSeries> hostelSeries = hostelSeriesLogic.GetModelsBy(a=> a.Hostel_Id == hosetl.Id && a.Activated == true).OrderByDescending(k => k.Name).ToList();

        //        if (hostelSeries == null || hostelSeries.Count <= 0)
        //        {
        //            return new List<SelectListItem>();
        //        }

        //        List<SelectListItem> selectItemList = new List<SelectListItem>();

        //        SelectListItem list = new SelectListItem();
        //        list.Value = "";
        //        list.Text = Select;
        //        selectItemList.Add(list);

        //        foreach (HostelSeries series in hostelSeries)
        //        {
        //            SelectListItem selectList = new SelectListItem();
        //            selectList.Value = series.Id.ToString();
        //            selectList.Text = series.Name;

        //            selectItemList.Add(selectList);
        //        }

        //        return selectItemList;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public static List<SelectListItem> PopulateHostelRooms(HostelSeries HostelSeries)
        //{
        //    try
        //    {
        //        HostelRoomLogic hostelRoomLogic = new HostelRoomLogic();
        //        List<HostelRoom> hostelRooms = hostelRoomLogic.GetModelsBy(a=> a.Series_Id == HostelSeries.Id && a.Activated == true).OrderByDescending(k => k.Number).ToList();

        //        if (hostelRooms == null || hostelRooms.Count <= 0)
        //        {
        //            return new List<SelectListItem>();
        //        }

        //        List<SelectListItem> selectItemList = new List<SelectListItem>();

        //        SelectListItem list = new SelectListItem();
        //        list.Value = "";
        //        list.Text = Select;
        //        selectItemList.Add(list);

        //        foreach (HostelRoom room in hostelRooms)
        //        {
        //            SelectListItem selectList = new SelectListItem();
        //            selectList.Value = room.Id.ToString();
        //            selectList.Text = room.Number;
        //            selectItemList.Add(selectList);
        //        }

        //        return selectItemList;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public static List<Session> GetAllSessions()
        {
            try
            {
                var sessionLogic = new SessionLogic();
                List<Session> sessions = sessionLogic.GetAll().OrderBy(a => a.Name).ToList();

                if (sessions != null && sessions.Count > 0)
                {
                    sessions.Insert(0, new Session { Id = 0, Name = SelectSession });
                }

                return sessions;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<Hostel> GetAllHostels()
        {
            try
            {
                var hostelLogic = new HostelLogic();
                List<Hostel> hostels = hostelLogic.GetAll().OrderBy(a => a.Name).ToList();

                if (hostels.Count > 0)
                {
                    hostels.Insert(0, new Hostel() { Id = 0, Name = Select });
                }

                return hostels;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<SessionSemester> GetAllSessionSemesters()
        {
            try
            {
                var sessionSemesterLogic = new SessionSemesterLogic();
                List<SessionSemester> sessionSemesters = sessionSemesterLogic.GetAll();

                if (sessionSemesters != null && sessionSemesters.Count > 0)
                {
                    sessionSemesters.Insert(0, new SessionSemester { Id = 0, Name = SelectSemester });
                }

                return sessionSemesters;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Semester> GetAllSemesters()
        {
            try
            {
                var sessionSemesterLogic = new SemesterLogic();
                List<Semester> sessionSemesters = sessionSemesterLogic.GetAll();

                if (sessionSemesters != null && sessionSemesters.Count > 0)
                {
                    sessionSemesters.Insert(0, new Semester { Id = 0, Name = SelectSemester });
                }

                return sessionSemesters;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<Department> GetAllDepartments()
        {
            try
            {
                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetAll();

                if (departments != null && departments.Count > 0)
                {
                    departments.Insert(0, new Department { Id = 0, Name = SelectDepartment });
                }

                return new List<Department>(departments.OrderBy(x => x.Name));
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<Level> GetAllLevels()
        {
            try
            {
                var levelLogic = new LevelLogic();
                List<Level> levels = levelLogic.GetAll();

                if (levels != null && levels.Count > 0)
                {
                    levels.Insert(0, new Level { Id = 0, Name = SelectLevel });
                }

                return levels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<Faculty> GetAllFaculties()
        {
            try
            {
                var facultyLogic = new FacultyLogic();
                List<Faculty> faculties = facultyLogic.GetAll();

                if (faculties != null && faculties.Count > 0)
                {
                    faculties.Insert(0, new Faculty { Id = 0, Name = "-- Select Faculty --" });
                }

                return new List<Faculty>(faculties.OrderBy(x => x.Name));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<FeeType> GetAllFeeTypes()
        {
            try
            {
                var feeTypeLogic = new FeeTypeLogic();
                List<FeeType> feeTypes = feeTypeLogic.GetAll();

                if (feeTypes != null && feeTypes.Count > 0)
                {
                    feeTypes.Insert(0, new FeeType { Id = 0, Name = SelectFeeType });
                }

                return new List<FeeType>(feeTypes.OrderBy(x => x.Name));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Programme> GetAllProgrammes()
        {
            try
            {
                var programmeLogic = new ProgrammeLogic();
                List<Programme> programmes = programmeLogic.GetAll();

                if (programmes != null && programmes.Count > 0)
                {
                    //programmes.Add(new Programme() { Id = -100, Name = "All" });
                    programmes.Insert(0, new Programme { Id = 0, Name = SelectProgramme });
                }

                return new List<Programme>(programmes.OrderBy(x => x.Name));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateMonthSelectListItem()
        {
            try
            {
                var months = new List<Value>();
                var january = new Value { Id = 1, Name = "January" };
                var february = new Value { Id = 2, Name = "February" };
                var march = new Value { Id = 3, Name = "March" };
                var april = new Value { Id = 4, Name = "April" };
                var may = new Value { Id = 5, Name = "May" };
                var june = new Value { Id = 6, Name = "June" };
                var july = new Value { Id = 7, Name = "July" };
                var august = new Value { Id = 8, Name = "August" };
                var september = new Value { Id = 9, Name = "September" };
                var october = new Value { Id = 10, Name = "October" };
                var november = new Value { Id = 11, Name = "November" };
                var december = new Value { Id = 12, Name = "December" };

                months.Add(january);
                months.Add(february);
                months.Add(march);
                months.Add(april);
                months.Add(may);
                months.Add(june);
                months.Add(july);
                months.Add(august);
                months.Add(september);
                months.Add(october);
                months.Add(november);
                months.Add(december);

                var monthList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = "--MM--";
                monthList.Add(list);

                foreach (Value month in months)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = month.Id.ToString();
                    selectList.Text = month.Name;

                    monthList.Add(selectList);
                }

                return monthList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateDaySelectListItem(Value month, Value year)
        {
            try
            {
                List<Value> days = GetNumberOfDaysInMonth(month, year);

                var dayList = new List<SelectListItem>();
                var list = new SelectListItem();
                list.Value = "";
                list.Text = "--DD--";

                dayList.Add(list);
                foreach (Value day in days)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = day.Id.ToString();
                    selectList.Text = day.Name;

                    dayList.Add(selectList);
                }

                return dayList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Value> GetNumberOfDaysInMonth(Value month, Value year)
        {
            try
            {
                int noOfDays = DateTime.DaysInMonth(year.Id, month.Id);
                List<Value> days = CreateNumberListFrom(1, noOfDays);
                return days;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsDateInTheFuture(DateTime date)
        {
            try
            {
                TimeSpan difference = DateTime.Now - date;
                if (difference.Days <= 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsStartDateGreaterThanEndDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                TimeSpan difference = endDate - startDate;
                if (difference.Days <= 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsDateOutOfRange(DateTime startDate, DateTime endDate, int noOfDays)
        {
            try
            {
                TimeSpan difference = endDate - startDate;
                if (difference.Days < noOfDays)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateTitleSelectListItem()
        {
            try
            {
                var titleLogic = new TitleLogic();
                List<Title> titles = titleLogic.GetAll();

                if (titles == null || titles.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var titlesList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                titlesList.Add(list);

                foreach (Title title in titles)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = title.Id.ToString();
                    selectList.Text = title.Name;

                    titlesList.Add(selectList);
                }

                return titlesList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateMaritalStatusSelectListItem()
        {
            try
            {
                var maritalStatusLogic = new MaritalStatusLogic();
                List<MaritalStatus> maritalStatuses = maritalStatusLogic.GetAll();

                if (maritalStatuses == null || maritalStatuses.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var maritalStatusesList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                maritalStatusesList.Add(list);

                foreach (MaritalStatus title in maritalStatuses)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = title.Id.ToString();
                    selectList.Text = title.Name;

                    maritalStatusesList.Add(selectList);
                }

                return maritalStatusesList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateBloodGroupSelectListItem()
        {
            try
            {
                var bloodGroupLogic = new BloodGroupLogic();
                List<BloodGroup> bloodGroups = bloodGroupLogic.GetAll();

                if (bloodGroups == null || bloodGroups.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var bloodGroupsList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "0";
                list.Text = Select;
                bloodGroupsList.Add(list);

                foreach (BloodGroup bloodGroup in bloodGroups)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = bloodGroup.Id.ToString();
                    selectList.Text = bloodGroup.Name;

                    bloodGroupsList.Add(selectList);
                }

                return bloodGroupsList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulatePaymentModeFirstYearSelectListItem()
        {
            try
            {
                var paymentModeLogic = new PaymentModeLogic();
                List<PaymentMode> paymentModes = paymentModeLogic.GetModelsBy(a => a.Payment_Mode_Id == 1 || a.Payment_Mode_Id == 2);

                if (paymentModes == null || paymentModes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var paymentModesList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "0";
                list.Text = Select;
                paymentModesList.Add(list);

                foreach (PaymentMode paymentMode in paymentModes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = paymentMode.Id.ToString();
                    selectList.Text = paymentMode.Name;

                    paymentModesList.Add(selectList);
                }

                return paymentModesList;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<SelectListItem> PopulateGenotypeSelectListItem()
        {
            try
            {
                var genotypeLogic = new GenotypeLogic();
                List<Genotype> genotypes = genotypeLogic.GetAll();

                if (genotypes == null || genotypes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var genotypeList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "0";
                list.Text = Select;
                genotypeList.Add(list);

                foreach (Genotype genotype in genotypes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = genotype.Id.ToString();
                    selectList.Text = genotype.Name;

                    genotypeList.Add(selectList);
                }

                return genotypeList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateModeOfEntrySelectListItem()
        {
            try
            {
                var modeOfEntryLogic = new ModeOfEntryLogic();
                List<ModeOfEntry> modeOfEntries = modeOfEntryLogic.GetAll();

                if (modeOfEntries == null || modeOfEntries.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var modeOfEntryList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                modeOfEntryList.Add(list);

                foreach (ModeOfEntry modeOfEntry in modeOfEntries)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = modeOfEntry.Id.ToString();
                    selectList.Text = modeOfEntry.Name;

                    modeOfEntryList.Add(selectList);
                }

                return modeOfEntryList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateModeOfStudySelectListItem()
        {
            try
            {
                var modeOfStudyLogic = new ModeOfStudyLogic();
                List<ModeOfStudy> modeOfStudies = modeOfStudyLogic.GetAll();

                if (modeOfStudies == null || modeOfStudies.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var modeOfStudyList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                modeOfStudyList.Add(list);

                foreach (ModeOfStudy modeOfStudy in modeOfStudies)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = modeOfStudy.Id.ToString();
                    selectList.Text = modeOfStudy.Name;

                    modeOfStudyList.Add(selectList);
                }

                return modeOfStudyList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateStudentTypeSelectListItem()
        {
            try
            {
                var studentTypeLogic = new StudentTypeLogic();
                List<StudentType> studentTypes = studentTypeLogic.GetAll();

                if (studentTypes == null || studentTypes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var studentTypeList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                studentTypeList.Add(list);

                foreach (StudentType studentType in studentTypes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = studentType.Id.ToString();
                    selectList.Text = studentType.Name;

                    studentTypeList.Add(selectList);
                }

                return studentTypeList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateStudentStatusSelectListItem()
        {
            try
            {
                var studentStatusLogic = new StudentStatusLogic();
                List<StudentStatus> studentStatuses = studentStatusLogic.GetAll();

                if (studentStatuses == null || studentStatuses.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var studentStatusList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                studentStatusList.Add(list);

                foreach (StudentStatus studentStatus in studentStatuses)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = studentStatus.Id.ToString();
                    selectList.Text = studentStatus.Name;

                    studentStatusList.Add(selectList);
                }

                return studentStatusList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateLevelSelectListItem()
        {
            try
            {
                var levelLogic = new LevelLogic();
                List<Level> levels = levelLogic.GetAll();

                if (levels == null || levels.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var levelList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                levelList.Add(list);

                foreach (Level level in levels)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = level.Id.ToString();
                    selectList.Text = level.Name;

                    levelList.Add(selectList);
                }

                return levelList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateEContentTypeSelectListItem()
        {
            try
            {
                var contentTypeLogic = new EContentTypeLogic();
                //List<EContentType> contentTypeList = contentTypeLogic.GetAll();
                List<EContentType> contentTypeList = contentTypeLogic.GetModelsBy(x => x.Active);

                if (contentTypeList == null || contentTypeList.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var contentTypeSelectList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                contentTypeSelectList.Add(list);

                foreach (EContentType type in contentTypeList)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = type.Id.ToString();
                    selectList.Text = type.Name;

                    contentTypeSelectList.Add(selectList);
                }

                return contentTypeSelectList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public static List<SelectListItem> PopulateEContentTypeSelectListItemByCourseId(long courseId)
        //{
        //    try
        //    {
        //        var contentTypeLogic = new EContentTypeLogic();
        //        //List<EContentType> contentTypeList = contentTypeLogic.GetAll();
        //        List<EContentType> contentTypeList = contentTypeLogic.GetModelsBy(x => x.Active && x.Course_Id==courseId);

        //        if (contentTypeList == null || contentTypeList.Count <= 0)
        //        {
        //            return new List<SelectListItem>();
        //        }

        //        var contentTypeSelectList = new List<SelectListItem>();

        //        var list = new SelectListItem();
        //        list.Value = "";
        //        list.Text = Select;
        //        contentTypeSelectList.Add(list);

        //        foreach (EContentType type in contentTypeList)
        //        {
        //            var selectList = new SelectListItem();
        //            selectList.Value = type.Id.ToString();
        //            selectList.Text = type.Name;

        //            contentTypeSelectList.Add(selectList);
        //        }

        //        return contentTypeSelectList;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public static List<SelectListItem> PopulateModeOfFinanceSelectListItem()
        {
            try
            {
                var modeOfFinanceLogic = new ModeOfFinanceLogic();
                List<ModeOfFinance> modeOfFinances = modeOfFinanceLogic.GetAll();

                if (modeOfFinances == null || modeOfFinances.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var modeOfFinanceList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                modeOfFinanceList.Add(list);

                foreach (ModeOfFinance modeOfFinance in modeOfFinances)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = modeOfFinance.Id.ToString();
                    selectList.Text = modeOfFinance.Name;

                    modeOfFinanceList.Add(selectList);
                }

                return modeOfFinanceList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateFormSettingSelectListItem()
        {
            try
            {
                var formSettingLogic = new ApplicationFormSettingLogic();
                List<ApplicationFormSetting> settings = formSettingLogic.GetAll();

                if (settings == null || settings.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var settingList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                settingList.Add(list);

                foreach (ApplicationFormSetting setting in settings)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = setting.Id.ToString();
                    selectList.Text = setting.Name;

                    settingList.Add(selectList);
                }

                return settingList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulatePaymentModeSelectListItem()
        {
            try
            {
                var paymentModeLogic = new PaymentModeLogic();
                List<PaymentMode> paymentModes = paymentModeLogic.GetAll();

                if (paymentModes == null || paymentModes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var paymentModeList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                paymentModeList.Add(list);

                foreach (PaymentMode paymentmode in paymentModes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = paymentmode.Id.ToString();
                    selectList.Text = paymentmode.Name;

                    paymentModeList.Add(selectList);
                }

                return paymentModeList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateDepartmentSelectListItemBy(Programme programme)
        {
            try
            {
                var programmeDepartmentLogic = new ProgrammeDepartmentLogic();
                List<Department> departments = programmeDepartmentLogic.GetBy(programme).OrderBy(a => a.Name).ToList();

                if (departments == null || departments.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var departmentList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                departmentList.Add(list);

                foreach (Department department in departments)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = department.Id.ToString();
                    selectList.Text = department.Name;

                    departmentList.Add(selectList);
                }

                return new List<SelectListItem>(departmentList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateDepartmentOptionSelectListItem(Department department,
            Programme programme)
        {
            try
            {
                var departmentOptionLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOPtions = departmentOptionLogic.GetBy(department, programme);

                if (departmentOPtions == null && departmentOPtions.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var departmentOptionList = new List<SelectListItem>();
                if (departmentOPtions != null && departmentOPtions.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    departmentOptionList.Add(list);

                    foreach (DepartmentOption departmentOption in departmentOPtions)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = departmentOption.Id.ToString();
                        selectList.Text = departmentOption.Name;

                        departmentOptionList.Add(selectList);
                    }
                }

                return new List<SelectListItem>(departmentOptionList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SelectListItem> PopulateDepartmentOptionSelectListItemByDepartment(Department department)
        {
            try
            {
                var departmentOptionLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOPtions = departmentOptionLogic.GetBy(department);

                if (departmentOPtions == null && departmentOPtions.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var departmentOptionList = new List<SelectListItem>();
                if (departmentOPtions != null && departmentOPtions.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    departmentOptionList.Add(list);

                    foreach (DepartmentOption departmentOption in departmentOPtions)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = departmentOption.Id.ToString();
                        selectList.Text = departmentOption.Name;

                        departmentOptionList.Add(selectList);
                    }
                }

                return new List<SelectListItem>(departmentOptionList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateAllDepartmentOptionSelectListItem()
        {
            try
            {
                var departmentOptionLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOPtions = departmentOptionLogic.GetAll();

                if (departmentOPtions == null && departmentOPtions.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var departmentOptionList = new List<SelectListItem>();
                if (departmentOPtions != null && departmentOPtions.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    departmentOptionList.Add(list);

                    foreach (DepartmentOption departmentOption in departmentOPtions)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = departmentOption.Id.ToString();
                        selectList.Text = departmentOption.Name;

                        departmentOptionList.Add(selectList);
                    }
                }

                return new List<SelectListItem>(departmentOptionList.OrderBy(x => x.Text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateStudentCategorySelectListItem()
        {
            try
            {
                var studentCategoryLogic = new StudentCategoryLogic();
                List<StudentCategory> studentCategories = studentCategoryLogic.GetAll();

                if (studentCategories == null && studentCategories.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var studentCategoryList = new List<SelectListItem>();
                if (studentCategories != null && studentCategories.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    studentCategoryList.Add(list);

                    foreach (StudentCategory studentCategory in studentCategories)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = studentCategory.Id.ToString();
                        selectList.Text = studentCategory.Name;

                        studentCategoryList.Add(selectList);
                    }
                }

                return studentCategoryList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateStudentResultTypeSelectListItem()
        {
            try
            {
                var studentResultTypeLogic = new StudentResultTypeLogic();
                List<StudentResultType> studentResultTypes = studentResultTypeLogic.GetAll();

                if (studentResultTypes == null && studentResultTypes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var studentResultTypeList = new List<SelectListItem>();
                if (studentResultTypes != null && studentResultTypes.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    studentResultTypeList.Add(list);

                    foreach (StudentResultType studentResultType in studentResultTypes)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = studentResultType.Id.ToString();
                        selectList.Text = studentResultType.Name;

                        studentResultTypeList.Add(selectList);
                    }
                }

                return studentResultTypeList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateSessionSemesterSelectListItem()
        {
            try
            {
                var sessionSemesterLogic = new SessionSemesterLogic();
                List<SessionSemester> sessionSemesters = sessionSemesterLogic.GetAll();

                if (sessionSemesters == null && sessionSemesters.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var sessionSemesterList = new List<SelectListItem>();
                if (sessionSemesters != null && sessionSemesters.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    sessionSemesterList.Add(list);

                    foreach (SessionSemester sessionSemester in sessionSemesters)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = sessionSemester.Id.ToString();
                        selectList.Text = sessionSemester.Name;

                        sessionSemesterList.Add(selectList);
                    }
                }

                return sessionSemesterList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Department> GetDepartmentByProgramme(Programme programme)
        {
            try
            {
                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                if (departments != null && departments.Count > 0)
                {
                    departments.Insert(0, new Department { Id = 0, Name = SelectDepartment });
                }

                return new List<Department>(departments.OrderBy(x => x.Name));

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<Department> GetDepartmentByProgramme(Faculty faculty, Programme programme)
        {
            try
            {
                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(faculty,programme);

                if (departments != null && departments.Count > 0)
                {
                    departments.Insert(0, new Department { Id = 0, Name = SelectDepartment });
                }

                return new List<Department>(departments.OrderBy(x => x.Name));

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateAdmissionListTypeSelectListItem()
        {
            try
            {
                var admissionListTypeLogic = new AdmissionListTypeLogic();
                List<AdmissionListType> admissionListTypes = admissionListTypeLogic.GetAll();

                if (admissionListTypes == null || admissionListTypes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var admissionListTypeList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = SelectAdmissiontype;
                admissionListTypeList.Add(list);

                foreach (AdmissionListType admissionListType in admissionListTypes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = admissionListType.Id.ToString();
                    selectList.Text = admissionListType.Name;

                    admissionListTypeList.Add(selectList);
                }

                return admissionListTypeList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Course> GetCoursesByLevelDepartmentAndSemester(Level level, Department department,
            Semester semester, Programme programme)
        {
            try
            {
                var courseLogic = new CourseLogic();
                List<Course> courses = courseLogic.GetBy(department, level, semester, programme);
                var formattedCourses = new List<Course>();
                if (courses != null && courses.Count > 0)
                {
                    foreach (Course course in courses)
                    {
                        var newCourse = new Course { Id = course.Id, Name = course.Code + '-' + course.Name };
                        formattedCourses.Add(newCourse);
                    }
                    formattedCourses.Insert(0, new Course { Id = 0, Name = SelectCourse });
                }

                return new List<Course>(formattedCourses.OrderBy(x => x.Name)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Course> GetCourseListByLevelDepartmentAndSemester(Level level, Department department,
          Semester semester, Programme programme)
        {
            try
            {
                var courseLogic = new CourseLogic();
                return courseLogic.GetBy(department, level, semester, programme);
                
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<Course> GetOnlyRegisteredCoursesByLevelDepartmentAndSemester(Level level,
            Department department, Semester semester, Programme programme, Session session)
        {
            try
            {
                var courseLogic = new CourseLogic();
                List<Course> courses = courseLogic.GetOnlyRegisteredCourses(department, level, semester, programme,
                    session);
                return new List<Course>(courses.OrderBy(x => x.Name)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<Course> GetOnlyRegisteredCoursesByLevelDepartmentAndSemester(Level level,
    Department department, Semester semester, Programme programme, Session session, DepartmentOption departmentOption)
        {
            try
            {
                var courseLogic = new CourseLogic();
                List<Course> courses = courseLogic.GetOnlyRegisteredCourses(department, level, semester, programme,
                    session, departmentOption);
                return new List<Course>(courses.OrderBy(x => x.Name)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Student> GetStudentsBy(Level level, Programme programme, Department department,
            Session session)
        {
            try
            {
                var studentLevelLogic = new StudentLevelLogic();
                List<StudentLevel> studentLevels = studentLevelLogic.GetBy(level, programme, department, session);

                var students = new List<Student>();
                foreach (StudentLevel studentLevel in studentLevels)
                {
                    studentLevel.Student.FirstName = studentLevel.Student.MatricNumber + " - " +
                                                     studentLevel.Student.Name;
                    students.Add(studentLevel.Student);
                }

                if (students != null && students.Count > 0)
                {
                    students.Insert(0, new Student { Id = 0, FirstName = "-- Select Student --" });
                }

                return new List<Student>(students.OrderBy(x => x.MatricNumber));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string Encrypt(string encrypData)
        {
            string data = "";
            try
            {
                string CharData = "";
                string ConChar = "";
                for (int i = 0; i < encrypData.Length; i++)
                {
                    CharData = Convert.ToString(encrypData.Substring(i, 1));
                    ConChar = char.ConvertFromUtf32(char.ConvertToUtf32(CharData, 0) + 115);
                    data = data + ConChar;
                }
            }
            catch (Exception ex)
            {
                data = "1";
                return data;
            }
            return data;
        }

        public static string Decrypt(string encrypData)
        {
            string data = "";
            try
            {
                string CharData = "";
                string ConChar = "";
                for (int i = 0; i < encrypData.Length; i++)
                {
                    CharData = Convert.ToString(encrypData.Substring(i, 1));
                    ConChar = char.ConvertFromUtf32(char.ConvertToUtf32(CharData, 0) - 115);
                    data = data + ConChar;
                }
            }
            catch (Exception ex)
            {
                data = "1";
                return data;
            }
            return data;
        }

        public static List<SelectListItem> PopulateTranscriptStatusSelectListItem()
        {
            try
            {
                var transcriptStatusLogic = new TranscriptStatusLogic();
                List<TranscriptStatus> transcripts = transcriptStatusLogic.GetAll();
                if (transcripts == null || transcripts.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var transcriptsList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                transcriptsList.Add(list);

                foreach (TranscriptStatus state in transcripts)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = state.TranscriptStatusId.ToString();
                    selectList.Text = state.TranscriptStatusName;

                    transcriptsList.Add(selectList);
                }

                return new List<SelectListItem>(transcriptsList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SelectListItem> PopulateDispatchStatusSelectListItem()
        {
            try
            {
                var transcriptStatusLogic = new TranscriptStatusLogic();
                List<TranscriptStatus> transcripts = transcriptStatusLogic.GetModelsBy(s => s.Transcript_Status_Id == 5 || s.Transcript_Status_Id == 4);
                if (transcripts == null || transcripts.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var transcriptsList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                transcriptsList.Add(list);

                foreach (TranscriptStatus state in transcripts)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = state.TranscriptStatusId.ToString();
                    selectList.Text = state.TranscriptStatusName;

                    transcriptsList.Add(selectList);
                }

                return new List<SelectListItem>(transcriptsList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateTranscriptClearanceStatusSelectListItem()
        {
            try
            {
                var transcriptStatusLogic = new TranscriptClearanceStatusLogic();
                List<TranscriptClearanceStatus> transcripts = transcriptStatusLogic.GetAll();
                if (transcripts == null || transcripts.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var transcriptsList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                transcriptsList.Add(list);

                foreach (TranscriptClearanceStatus state in transcripts)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = state.TranscriptClearanceStatusId.ToString();
                    selectList.Text = state.TranscriptClearanceStatusName;

                    transcriptsList.Add(selectList);
                }

                return new List<SelectListItem>(transcriptsList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateCountrySelectListItem()
        {
            try
            {
                var countryLogic = new CountryLogic();
                List<Country> countries = countryLogic.GetAll();
                if (countries == null || countries.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var countryList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                countryList.Add(list);

                foreach (Country country in countries)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = country.Id;
                    selectList.Text = country.CountryName;

                    countryList.Add(selectList);
                }

                return new List<SelectListItem>(countryList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateCourseModeSelectListItem()
        {
            try
            {
                var courseModeLogic = new CourseModeLogic();
                List<CourseMode> courseModes = courseModeLogic.GetAll();
                if (courseModes == null || courseModes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var courseModeList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                courseModeList.Add(list);

                foreach (CourseMode courseMode in courseModes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = courseMode.Id.ToString();
                    selectList.Text = courseMode.Name;

                    courseModeList.Add(selectList);
                }

                return courseModeList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateCourseTypeSelectListItem()
        {
            try
            {
                var courseModeLogic = new CourseTypeLogic();
                List<CourseType> courseModes = courseModeLogic.GetAll();
                if (courseModes == null || courseModes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var courseModeList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                courseModeList.Add(list);

                foreach (CourseType courseMode in courseModes)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = courseMode.Id.ToString();
                    selectList.Text = courseMode.Name;

                    courseModeList.Add(selectList);
                }

                return courseModeList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateStaffSelectListItem()
        {
            try
            {
                var userLogic = new UserLogic();
                List<User> users =
                    userLogic.GetModelsBy(p => p.ROLE.Role_Name == "Lecturer").OrderBy(a => a.Username).ToList();
                if (users == null || users.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var userList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                userList.Add(list);

                foreach (User user in users)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = user.Id.ToString();
                    selectList.Text = user.Username;

                    userList.Add(selectList);
                }

                return new List<SelectListItem>(userList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateRoleSelectListItem()
        {
            try
            {
                var roleLogic = new RoleLogic();
                List<Role> roles = roleLogic.GetAll();
                if (roles == null || roles.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var roleList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                roleList.Add(list);

                foreach (Role role in roles)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = role.Id.ToString();
                    selectList.Text = role.Name;

                    roleList.Add(selectList);
                }

                return roleList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateSecurityQuestionSelectListItem()
        {
            try
            {
                var securityQuestionLogic = new SecurityQuestionLogic();
                List<SecurityQuestion> questions = securityQuestionLogic.GetAll();
                if (questions == null || questions.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var roleList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                roleList.Add(list);

                foreach (SecurityQuestion question in questions)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = question.Id.ToString();
                    selectList.Text = question.Name;

                    roleList.Add(selectList);
                }

                return roleList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<InstitutionChoice> GetAllInstitutionChoices()
        {
            try
            {
                var institutionChoiceLogic = new InstitutionChoiceLogic();
                List<InstitutionChoice> institutionChoices = institutionChoiceLogic.GetAll();

                if (institutionChoices != null && institutionChoices.Count > 0)
                {
                    institutionChoices.Insert(0,
                        new InstitutionChoice { Id = 0, Name = "-- Select Institution Choice --" });
                }

                return new List<InstitutionChoice>(institutionChoices.OrderBy(x => x.Name)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<ApplicationFormSetting> GetAllApplicationFormSetting()
        {
            try
            {
                var applicationFormSettingLogic = new ApplicationFormSettingLogic();
                List<ApplicationFormSetting> institutionChoices = applicationFormSettingLogic.GetAll();

                if (institutionChoices != null && institutionChoices.Count > 0)
                {
                    institutionChoices.Insert(0,
                        new ApplicationFormSetting { Id = 0, Name = "-- Select Form Type --" });
                }

                return institutionChoices;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateStaffRoleSelectListItem()
        {
            try
            {
                var roleLogic = new RoleLogic();
                List<Role> roles = roleLogic.GetAll().OrderBy(x => x.Id).ToList();
                roles.RemoveAt(0);
                if (roles == null || roles.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var roleList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = "-- Select Role --";
                roleList.Add(list);

                foreach (Role role in roles)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = role.Id.ToString();
                    selectList.Text = role.Name;

                    roleList.Add(selectList);
                }

                return roleList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateMenuGroupSelectListItem()
        {
            try
            {
                var menuGroupLogic = new MenuGroupLogic();
                List<MenuGroup> menuGroups = menuGroupLogic.GetAll().OrderBy(a => a.Name).ToList();
                if (menuGroups == null || menuGroups.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var MenuGroupList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                MenuGroupList.Add(list);

                foreach (MenuGroup role in menuGroups)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = role.Id.ToString();
                    selectList.Text = role.Name;

                    MenuGroupList.Add(selectList);
                }

                return MenuGroupList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateMenuSelectListItem()
        {
            try
            {
                var menuLogic = new MenuLogic();
                List<Model.Model.Menu> menuList = menuLogic.GetAll().OrderBy(a => a.DisplayName).ToList();
                if (menuList == null || menuList.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var MenuList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                MenuList.Add(list);

                foreach (Model.Model.Menu menu in menuList)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = menu.Id.ToString();
                    selectList.Text = menu.DisplayName + ", In " + menu.MenuGroup.Name;

                    MenuList.Add(selectList);
                }

                return new List<SelectListItem>(MenuList.OrderBy(x => x.Text)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateSemesterSelectListItem()
        {
            try
            {
                var semesterLogic = new SemesterLogic();
                List<Semester> semesters = semesterLogic.GetAll();

                if (semesters == null || semesters.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var semesterList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                semesterList.Add(list);

                foreach (Semester semester in semesters)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = semester.Id.ToString();
                    selectList.Text = semester.Name;

                    semesterList.Add(selectList);
                }

                return semesterList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Student GetStudent(string MatricNumber)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                return studentLogic.GetBy(MatricNumber);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static List<SelectListItem> PopulateHostelRoomCorners(HostelRoom hostelRoom)
        {
            try
            {
                HostelRoomCornerLogic hostelRoomCornerLogic = new HostelRoomCornerLogic();
                List<HostelRoomCorner> hostelRoomCorners = hostelRoomCornerLogic.GetModelsBy(a => a.Room_Id == hostelRoom.Id && a.Activated == true).OrderByDescending(k => k.Name).ToList();

                if (hostelRoomCorners == null || hostelRoomCorners.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                List<SelectListItem> selectItemList = new List<SelectListItem>();

                SelectListItem list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                selectItemList.Add(list);

                foreach (HostelRoomCorner roomCorner in hostelRoomCorners)
                {
                    SelectListItem selectList = new SelectListItem();
                    selectList.Value = roomCorner.Id.ToString();
                    selectList.Text = roomCorner.Name;
                    selectItemList.Add(selectList);
                }

                return selectItemList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateAllCourseCodeSelectListItem()
        {
            try
            {
                var courseLogic = new CourseLogic();
                List<Course> courses = courseLogic.GetAll().Where(c => c.Code.Contains("GST")).Distinct().ToList();


                if (courses == null || courses.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var stateList = new List<SelectListItem>();
                if (courses != null && courses.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    stateList.Add(list);

                    foreach (Course course in courses)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = course.Id.ToString();
                        selectList.Text = course.Code;
                        stateList.Add(selectList);
                    }
                }

                return stateList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SelectListItem> PopulateDeliveryServiceSelectListItem()
        {
            try
            {
                var deliveryServiceLogic = new DeliveryServiceLogic();
                List<DeliveryService> deliveryServices = deliveryServiceLogic.GetAll();


                if (deliveryServices == null || deliveryServices.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var deliveryServiceList = new List<SelectListItem>();
                if (deliveryServices != null && deliveryServices.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    deliveryServiceList.Add(list);

                    foreach (DeliveryService deliveryService in deliveryServices)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = deliveryService.Id.ToString();
                        selectList.Text = deliveryService.Name;
                        deliveryServiceList.Add(selectList);
                    }
                }

                return deliveryServiceList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SelectListItem> PopulateGeoZoneSelectListItem()
        {
            try
            {
                var geoZoneLogic = new GeoZoneLogic();
                List<GeoZone> geoZones = geoZoneLogic.GetAll();


                if (geoZones == null || geoZones.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var geoZoneList = new List<SelectListItem>();
                if (geoZones != null && geoZones.Count > 0)
                {
                    var list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    geoZoneList.Add(list);

                    foreach (GeoZone geoZone in geoZones)
                    {
                        var selectList = new SelectListItem();
                        selectList.Value = geoZone.Id.ToString();
                        selectList.Text = geoZone.Name;
                        geoZoneList.Add(selectList);
                    }
                }

                return geoZoneList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateCampusSelectListItem()
        {
            try
            {
                var campusLogic = new CampusLogic();
                List<Campus> campuses = campusLogic.GetAll();


                if (campuses == null || campuses.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var campusList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                campusList.Add(list);

                foreach (Campus campus in campuses)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = campus.Id.ToString();
                    selectList.Text = campus.Name;
                    campusList.Add(selectList);
                }

                return campusList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateStaffTypeSelectListItem()
        {
            try
            {
                StaffTypeLogic staffTypeLogic = new StaffTypeLogic();
                List<StaffType> staffTypes = staffTypeLogic.GetAll();

                if (staffTypes == null || staffTypes.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                List<SelectListItem> selectLists = new List<SelectListItem>();
                if (staffTypes.Count > 0)
                {
                    SelectListItem list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    selectLists.Add(list);

                    foreach (StaffType staffType in staffTypes)
                    {
                        SelectListItem selectList = new SelectListItem();
                        selectList.Value = staffType.Id.ToString();
                        selectList.Text = staffType.Name;
                        selectLists.Add(selectList);
                    }
                }

                return selectLists;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SelectListItem> PopulateDesignationSelectListItem()
        {
            try
            {
                DesignationLogic designationLogic = new DesignationLogic();
                List<Designation> designations = designationLogic.GetAll();

                if (designations == null || designations.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                List<SelectListItem> selectLists = new List<SelectListItem>();
                if (designations.Count > 0)
                {
                    SelectListItem list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    selectLists.Add(list);

                    foreach (Designation designation in designations)
                    {
                        SelectListItem selectList = new SelectListItem();
                        selectList.Value = designation.Id.ToString();
                        selectList.Text = designation.Name;
                        selectLists.Add(selectList);
                    }
                }

                return selectLists;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SelectListItem> PopulateGradeLevelSelectListItem()
        {
            try
            {
                GradeLevelLogic gradeLevelLogic = new GradeLevelLogic();
                List<GradeLevel> gradeLevels = gradeLevelLogic.GetAll();

                if (gradeLevels == null || gradeLevels.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                List<SelectListItem> selectLists = new List<SelectListItem>();
                if (gradeLevels.Count > 0)
                {
                    SelectListItem list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    selectLists.Add(list);

                    foreach (GradeLevel gradeLevel in gradeLevels)
                    {
                        SelectListItem selectList = new SelectListItem();
                        selectList.Value = gradeLevel.Id.ToString();
                        selectList.Text = gradeLevel.Name;
                        selectLists.Add(selectList);
                    }
                }

                return selectLists;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SelectListItem> PopulateStaffResultGradeSelectListItem()
        {
            try
            {
                StaffResultGradeLogic resultGradeLogic = new StaffResultGradeLogic();
                List<StaffResultGrade> resultGrades = resultGradeLogic.GetAll();

                if (resultGrades == null || resultGrades.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                List<SelectListItem> selectLists = new List<SelectListItem>();
                if (resultGrades.Count > 0)
                {
                    SelectListItem list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    selectLists.Add(list);

                    foreach (StaffResultGrade resultGrade in resultGrades)
                    {
                        SelectListItem selectList = new SelectListItem();
                        selectList.Value = resultGrade.Id.ToString();
                        selectList.Text = resultGrade.Grade;
                        selectLists.Add(selectList);
                    }
                }

                return selectLists;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SelectListItem> PopulateUnitSelectListItem()
        {
            try
            {
                UnitLogic unitLogic = new UnitLogic();
                List<Model.Model.Unit> units = unitLogic.GetAll();

                if (units == null || units.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                List<SelectListItem> selectLists = new List<SelectListItem>();
                if (units.Count > 0)
                {
                    SelectListItem list = new SelectListItem();
                    list.Value = "";
                    list.Text = Select;
                    selectLists.Add(list);

                    foreach (Model.Model.Unit unit in units)
                    {
                        SelectListItem selectList = new SelectListItem();
                        selectList.Value = unit.Id.ToString();
                        selectList.Text = unit.Name;
                        selectLists.Add(selectList);
                    }
                }

                return selectLists;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Level SetLevel(Programme programme)
        {
            try
            {
                Level level;
                switch (programme.Id)
                {
                    case 1:
                        {
                            return level = new Level { Id = 1 };
                        }
                    case 2:
                        {
                            return level = new Level { Id = 1 };
                        }
                    case 3:
                        {
                            return level = new Level { Id = 1 };
                        }
                    case 4:
                        {
                            return level = new Level { Id = 1 };
                        }
                    default:
                        return level = new Level { Id = 1 };
                }
                return level = new Level();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SelectListItem> PopulateEContentTypeSelectListItemByCourseAllocation(long courseAllocationId)
        {
            try
            {
                var contentTypeLogic = new EContentTypeLogic();
                List<EContentType> contentTypeList = contentTypeLogic.GetModelsBy(f => f.Course_Allocation_Id == courseAllocationId && f.IsDelete == false);

                if (contentTypeList == null || contentTypeList.Count <= 0)
                {
                    return new List<SelectListItem>();
                }

                var contentTypeSelectList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                contentTypeSelectList.Add(list);

                foreach (EContentType type in contentTypeList)
                {
                    var selectList = new SelectListItem();
                    selectList.Value = type.Id.ToString();
                    selectList.Text = type.Name;

                    contentTypeSelectList.Add(selectList);
                }

                return contentTypeSelectList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SelectListItem> GradeGuideSelectListItem(int Max)
        {
            try
            {
                var contentTypeLogic = new EContentTypeLogic();
                //int Max = 100;

                var gradeSelectList = new List<SelectListItem>();

                var list = new SelectListItem();
                list.Value = "";
                list.Text = Select;
                gradeSelectList.Add(list);



                for (int i = 1; i <= Max; i++)
                {
                    SelectListItem selectList = new SelectListItem();
                    selectList.Text = i.ToString();
                    selectList.Value = i.ToString();
                    gradeSelectList.Add(selectList);
                }

                return gradeSelectList;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}