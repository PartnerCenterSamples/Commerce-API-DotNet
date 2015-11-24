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

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
    public class UsageResponse
    {
            
            public List<UsageType> items { get; set; }
            public string object_type { get; set; }
            public string contract_version { get; set; }
            public Links links { get; set; }
        
    }
}
