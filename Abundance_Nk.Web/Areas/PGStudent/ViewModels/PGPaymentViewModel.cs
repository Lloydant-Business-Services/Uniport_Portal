using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.PGStudent.ViewModels
{
    public class PGPaymentViewModel
    {
        public PGPaymentViewModel()
        {
            //Programme = new Programme();
            StudentLevel = new StudentLevel();
            StudentLevel.Programme = new Programme();
            StudentLevel.Department = new Department();
            StudentLevel.DepartmentOption = new DepartmentOption();
            StudentLevel.Student = new Model.Model.Student();
            StudentLevel.Level = new Level();

            Session = new Session();

            Student = new Model.Model.Student();
            Student.Type = new StudentType { Id = 2 };
            Student.Status = new StudentStatus { Id = 1 };

            Person = new Person();
            Person.State = new State();
            Person.Type = new PersonType { Id = 3 };

            PaymentMode = new PaymentMode();
            PaymentType = new PaymentType();

            FeeType = new FeeType();

            StateSelectListItem = Utility.PopulateStateSelectListItem();
            ProgrammeSelectListItem = Utility.PopulatePostGraduateProgrammeSelectListItem();
            FeeTypeSelectListItem = Utility.PopulateFeeTypeActiveSelectListItem();
            PaymentModeSelectListItem = Utility.PopulatePaymentModeSelectListItem();
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            AllSessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
        }

        public Person Person { get; set; }
        public Model.Model.Student Student { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public bool StudentAlreadyExist { get; set; }

        public Payment Payment { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public PaymentType PaymentType { get; set; }
        public FeeType FeeType { get; set; }
        public Session Session { get; set; }
        public StudentExtraYearSession extraYear { get; set; }
        public bool IsDeferred { get; set; }
        public List<SelectListItem> StateSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOptionSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<SelectListItem> PaymentModeSelectListItem { get; set; }
        public List<SelectListItem> AllSessionSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<SelectListItem> FeeTypeSelectListItem { get; set; }
        public List<SelectListItem> SessionRegisteredSelectListItem { get; set; }
        public decimal Amount { get; set; }
        public Paystack Paystack { get; set; }
    }
}