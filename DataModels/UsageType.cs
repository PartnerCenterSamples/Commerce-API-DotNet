/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Partner.CSP.Api.V1.Samples.DataModels
{
    public class UsageType
    {
        public string entitlement_id { get; set; }
        public string usage_start_time { get; set; }
        public string usage_end_time { get; set; }
        public string object_type { get; set; }
        public string meter_name { get; set; }
        public string meter_category { get; set; }
        public string unit { get; set; }
        public string instance_data { get; set; }
        public string meter_id { get; set; }
        public InfoField info_fields { get; set; }
        public double quantity { get; set; }
        public string meter_region { get; set; }
        public string meter_sub_category { get; set; }
    }
}
