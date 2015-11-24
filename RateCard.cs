/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Helpers;
using System.Web.Hosting;
using Newtonsoft.Json;


namespace Microsoft.Partner.CSP.Api.V1.Samples
{
    class RateCard
    {
        /// <summary>
        /// This method gets a Rate Card with prices for Azure offers.
        /// </summary>
        /// <param name="resellerCid">reseller cid</param>
        /// <param name="sa_token">sales agent token</param>
        /// <returns>returns a Dictionary object containing mater objects mapped against meter ids</returns>
        public static RateCardResponse GetRateCard(string resellerCid, string sa_token)
        {

            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/rate-card?&OfferDurableId=MS-AZR-0145P", resellerCid));

            request.Method = "GET";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_token);
            RateCardResponse rateCardResponse = new RateCardResponse();
            try
            {
                Utilities.PrintWebRequest(request, string.Empty);
                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();


                    //// Writes the RateCard to a File
                    //string filePath = ConfigurationManager.AppSettings["FilePathForRateCard"];
                    //if (filePath != null)
                    //{
                    //    WriteRateCardToFile(filePath, resellerCid, responseContent);
                    //}

                   // var rateCard = Json.Decode(responseContent);
                    rateCardResponse = JsonConvert.DeserializeObject<RateCardResponse>(responseContent);
                    //Dictionary<string, object> rateCardMap = new Dictionary<string, object>();
                    ////foreach (var meter in rateCard.Meters)
                    //{
                    //    rateCardMap.Add(meter.MeterId, meter);
                    //}

                    return rateCardResponse;
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
            return rateCardResponse;
        }


        private static void WriteRateCardToFile(string path, string resellerCid, string content)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (StreamWriter w = File.AppendText(path))
                {
                    w.WriteLine("\nRATE CARD FOR {0}", resellerCid);
                    w.WriteLine("\n{0}", content);
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                return;
            }
        }
    }
}
