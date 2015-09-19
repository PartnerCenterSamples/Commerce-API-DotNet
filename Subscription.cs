/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Web.Helpers;

    public static class Subscription
    {
        /// <summary>
        /// This method is to retrieve the subscriptions of a customer bought from the reseller
        /// </summary>
        /// <param name="customerCid">cid of the customer</param>
        /// <param name="resellerCid">cir of the reseller</param>
        /// <param name="sa_Token">sales agent token</param>
        /// <returns>object that contains all of the subscriptions</returns>
        public static dynamic GetSubscriptions(string customerCid, string resellerCid, string sa_Token)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/subscriptions?recipient_customer_id={1}", resellerCid, customerCid));

            request.Method = "GET";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_Token);

            try
            {
                Utilities.PrintWebRequest(request, string.Empty);

                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
                    var subscriptionsResponse = Json.Decode(responseContent);

                    foreach (var subscription in subscriptionsResponse.items)
                    {
                        PrintSubscription(subscription);
                    }

                    return subscriptionsResponse;
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
        /// This method returns the subscription given the subscription id
        /// </summary>
        /// <param name="subscriptionId">subscription id</param>
        /// <param name="resellerCid">cid of the reseller</param>
        /// <param name="sa_Token">sales agent token</param>
        /// <returns>subscription object</returns>
        public static dynamic GetSubscriptionById(string subscriptionId, string resellerCid, string sa_Token)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/subscriptions/{1}", resellerCid, subscriptionId));

            request.Method = "GET";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_Token);

            try
            {
                Utilities.PrintWebRequest(request, string.Empty);

                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
                    var subscription = Json.Decode(responseContent);
                    PrintSubscription(subscription);

                    return subscription;
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
        /// This method returns the subscription given the subscription uri
        /// </summary>
        /// <param name="subscriptionUri">subscription uri</param>
        /// <param name="sa_Token">sales agent token</param>
        /// <returns>subscription object</returns>
        public static dynamic GetSubscriptionByUri(string subscriptionUri, string sa_Token)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}", subscriptionUri));

            request.Method = "GET";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_Token);

            try
            {
                Utilities.PrintWebRequest(request, string.Empty);

                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
                    var subscription = Json.Decode(responseContent);
                    PrintSubscription(subscription);

                    return subscription;
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
        /// Transitions the existing subscription and returns the new subscription object
        /// </summary>
        /// <param name="subscriptionUri">existing subscription uri</param>
        /// <param name="newOfferUri">transitioning to the new offer</param>
        /// <param name="resellerCid">reseller Cid</param>
        /// <param name="sa_Token">sa token of the reseller</param>
        /// <returns>subscription to the new offer</returns>
        public static dynamic Transition(string subscriptionId, string customerCid, string newOfferUri, string resellerCid, string sa_Token)
        {
            var subscription = GetSubscriptionById(subscriptionId, resellerCid, sa_Token);

            var order = new
            {
                line_items = new List<dynamic>
                {
                    new
                    {
                        line_item_number = 0,
                        reference_entitlement_uris = new List<string>
                        {
                            subscription.links.entitlement.href
                        },
                        offer_uri = newOfferUri,
                        quantity = subscription.quantity
                    }
                },
                recipient_customer_id = customerCid
            };

            var transitionOrder = Order.PlaceOrder(order, resellerCid, sa_Token);

            return GetSubscriptionByUri(transitionOrder.line_items[0].resulting_subscription_uri, sa_Token);
        }

        /// <summary>
        /// Suspends the subscription 
        /// Ref https://msdn.microsoft.com/en-us/library/partnercenter/mt146400.aspx
        /// </summary>
        /// <param name="subscriptionId">existing subscription Id</param>
        /// <param name="resellerCid">reseller Cid</param>
        /// <param name="sa_Token">sa token of the reseller</param>
        /// <returns>the subscription</returns>
        public static dynamic Suspend(string subscriptionId, string resellerCid, string sa_Token)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/subscriptions/{1}/add-suspension", resellerCid, subscriptionId));

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_Token);

            // The only valid reason for suspension is CustomerCancellation. This is also true for removing the suspension
            string content = "{ \"suspension_reason\" : \"CustomerCancellation\", \"suspension_reason_comment\" : \"Customer wanted to cancel\" }";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(content);
            }

            try
            {
                Utilities.PrintWebRequest(request, content);

                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);

                    return responseContent;
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
        /// This method is to create a stream for a reseller to hear all subscription events
        /// </summary>
        /// <param name="resellerCid">cid of the reseller</param>
        /// <param name="streamName">name for the string to be created</param>
        /// <param name="sa_Token">sales agent token</param>
        /// <returns>stream information</returns>
        public static dynamic CreateSubscriptionStream(string resellerCid, string streamName, string sa_Token)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/subscription-streams/{0}/{1}", resellerCid, streamName));

            request.Method = "PUT";
            request.Accept = "application/json";
            request.ContentType = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_Token);

            var streamInfo = new
            {
                start_time = string.Format("{0:MM/dd/yyyy HH:mm:ss}",DateTime.UtcNow),
                page_size = 100
            };

            string content = Json.Encode(streamInfo);

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(content);
            }

            try
            {
                Utilities.PrintWebRequest(request, content);

                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
                    return Json.Decode(responseContent);
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
        /// This method returns the subscription stream for a reseller
        /// </summary>
        /// <param name="resellerCid">cid of the reseller</param>
        /// <param name="streamName">name of the stream to be retrieved</param>
        /// <param name="sa_Token">sales agent token</param>
        /// <returns>subscription events for the reseller along with link to mark the page as completed</returns>
        public static dynamic GetSubscriptionStream(string resellerCid, string streamName, string sa_Token)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/subscription-streams/{0}/{1}/pages", resellerCid, streamName));

            request.Method = "GET";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_Token);

            try
            {
                Utilities.PrintWebRequest(request, string.Empty);

                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
                    return Json.Decode(responseContent);
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
        /// This method is to mark the page as read inorder to move to the next page in the stream
        /// </summary>
        /// <param name="completedUri">uri to mark the stream as completed</param>
        /// <param name="sa_Token">sales agent token</param>
        /// <returns>information that indicated success</returns>
        public static dynamic MarkPageAsCompletedInStream(string completedUri, string sa_Token)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}", completedUri));

            request.Method = "GET";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_Token);

            try
            {
                Utilities.PrintWebRequest(request, string.Empty);

                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
                    return Json.Decode(responseContent);
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
        /// This method is to print the subscription information
        /// </summary>
        /// <param name="subscription">subscription object</param>
        private static void PrintSubscription(dynamic subscription)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("=========================================");
            Console.WriteLine("Subscription Information");
            Console.WriteLine("=========================================");
            Console.WriteLine("Id\t\t: {0}", subscription.id);
            Console.WriteLine("Order Id\t: {0}", subscription.order_id);
            Console.WriteLine("Etag\t\t: {0}", subscription.etag);
            Console.WriteLine("Offer Uri\t:{0}", subscription.offer_uri);
            Console.WriteLine("Friendly Name\t: {0}", subscription.friendly_name);

            Console.WriteLine("\nImportant Dates");
            Console.WriteLine("\tCreated Date\t\t: {0}", subscription.creation_date);
            Console.WriteLine("\tEffective Start Date\t: {0}", subscription.effective_start_date);
            Console.WriteLine("\tCommitment End Date\t: {0}", subscription.commitment_end_date);

            Console.WriteLine("\nLife Cycle");
            Console.WriteLine("\tState\t\t\t: {0}", subscription.state);
            foreach (var suspensionReason in subscription.suspension_reasons)
            {
                Console.WriteLine("\tSuspended Reason\t: {0}", suspensionReason);
            }

            Console.WriteLine("=========================================");
            Console.ResetColor();

        }
    }
}
