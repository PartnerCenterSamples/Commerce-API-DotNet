using System;
using System.Linq;
using System.Net;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
	internal class Orchestrator
	{
		/// <summary>
		/// Gets the customer entity for the given customer id
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="customerMicrosoftId">Microsoft Id of the customer</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		/// <returns>customer object</returns>
		public static dynamic GetCustomer(string defaultDomain, string appId, string key, string customerMicrosoftId,
				string resellerMicrosoftId)
		{
			// Get Active Directory token first
			AuthorizationToken adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

			// Using the ADToken get the sales agent token
			AuthorizationToken saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			var existingCustomerCid = Customer.GetCustomerCid(customerMicrosoftId, resellerMicrosoftId,
					saAuthorizationToken.AccessToken);
			// Get Customer token
			AuthorizationToken customerAuthorizationToken = Customer.GetCustomer_Token(existingCustomerCid,
					adAuthorizationToken);

			return Customer.GetCustomer(existingCustomerCid, customerAuthorizationToken.AccessToken);
		}

		/// <summary>
		/// Get all subscriptions placed by the reseller for the customer
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="customerMicrosoftId">Microsoft Id of the customer</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		/// <returns>object that contains all of the subscriptions</returns>
		public static dynamic GetSubscriptions(string defaultDomain, string appId, string key,
				string customerMicrosoftId, string resellerMicrosoftId)
		{
			// Get Active Directory token first
			AuthorizationToken adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

			// Using the ADToken get the sales agent token
			AuthorizationToken saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			// Get the Reseller Cid, you can cache this value
			string resellerCid = Reseller.GetCid(resellerMicrosoftId, saAuthorizationToken.AccessToken);

			// You can cache this value too
			var customerCid = Customer.GetCustomerCid(customerMicrosoftId, resellerMicrosoftId,
					saAuthorizationToken.AccessToken);

			return Subscription.GetSubscriptions(customerCid, resellerCid, saAuthorizationToken.AccessToken);
		}

		/// <summary>
		/// Get all orders placed by the reseller for this customer
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="customerMicrosoftId">Microsoft Id of the customer</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		/// <returns>object that contains orders</returns>
		public static dynamic GetOrders(string defaultDomain, string appId, string key, string customerMicrosoftId,
				string resellerMicrosoftId)
		{
			// Get Active Directory token first
			AuthorizationToken adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

			// Using the ADToken get the sales agent token
			AuthorizationToken saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			// Get the Reseller Cid, you can cache this value
			string resellerCid = Reseller.GetCid(resellerMicrosoftId, saAuthorizationToken.AccessToken);

			// You can cache this value too
			var customerCid = Customer.GetCustomerCid(customerMicrosoftId, resellerMicrosoftId,
					saAuthorizationToken.AccessToken);

			return Order.GetOrders(customerCid, resellerCid, saAuthorizationToken.AccessToken);
		}

		/// <summary>
		/// Create an Order
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="customerMicrosoftId">Microsoft Id of the customer</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		public static void CreateOrder(string defaultDomain, string appId, string key, string customerMicrosoftId,
				string resellerMicrosoftId)
		{
			// Get Active Directory token first
			AuthorizationToken adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

			// Using the ADToken get the sales agent token
			AuthorizationToken saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			// Get the Reseller Cid, you can cache this value
			string resellerCid = Reseller.GetCid(resellerMicrosoftId, saAuthorizationToken.AccessToken);

			// You can cache this value too
			var customerCid = Customer.GetCustomerCid(customerMicrosoftId, resellerMicrosoftId,
					saAuthorizationToken.AccessToken);

			// Populate a multi line item order
			var existingCustomerOrder = Order.PopulateOrderFromConsole(customerCid);
			// Place the order and subscription uri and entitlement uri are returned per each line item
			var existingCustomerPlacedOrder = Order.PlaceOrder(existingCustomerOrder, resellerCid,
					saAuthorizationToken.AccessToken);
			foreach (var line_Item in existingCustomerPlacedOrder.line_items)
			{
				var subscription = Subscription.GetSubscriptionByUri(line_Item.resulting_subscription_uri,
						saAuthorizationToken.AccessToken);
				Console.WriteLine("Subscription: {0}", subscription.Id);
			}
		}

		internal static void CreateCustomer()
		{
			//// Get input from the console application for creating a new customer
			//var customer = Customer.PopulateCustomerFromConsole();

			//// This is the created customer object that contains the cid, the microsoft tenant id etc
			//var createdCustomer = Customer.CreateCustomer(customer, resellerCid, saAuthorizationToken.AccessToken);

			//// Populate a multi line item order
			//var newCustomerOrder = Order.PopulateOrderWithMultipleLineItems(createdCustomer.customer.id);

			//// Place the order and subscription uri and entitlement uri are returned per each line item
			//var newCustomerPlacedOrder = Order.PlaceOrder(newCustomerOrder, resellerCid, saAuthorizationToken.AccessToken);

			//foreach (var line_Item in newCustomerPlacedOrder.line_items)
			//{
			//	var subscription = Subscription.GetSubscriptionByUri(line_Item.resulting_subscription_uri, saAuthorizationToken.AccessToken);
			//	Console.WriteLine("Subscription: {0}", subscription.Id);
			//}
		}

		/// <summary>
		/// Get a customer's usage information for the last 1 month, calculates the total cost using RateCard API 
		/// and Suspends the subscription if the total cost is more than the credit limit.
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="customerMicrosoftId">Microsoft Id of the customer</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		public static void GetRateCardAndUsage(string defaultDomain, string appId, string key,
				string customerMicrosoftId, string resellerMicrosoftId)
		{
			var correlationId = Guid.NewGuid().ToString();
			// Get Active Directory token first
			AuthorizationToken adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

			// Using the ADToken get the sales agent token
			AuthorizationToken saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			// Get the Reseller Cid, you can cache this value
			string resellerCid = Reseller.GetCid(resellerMicrosoftId, saAuthorizationToken.AccessToken);

			// You can cache this value too
			var customerCid = Customer.GetCustomerCid(customerMicrosoftId, resellerMicrosoftId,
					saAuthorizationToken.AccessToken);

			// Get Customer token
			AuthorizationToken customerAuthorizationToken = Customer.GetCustomer_Token(customerCid, adAuthorizationToken);

			// Gets the RateCard to get the prices
			var rateCard = RateCard.GetRateCard(resellerCid, saAuthorizationToken.AccessToken);

			var startTime = String.Format("{0:u}", DateTime.Today.AddDays(-30));
			var endTime = String.Format("{0:u}", DateTime.Today.AddDays(-1));

			// Get all of a Customer's entitlements
			var entitlements = Usage.GetEntitlements(customerCid, customerAuthorizationToken.AccessToken);

			try
			{
				foreach (var entitlement in entitlements.items)
				{
					// Get the usage for the given entitlement for the last 1 month
					var usageRecords = Usage.GetUsageRecords(resellerCid, entitlement.id, saAuthorizationToken.AccessToken,
							startTime, endTime);

					if (usageRecords.items.Count > 0)
					{
						Console.ForegroundColor = ConsoleColor.DarkGreen;
						Console.WriteLine("================================================================================");
						Console.WriteLine("\nPrices for Entitlement: {0}", entitlement.id);
						Console.WriteLine("================================================================================");

						double totalCost = 0;
						// Looping through the usage records to calculate the cost of each item
						foreach (UsageType usageRecord in usageRecords.items)
						{
							string meterId = usageRecord.meter_id;

							// Gets the price corresponding to the given meterId from the ratecard.
							Console.WriteLine("\nMeter Name\t\t: {0}", usageRecord.meter_name);
							double includedQty = Usage.GetIncludedQuantityByMeterID(rateCard, meterId);
							Console.WriteLine("Included Quantity\t\t: {0}", includedQty);
							double consumedQty = (double)usageRecord.quantity;
							Console.WriteLine("Consumed Quantity\t\t: {0}", consumedQty);
							double ratableUsage = Usage.GetRatableUsage(consumedQty, includedQty);
							double cost = Usage.computeRatedUsagePerMeter(Usage.GetRatesForMeterID(rateCard, meterId), ratableUsage);
							Console.WriteLine("Cost\t\t: {0}", cost);
							totalCost += cost;


						}
						Console.WriteLine("\nTOTAL COST:  {0}", totalCost);
						// Setting the credit limit below the total cost for testing this scenario
						double creditLimit = totalCost - 1;
						// Suspends the subscription if the total cost is above the credit limit.
						if (totalCost > creditLimit)
						{
							var subscription = Subscription.GetSubscriptionByUri(entitlement.billing_subscription_uri,
									saAuthorizationToken.AccessToken);
							Subscription.SuspendSubscription(subscription.id, resellerCid, saAuthorizationToken.AccessToken);
						}
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());

			}
		}

		/// <summary>
		/// Transition an Office 365 subscription to another (Office 365 Enterprise E1) SKU.
		/// Creates a subscription stream and gather events from the stream and show them on console.
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="customerMicrosoftId">Microsoft Id of the customer</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		public static void TransitionToNewSKU(string subscriptionId, string defaultDomain, string appId, string key,
				string customerMicrosoftId, string resellerMicrosoftId)
		{
			// Get Active Directory token first
			AuthorizationToken adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

			// Using the ADToken get the sales agent token
			AuthorizationToken saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			// Get the Reseller Cid, you can cache this value
			string resellerCid = Reseller.GetCid(resellerMicrosoftId, saAuthorizationToken.AccessToken);

			// You can cache this value too
			var customerCid = Customer.GetCustomerCid(customerMicrosoftId, resellerMicrosoftId,
					saAuthorizationToken.AccessToken);

			// Correlation Id to be used for this scenaario
			var correlationId = Guid.NewGuid().ToString();

			// Offer Uri for an Office 365 Enterprise E1 subscription
			string newOfferUri = PromptForOfferUri();

			Subscription.Transition(subscriptionId, customerCid, newOfferUri, resellerCid,
					saAuthorizationToken.AccessToken);

			WaitForSuccessfulTransition(resellerCid, subscriptionId, saAuthorizationToken.AccessToken);
		}

		/// <summary>
		/// List all customers for the reseller, loops through them and delete one by one.
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="customerMicrosoftId">Microsoft Id of the customer</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		public static void ListAndDeleteAllCustomers(string defaultDomain, string appId, string key,
				string resellerMicrosoftId, string customerMicrosoftId)
		{
			// Get Active Directory token first
			AuthorizationToken adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

			// Using the ADToken get the sales agent token
			AuthorizationToken saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			// Get the Reseller Cid, you can cache this value
			string resellerCid = Reseller.GetCid(resellerMicrosoftId, saAuthorizationToken.AccessToken);

			var customers = Customer.GetAllCustomers(resellerMicrosoftId, adAuthorizationToken.AccessToken);

			foreach (var customer in customers.value)
			{
				Console.WriteLine("\nCustomer Info");
				Console.WriteLine("\tCustomer Id\t: {0}", customer.customerContextId);
				Console.WriteLine("\tName\t: {0}", customer.displayName);
				var existingCustomerCid = Customer.GetCustomerCid(customer.customerContextId, resellerMicrosoftId,
						saAuthorizationToken.AccessToken);
				Console.WriteLine("Deleting Customer " + customer.displayName);
				Customer.DeleteCustomer(resellerCid, existingCustomerCid, saAuthorizationToken.AccessToken);
			}
		}

		/// <summary>
		/// Monitor the stream api, to see if the transition is successful.
		/// </summary>
		/// <param name="resellerCid">cid of the reseller</param>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="saToken">sales agent token</param>
		private static void WaitForSuccessfulTransition(string resellerCid, string subscriptionId, string saToken)
		{
			Boolean transitionComplete = false;
			const string transitionCompletedStatus = "subscription_provisioned";
			var subscriptionUri = String.Format("/{0}/subscriptions/{1}", resellerCid, subscriptionId);
			// Nmae for the subscription stream
			const string streamName = "SampleCodeSubscriptionStream";

			Subscription.CreateSubscriptionStream(resellerCid, streamName,
					saToken);

			var subStream = Subscription.GetSubscriptionStream(resellerCid, streamName,
					saToken);
			var items = subStream.items;

			while (!transitionComplete)
			{
				foreach (var item in items)
				{
					if (subscriptionUri.Equals(item.subscription_uri) && transitionCompletedStatus.Equals(item.type))
					{
						transitionComplete = true;
						break;
					}
				}
				if (!transitionComplete)
				{
					subStream = Subscription.MarkPageAsCompletedInStream(subStream.links.completion.href,
							saToken);
					items = subStream.items;
				}
			}
		}

		/// <summary>
		///  Prompts user to select the offer to transition to.
		/// </summary>
		/// <returns>offer uri</returns>
		private static string PromptForOfferUri()
		{
			GroupedOffers selectedGroupedOffers =
					OfferCatalog.Instance.GroupedOffersCollection.First(
							groupedOffers => groupedOffers.OfferType == OfferType.IntuneAndOffice);
			Console.Write("\nSelect Offer (by index):\n ");
			foreach (
					var item in
							selectedGroupedOffers.Offers.Select((offer, index) => new { Offer = offer, Index = index }))
			{
				Console.WriteLine("{0}. {1}", item.Index + 1, item.Offer.Name);
			}
			bool done = false;
			int selectedIndex = -1;
			do
			{
				string input = Console.ReadLine().Trim();

				if (int.TryParse(input, out selectedIndex))
				{
					done = true;
				}
			} while (!done);
			var selectedOffer = selectedGroupedOffers.Offers.ElementAt(selectedIndex - 1);
			return selectedOffer.Uri;
		}
	}
}