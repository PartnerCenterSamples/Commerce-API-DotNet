/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
    using System;
    using System.Configuration;

    static class Program
    {
        /// <summary>
        /// If you an existing Customer, this is the Microsoft Id of that existing customer
        /// Replace the string below with the id of the customer
        /// You can get this by 
        ///     1. going to Partner center at https://partnercenter.microsoft.com/
        ///     2. Sign in with your credentials
        ///     3. Click View Customers
        ///     4. Expand the customer you are interested in by clicking on the chevron (down arrow)
        ///     5. You will find the Microsoft ID, copy that value to this variable
        /// </summary>
        private const string ExistingCustomerMicrosoftId = "Existing customer's Microsoft Id goes here";

        private static AuthorizationToken adAuthorizationToken { get; set; }

        private static AuthorizationToken saAuthorizationToken { get; set; }

        private static AuthorizationToken customerAuthorizationToken { get; set; }

        static void Main()
        {
            Console.Write("\nHave you updated the app.config, with the settings from https://partnercenter.microsoft.com (y/n)? ");
            string response = Console.ReadLine().Trim().ToUpperInvariant();
            if (response != "Y" && response != "YES")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nUpdate AppId, Key, MicrosoftId, DefaultDomain in the app.config and run the app again");

                Console.ResetColor();
                Console.Write("\n\n\nHit enter to exit the app now");
                Console.ReadLine();
                return;
            }

            // This is the Microsoft ID of the reseller
            // Please work with your Admin Agent to get it from https://partnercenter.microsoft.com/en-us/pc/AccountSettings/TenantProfile
            string microsoftId = ConfigurationManager.AppSettings["MicrosoftId"];

            // This is the default domain of the reseller
            // Please work with your Admin Agent to get it from https://partnercenter.microsoft.com/en-us/pc/AccountSettings/TenantProfile
            string defaultDomain = ConfigurationManager.AppSettings["DefaultDomain"];

            // This is the appid that is registered for this application in Azure Active Directory (AAD)
            // Please work with your Admin Agent to get it from  https://partnercenter.microsoft.com/en-us/pc/ApiIntegration/Overview 
            string appId = ConfigurationManager.AppSettings["AppId"];

            // This is the key for this application in Azure Active Directory
            // This is only available at the time your admin agent has created a new app at https://partnercenter.microsoft.com/en-us/pc/ApiIntegration/Overview
            // You could alternatively goto Azure Active Directory and generate a new key, and use that.
            string key = ConfigurationManager.AppSettings["Key"];

            try
            {
                // Get Active Directory token first
                adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

                // Using the ADToken get the sales agent token
                saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

                // Get the Reseller Cid, you can cache this value
                string resellerCid = Reseller.GetCid(microsoftId, saAuthorizationToken.AccessToken);

                // Get input from the console application for creating a new customer
                var customer = Customer.PopulateCustomerFromConsole();

                // This is the created customer object that contains the cid, the microsoft tenant id etc
                var createdCustomer = Customer.CreateCustomer(customer, resellerCid, saAuthorizationToken.AccessToken);

                // Populate a multi line item order
                var newCustomerOrder = Order.PopulateOrderWithMultipleLineItems(createdCustomer.customer.id);

                // Place the order and subscription uri and entitlement uri are returned per each line item
                var newCustomerPlacedOrder = Order.PlaceOrder(newCustomerOrder, resellerCid, saAuthorizationToken.AccessToken);

                foreach (var line_Item in newCustomerPlacedOrder.line_items)
                {
                    var subscription = Subscription.GetSubscriptionByUri(line_Item.resulting_subscription_uri, saAuthorizationToken.AccessToken);
                    Console.WriteLine("Subscription: {0}", subscription.Id);
                }

                // You can cache this value too
                var existingCustomerCid = Customer.GetCustomerCid(ExistingCustomerMicrosoftId, microsoftId, saAuthorizationToken.AccessToken);

                customerAuthorizationToken = Customer.GetCustomer_Token(existingCustomerCid, adAuthorizationToken);

                // Get all subscriptions placed by the reseller for the customer
                var subscriptions = Subscription.GetSubscriptions(existingCustomerCid, resellerCid, saAuthorizationToken.AccessToken);

                // Get all orders placed by the reseller for this customer
                var orders = Order.GetOrders(existingCustomerCid, resellerCid, saAuthorizationToken.AccessToken);

                // Populate a multi line item order
                var existingCustomerOrder = Order.PopulateOrderFromConsole(existingCustomerCid);

                // Place the order and subscription uri and entitlement uri are returned per each line item
                var existingCustomerPlacedOrder = Order.PlaceOrder(existingCustomerOrder, resellerCid, saAuthorizationToken.AccessToken);

                foreach (var line_Item in existingCustomerPlacedOrder.line_items)
                {
                    var subscription = Subscription.GetSubscriptionByUri(line_Item.resulting_subscription_uri, saAuthorizationToken.AccessToken);
                    Console.WriteLine("Subscription: {0}", subscription.Id);
                }
            }
            catch (System.FieldAccessException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n\n\n Looks like you are debugging the application.  Inorder to fix this exception: "
                    + "\n 1. In Visual Studio, Right Click on the Project Microsoft.CSP.Api.V1.Samples"
                    + "\n 2. Select the Debug Tab"
                    + "\n 3. Uncheck the option \"Enable the Visual Studio hosting process\" (it is at the bottom of the page)"
                    + "\n 4. Save the changes (File -> Save Selected Items)"
                    + "\n 5. Debug the app now.");
                Console.Write("Make sure you copy/remember the above steps before exiting the app.");
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n Make sure the app.config has all the right settings.  The defaults in the app.config won't work."
                    + "\n If the settings are correct, its possible you are hitting a service error.  Try again."
                    + "\n If the error persists, contact support");
            }


            Console.ResetColor();
            Console.Write("\n\n\nHit enter to exit the app...");
            Console.ReadLine();
        }
    }
}
