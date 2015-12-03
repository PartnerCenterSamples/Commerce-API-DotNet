using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Partner.CSP.Api.V1.Samples.DataModels
{
    public class Resource
    {
        public string MeterId { get; set; }
        public string MeterName { get; set; }
        public string MeterCategory { get; set; }
        public string MeterSubCategory { get; set; }
        public string Unit { get; set; }
        public Dictionary<double, double> MeterRates { get; set; }
        public string EffectiveDate { get; set; }
        public double IncludedQuantity { get; set; }
    }
}
