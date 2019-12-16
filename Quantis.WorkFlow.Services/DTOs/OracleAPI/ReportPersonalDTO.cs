using System;
using System.Collections.Generic;
using System.Text;

namespace Quantis.WorkFlow.Services.DTOs.OracleAPI
{
    public class ReportPersonalDTO
    {
        public string XValue { get; set; }
        public double Target { get; set; }
        public double Actual { get; set; }
        public string Result { get; set; }
        public string Unit { get; set; }
    }
}
