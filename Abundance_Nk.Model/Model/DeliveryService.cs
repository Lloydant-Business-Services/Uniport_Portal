﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class DeliveryService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Activated { get; set; }
        public string DeliveryServiceAccount { get; set; }
        public string BankCode { get; set; }

    }
}
