﻿using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Web.Areas.Student.ViewModels
{
    public class RegistrationIndexViewModel
    {
        public RegistrationIndexViewModel()
        {
            PaymentHistory = new PaymentHistory();
            Student = new Model.Model.Student();
            StudentLevel = new StudentLevel();
            Session = new Session();
            Payment = new Payment();
        }

        public PaymentHistory PaymentHistory { get; set; }
        public Model.Model.Student Student { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public Session Session { get; set; }
        public Payment Payment { get; set; }
        public bool isExtraYearStudent { get; set; }
    }
}