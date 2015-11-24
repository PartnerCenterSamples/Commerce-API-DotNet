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
    using System.Linq;
    using System.Net;
    using System.Web.Helpers;

    public static class Order
    {
        /// <summary>
        /// This method returns an order object populated from the console for a given customer
        /// </summary>
        /// <param name="customerCid">cid of the customer</param>
        /// <returns>order object</returns>
        public static dynamic PopulateOrderFromConsole(string customerCid)
        {
            var offerTypes = Enum.GetValues(typeof(OfferType));

            bool InvalidInput = false;
            Console.Clear();
            do
            {
                Console.WriteLine("Select Offer Group");
                Console.WriteLine("1.Azure");
                Console.WriteLine("2.IntuneAndOffice");

                Console.Write("Enter index [1...{0}]:", offerTypes.Length);
                string input = Console.ReadLine().Trim();
                switch (input)
                {
                    case "1":
                        return PopulateOrderFromConsoleForOfferType(OfferType.Azure, customerCid);
                    case "2":
                        return PopulateOrderFromConsoleForOfferType(OfferType.IntuneAndOffice, customerCid);
                    default:
                        Console.WriteLine("Invalid Input, Try Again");
                        InvalidInput = true;
                        break;
                }
            }
            while (InvalidInput);

            return null;
        }

        /// <summary>
        /// This method populates Order info by offer type from the console
        /// </summary>
        /// <param name="offerType">Offer type: Azure or IntuneAndOffice</param>
        /// <param name="customerCid">cid of the customer</param>
        /// <returns>order object</returns>
        private static dynamic PopulateOrderFromConsoleForOfferType(OfferType offerType, string customerCid)
        {
            GroupedOffers selectedGroupedOffers = OfferCatalog.Instance.GroupedOffersCollection.First(groupedOffers => groupedOffers.OfferType == offerType);
            dynamic order = new
            {
                line_items = new List<dynamic>(),
                recipient_customer_id = customerCid
            };

            int nrOfLineItems = 0;

            bool done = false;
            Console.Clear();
            do
            {
                Console.WriteLine("OfferType: {0}", offerType);
                foreach (var item in selectedGroupedOffers.Offers.Select((offer, index) => new { Offer = offer, Index = index }))
                {
                    Console.WriteLine("{0}. {1}", item.Index + 1, item.Offer.Name);
                }

                Console.Write("\nSelect Offer (by index): ");
                string input = Console.ReadLine().Trim();
                int selectedIndex = -1;
                if (!int.TryParse(input, out selectedIndex))
                {
                    done = false;
                }

                var selectedOffer = selectedGroupedOffers.Offers.ElementAt(selectedIndex - 1);

                bool validQuantity = false;

                do
                {
                    Console.Write("\nQuantity {0} to {1}: ", selectedOffer.MinimumQuantity, selectedOffer.MaximumQuantity);
                    input = Console.ReadLine().Trim();
                    int quantity = 1;
                    if (!int.TryParse(input, out quantity))
                    {
                        done = false;
                    }

                    if (quantity >= selectedOffer.MinimumQuantity && quantity <= selectedOffer.MaximumQuantity)
                    {
                        validQuantity = true;
                    }

                    Console.Write("\nFriendly Name (or hit Enter for none): ");
                    input = Console.ReadLine().Trim();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        order.line_items.Add(new
                        {
                            //// has to be a unique number for each line item
                            //// recommendation is to start with 0
                            line_item_number = nrOfLineItems,

                            //// this is the offer uri for the offer that is being purchased, refer to the excel sheet for this
                            offer_uri = selectedOffer.Uri,

                            //// This is the quantity for this offer
                            quantity = quantity,

                            //// This is friendly name
                            friendlyname = input
                        });
                    }
                    else
                    {
                        order.line_items.Add(new
                        {
                            //// has to be a unique number for each line item
                            //// recommendation is to start with 0
                            line_item_number = nrOfLineItems,

                            //// this is the offer uri for the offer that is being purchased, refer to the excel sheet for this
                            offer_uri = selectedOffer.Uri,

                            //// This is the quantity for this offer
                            quantity = quantity,
                        });
                    }
                }
                while (!validQuantity);

                Console.Write("\nDo you want to add another line item (y/n)? ");
                input = Console.ReadLine().Trim();

                switch (input)
                {
                    case "y":
                    case "Y":
                        nrOfLineItems++;
                        done = false;
                        break;
                    default:
                        done = true;
                        break;
                }
            }
            while (!done);

            return order;
        }

        /// <summary>
        /// Populates an order with multiple line items
        /// </summary>
        /// <param name="customerCid">cid of the customer</param>
        /// <returns>multiple line items order</returns>
        public static dynamic PopulateOrderWithMultipleLineItems(string customerCid)
        {
            //// This is the offer Uri of Lync Online (Plan 1) from the excel sheet shared to partners
            const string lyncOnlinePlan1OfferUri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/ACA0C06C-890D-4ABB-83CF-BC519A2565E5";

            //// This is the offer Uri of Exchange  Online (Plan 1) from the excel sheet shared to partners
            const string exchangeOnlinePlan1OfferUri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/195416C1-3447-423A-B37B-EE59A99A19C4";

            return new
            {
                line_items = new List<dynamic>()
                {
                    new
                    {
                        //// has to be a unique number for each line item
                        //// recommendation is to start with 0
                        line_item_number = 0,

                        //// this is the offer uri for the offer that is being purchased, refer to the excel sheet for this
                        offer_uri = lyncOnlinePlan1OfferUri,

                        //// This is the quantity for this offer
                        quantity = 1,
                    },
                    new
                    {
                        //// has to be a unique number for each line item
                        //// recommendation is to start with 0
                        line_item_number = 1,
                        
                        //// this is the offer uri for the offer that is being purchased, refer to the excel sheet for this
                        offer_uri = exchangeOnlinePlan1OfferUri,

                        //// This is the quantity for this offer
                        quantity = 1,
                    }
                },
                //// customer cid for who the order is being placed
                recipient_customer_id = customerCid
            };
        }

        /// <summary>
        /// Sample for how to populate an order with Lync Online Plan 1
        /// </summary>
        /// <param name="customerCid">cid of the customer</param>
        /// <returns>order object</returns>
        public static dynamic PopulateLyncOnlinePlan1(string customerCid)
        {
            //// This is the offer Uri of Lync  Online (Plan 1) from the excel sheet shared to partners
            const string lyncOnlinePlan1OfferUri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/ACA0C06C-890D-4ABB-83CF-BC519A2565E5";

            return new
            {
                line_items = new List<dynamic>()
                {
                    new
                    {
                        //// has to be a unique number for each line item
                        //// recommendation is to start with 0
                        line_item_number = 0,
                        
                        //// this is the offer uri for the offer that is being purchased, refer to the excel sheet for this
                        offer_uri = lyncOnlinePlan1OfferUri,

                        //// This is the quantity for this offer
                        quantity = 1,
                    }
                },
                //// customer cid for who the order is being placed
                recipient_customer_id = customerCid
            };
        }

        /// <summary>
        /// Sample for populating an azure order
        /// </summary>
        /// <param name="customerCid">cid of the customer</param>
        /// <returns>order object</returns>
        public static dynamic PopulateAzureOrder(string customerCid)
        {
            //// This is the offer Uri of Azure Cloud Solution Provider from the excel sheet shared to partners
            //// If you are using a tip tenant, placing this order will fail as it is not supported currently
            const string azureOfferUri = "/fbf178a5-144e-46d1-aa81-612c2d3f97f4/offers/MS-AZR-0145P";

            return new
            {
                line_items = new List<dynamic>()
                {
                    new
                    {
                        //// has to be a unique number for each line item
                        //// recommendation is to start with 0
                        line_item_number = 0,

                        //// this is the offer uri for the offer that is being purchased, refer to the excel sheet for this
                        offer_uri = azureOfferUri,

                        //// Max quantity for azure subscription is only 1
                        quantity = 1,

                        //// since a customer can have multiple azure subscriptions, one can use this friendly name to distinguish the subscriptions
                        //// when queried for subscription of this order line item, one would get this back in the friendlyname
                        friendlyname = "Engineering Team Azure Subscription"
                    }
                },
                //// customer cid for who the order is being placed
                recipient_customer_id = customerCid
            };
        }

        /// <summary>
        /// Sample for populating an azure order in TIP or Integration Sandbox
        /// </summary>
        /// <param name="customerCid">cid of the customer</param>
        /// <returns>order object</returns>
        public static dynamic PopulateAzureOrderInTIP(string customerCid)
        {
            //// This is the offer Uri of Azure Cloud Solution Provider from the excel sheet shared to partners
            //// If you are using a tip tenant, placing this order will fail as it is not supported currently
            const string azureOfferUri = "/fbf178a5-144e-46d1-aa81-612c2d3f97f4/offers/MS-AZR-0146P";

            return new
            {
                line_items = new List<dynamic>()
                {
                    new
                    {
                        //// has to be a unique number for each line item
                        //// recommendation is to start with 0
                        line_item_number = 0,

                        //// this is the offer uri for the offer that is being purchased, refer to the excel sheet for this
                        offer_uri = azureOfferUri,

                        //// Max quantity for azure subscription is only 1
                        quantity = 1,

                        //// since a customer can have multiple azure subscriptions, one can use this friendly name to distinguish the subscriptions
                        //// when queried for subscription of this order line item, one would get this back in the friendlyname
                        friendlyname = "Engineering Team Azure Subscription"
                    }
                },
                //// customer cid for who the order is being placed
                recipient_customer_id = customerCid
            };
        }

        /// <summary>
        /// Sample for populating an order from the given offer uri
        /// </summary>
        /// <param name="customerCid">cid of the customer</param>
        /// <param name="offerUri">The offer Uri</param>
        /// <param name="friendlyName">Friendly Name</param>
        /// <returns>order object</returns>
        public static dynamic PopulateOrderFromOfferUri(string customerCid, string offerUri, string friendlyName)
        {
           
            return new
            {
                line_items = new List<dynamic>()
                {
                    new
                    {
                        //// has to be a unique number for each line item
                        //// recommendation is to start with 0
                        line_item_number = 0,

                        //// this is the offer uri for the offer that is being purchased, refer to the excel sheet for this
                        offer_uri = offerUri,

                        //// Max quantity for azure subscription is only 1
                        quantity = 1,

                        //// since a customer can have multiple azure subscriptions, one can use this friendly name to distinguish the subscriptions
                        //// when queried for subscription of this order line item, one would get this back in the friendlyname
                        friendlyname = friendlyName
                    }
                },
                //// customer cid for who the order is being placed
                recipient_customer_id = customerCid
            };
        }

        /// <summary>
        /// This method retrieves all the orders placed for a customer by a reseller
        /// </summary>
        /// <param name="customerCid">cid of the customer</param>
        /// <param name="resellerCid">cid of the reseller</param>
        /// <param name="sa_Token">sales agent token</param>
        /// <returns>object that contains orders</returns>
        public static dynamic GetOrders(string customerCid, string resellerCid, string sa_Token)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/orders?recipient_customer_id={1}", resellerCid, customerCid));

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
                    var ordersResponse = Json.Decode(responseContent);

                    foreach (var order in ordersResponse.items)
                    {
                        PrintOrder(order);
                    }

                    return ordersResponse;
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
        /// This method returns the order given an orderId for a reseller
        /// </summary>
        /// <param name="orderId">id of the order</param>
        /// <param name="resellerCid">cid of the reseller</param>
        /// <param name="sa_Token">sales agent token</param>
        /// <returns>order object</returns>
        public static dynamic GetOrder(string orderId, string resellerCid, string sa_Token)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/orders/{1}", resellerCid, orderId));

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
                    var order = Json.Decode(responseContent);
                    PrintOrder(order);

                    return order;
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
        /// This method is used to place order on behalf of a customer by a reseller
        /// </summary>
        /// <param name="resellerCid">the cid of the reseller</param>
        /// <param name="order">new Order that can contain multiple line items</param>
        /// <param name="sa_Token">unexpired access token to call the partner apis</param>
        /// <returns>order information that has references to the subscription uris and entitlement uri for the line items</returns>
        public static dynamic PlaceOrder(dynamic order, string resellerCid, string sa_Token)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/orders", resellerCid));

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_Token);
            string content = Json.Encode(order);

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
                    var placedOrder = Json.Decode(responseContent);
                    PrintOrder(placedOrder);

                    return placedOrder;
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
        /// This method is used to patch an existing order by a reseller
        /// </summary>
        /// <param name="resellerCid">the cid of the reseller</param>
        /// <param name="order">new Order that can contain multiple line items</param>
        /// <param name="sa_Token">unexpired access token to call the partner apis</param>
        /// <returns>order information that has references to the subscription uris and entitlement uri for the line items</returns>
        public static dynamic PatchOrder(string id, dynamic order, string resellerCid, string sa_Token)
        {
            var previousOrder = GetOrder(id, resellerCid, sa_Token);

            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/orders/{1}", resellerCid, id));

            request.Method = "PATCH";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            request.Headers.Add("api-version", "2015-03-31");
            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + sa_Token);
            request.Headers.Add("If-Match", previousOrder.etag);
            string content = Json.Encode(order);

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
                    var patchedOrder = Json.Decode(responseContent);
                    PrintOrder(patchedOrder);
                    return patchedOrder;
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
        /// This method prints the information in an order
        /// </summary>
        /// <param name="order">Order that was placed</param>
        public static void PrintOrder(dynamic order)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("=========================================");
            Console.WriteLine("Order Information");
            Console.WriteLine("=========================================");
            Console.WriteLine("Id\t\t: {0}", order.id);
            Console.WriteLine("CustomerId\t: {0}", order.recipient_customer_id);
            Console.WriteLine("Etag\t\t: {0}", order.etag);
            Console.WriteLine("Created Date\t: {0}\n", order.creation_date);

            foreach (var line_item in order.line_items)
            {
                Console.WriteLine("LineItem {0}", line_item.line_item_number);
                Console.WriteLine("\tOfferUri\t\t: {0}", line_item.offer_uri);
                Console.WriteLine("\tQuantity\t\t: {0}", line_item.quantity);
                Console.WriteLine("\tSubscriptionUri\t\t: {0}", line_item.resulting_subscription_uri);
            }

            Console.WriteLine("=========================================");
            Console.ResetColor();
        }
    }
}