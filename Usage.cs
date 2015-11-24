/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Newtonsoft.Json;

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
    class Usage
    {

        /// <summary>
        /// This method gets a collection resource containing all the entitlements that belong to a given customer
        /// </summary>
        /// <param name="customerCid">cid of the customer</param>
        /// <param name="customerAuthorizationToken">customer authorization token</param>
        /// <returns>object that contains all the entitlements</returns>
        public static dynamic GetEntitlements(string customerCid, string customerAuthorizationToken)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/Entitlements ", customerCid));

            request.Method = "GET";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + customerAuthorizationToken);

            try
            {
                Utilities.PrintWebRequest(request, string.Empty);

                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
                    var entitlements = Json.Decode(responseContent);
                    return entitlements;
                }
            }
            catch (WebException webException)
            {
                using (var reader = new StreamReader(webException.Response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
                }
            }
            return string.Empty;
        }
        
        /// <summary>
        /// This method gets a customer's Azure usage information between the given dates.
        /// </summary>
        /// <param name="resellerCid">cid of the reseller</param>
        /// <param name="entitlementId">Specifies the entitlement for which the caller is requesting the usage</param>
        /// <param name="sa_Token">sales agent token</param>
        /// <param name="startTime">Specified the start time range of when the usage data was metered within the billing system</param>
        /// <param name="endTime">Specifies the end time range of when the usage data was metered within the billing system.</param>
        /// <returns>object containing usage records of the customer</returns>
        public static UsageResponse GetUsageRecords(string resellerCid, string entitlementId, string sa_Token, string startTime, string endTime)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/usage-records?entitlement_id={1}&reported_start_time={2}&reported_end_time={3}", resellerCid, entitlementId, startTime, endTime));

            request.Method = "GET";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_Token);
            UsageResponse usageRecordsPayload = new UsageResponse();
            try
            {
                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    var usageRecords = Json.Decode(responseContent);
                    usageRecordsPayload = JsonConvert.DeserializeObject<UsageResponse>(responseContent);
                    //// Prints the data to console only if the usageRecord is NOT empty
                    //if (usageRecords.items.Length > 0)
                    //{
                    //    Utilities.PrintWebRequest(request, string.Empty);
                    //    Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
                    //}
                    return usageRecordsPayload;
                }
            }
            catch (WebException webException)
            {
                using (var reader = new StreamReader(webException.Response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
                }
            }
            return usageRecordsPayload;
        }
        public static double computeRatedUsagePerMeter(Dictionary<double, double> rates, double usage)
        {
            double total = 0.0;

            if (rates.Count == 0)
                return 0.0;

            else if (rates.Count == 1)
                return (usage * rates.Values.FirstOrDefault());

            var remainingUsage = usage;

            while (rates.Count > 0)
            {
                //double currentValue=rates.GetEnumerator().Current.Key;
                double LastKey = rates.Keys.Last();

                if (remainingUsage > LastKey)
                {
                    double LastKeyValue = 0.0;
                    if (rates.TryGetValue(LastKey, out LastKeyValue))
                    {
                        total = total + ((remainingUsage - LastKey + 1) * LastKeyValue); // remainingUsage - LastKey +1  because tiered pricing is exclusive
                        remainingUsage = LastKey - 1;
                    }
                    rates.Remove(LastKey);
                }

                else if (remainingUsage <= LastKey)
                {
                    rates.Remove(LastKey);
                }
            }
            return total;
        }
        public static Dictionary<double, double> GetRatesForMeterID(RateCardResponse rates, string meterId)
        {
            List<Resource> resources = rates.Meters;
            Resource r = resources.Find(x => x.MeterId == meterId);
            return r != null ? r.MeterRates : new Dictionary<double, double>();
        }

        public static double GetRatableUsage(double usage, double includedQuantity)
        {
            if ((usage - includedQuantity) > 0)
            {
                return (usage - includedQuantity);
            }
            else
            {
                return 0.0;
            }
        }
        public static double GetIncludedQuantityByMeterID(RateCardResponse rates, string meterId)
        {
            List<Resource> resources = rates.Meters;

            IEnumerable<double> includedQty = from r in resources
                                              where r.MeterId == meterId
                                              select r.IncludedQuantity;

            return includedQty.FirstOrDefault();
        }
    }

}
