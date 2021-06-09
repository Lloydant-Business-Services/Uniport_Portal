﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Model
{
    public class JsonResultModel
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public List<PaymentEtranzactType> PaymentEtranzactTypes { get; set; }
        public List<FeeDetail> FeeDetails { get; set; }
        public List<ApplicationProgrammeFee> ProgrammeFees { get; set; }
        public List<ApplicationFormSetting> FormSettings { get; set; }
        public List<Hostel> Hostels { get; set; }
        public List<HostelType> HostelTypes { get; set; }
        public List<HostelSeries> HostelSeries { get; set; }
        public List<PaymentTerminal> PaymentTerminals { get; set; }
        public List<Fee> Fees { get; set; }
        public List<FeeType> FeeTypes { get; set; }
        public string Username { get; set; }
        public string Operation { get; set; }
        public string InitialValues { get; set; }
        public string CurrentValues { get; set; }
        public string Date { get; set; }
        public string Client { get; set; }
        public string Id { get; set; }
        public EContentType EContentType { get; set; } 
        public string CourseId { get; set; }
        public FeeSetup FeeSetup { get; set; }
        public List<EChatBoard> EChatBoards { get; set; }
        public List<EContentType> EContentTypes { get; set; }

    }
}
