﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Global_Intern.Models
{
    public class VisaStatus
    {
        public int VisaStatusId { get; set; }
        public string VisaType { get; set; }
        public int VisaNumber { get; set; }
        public User User { get; set; }
    }
}
