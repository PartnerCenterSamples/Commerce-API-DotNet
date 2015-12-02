using Microsoft.Partner.CSP.Api.V1.Samples.DataModels;
using System;
using System.Linq;
using System.Net;

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
		public static string CreateOrder(string defaultDomain, string appId, string key,
																		 string customerMicrosoftId, string resellerMicrosoftId)
		{
			string result = String.Empty;

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
				result = subscription.Id;
				Console.WriteLine("Subscription: {0}", subscription.Id);
			}

			return result;
		}

		/// <summary>
		/// Create an Order for Azure in Integration Sandbox
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="customerMicrosoftId">Microsoft Id of the customer</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		/// <returns></returns>
		public static string CreateOrderWithAzureTip(string defaultDomain, string appId, string key,
																									string customerMicrosoftId, string resellerMicrosoftId)
		{
			string result = String.Empty;

			// Get Active Directory token first
			AuthorizationToken adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

			// Using the ADToken get the sales agent token
			AuthorizationToken saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			// Get the Reseller Cid, you can cache this value
			string resellerCid = Reseller.GetCid(resellerMicrosoftId, saAuthorizationToken.AccessToken);

			// You can cache this value too
			var customerCid = Customer.GetCustomerCid(customerMicrosoftId, resellerMicrosoftId,
					saAuthorizationToken.AccessToken);

			// Populate an Azure in TIP order
			var newOrder = Order.PopulateAzureOrderInTIP(customerCid);
			// Place the order and subscription uri and entitlement uri are returned per each line item
			var existingCustomerPlacedOrder = Order.PlaceOrder(newOrder, resellerCid,
																												 saAuthorizationToken.AccessToken);
			foreach (var line_Item in existingCustomerPlacedOrder.line_items)
			{
				var subscription = Subscription.GetSubscriptionByUri(line_Item.resulting_subscription_uri,
																														 saAuthorizationToken.AccessToken);
				result = subscription.Id;
				Console.WriteLine("Subscription: {0}", subscription.Id);
			}

			return result;
		}
		/// <summary>
		/// Create a new customer
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		/// <returns></returns>
		internal static string CreateCustomer(string defaultDomain, string appId, string key,
				string resellerMicrosoftId)
		{
			// Get Active Directory token first
			AuthorizationToken adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

			// Using the ADToken get the sales agent token
			AuthorizationToken saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			// Get the Reseller Cid, you can cache this value
			string resellerCid = Reseller.GetCid(resellerMicrosoftId, saAuthorizationToken.AccessToken);
			// Get input from the console application for creating a new customer
			var customer = Customer.PopulateCustomerFromConsole();

			// This is the created customer object that contains the cid, the microsoft tenant id etc
			var createdCustomer = Customer.CreateCustomer(customer, resellerCid, saAuthorizationToken.AccessToken);
			if (createdCustomer == null)
			{
				throw new Exception("Error creating customer");
			}
			string newCustomerMicrosoftId = createdCustomer.customer.identity.data.tid;

			// Populate a multi line item order
			var newCustomerOrder = Order.PopulateOrderWithMultipleLineItems(createdCustomer.customer.id);

			// Place the order and subscription uri and entitlement uri are returned per each line item
			var newCustomerPlacedOrder = Order.PlaceOrder(newCustomerOrder, resellerCid, saAuthorizationToken.AccessToken);

			foreach (var line_Item in newCustomerPlacedOrder.line_items)
			{
				var subscription = Subscription.GetSubscriptionByUri(line_Item.resulting_subscription_uri, saAuthorizationToken.AccessToken);
				Console.WriteLine("Subscription: {0}", subscription.Id);
			}

			return newCustomerMicrosoftId;
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
						double creditLimit = 100;
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

		/// <summary>
		/// Creates a virtual machine in Azure. 
		/// Creates all the required resources before creating VM.
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="credentialName">Internet name/address used to identify entry in Credential Manager</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="customerTenantId">Id or Domain of the customer Azure tenant</param>
		private static void CreateAzureVirtualMachine(string appId, string credentialName, string customerTenantId, string subscriptionId)
		{
			// Get Azure Authentication Token
			string azureToken = Reseller.GetAzureAuthTokenForCustomerTenant(appId, credentialName, customerTenantId);
			// Correlation Id to be used for this scenaario
			var correlationId = Guid.NewGuid().ToString();

			var resourceGroupName = Guid.NewGuid().ToString();

			// Appending suffix to resource group name to build names of other resources

			// Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only. 
			// So removing hyphen and truncating.
			var storageAccountName = String.Format("{0}sa", resourceGroupName.Replace("-", "").Remove(20));

			var networkSecurityGroupName = String.Format("{0}_nsg", resourceGroupName);
			var networkSecurityGroupId =
					String.Format(
							"/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Network/networkSecurityGroups/{2}",
							subscriptionId, resourceGroupName, networkSecurityGroupName);

			var virtualNetworkName = String.Format("{0}_vn", resourceGroupName);
			var subnetName = String.Format("{0}_sn", resourceGroupName);
			var subNetId =
					String.Format(
							"/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.network/virtualNetworks/{2}/subnets/{3}",
							subscriptionId, resourceGroupName, virtualNetworkName, subnetName);

			var publicIpName = String.Format("{0}_pip", resourceGroupName);
			var publicIpId =
					String.Format(
							"/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Network/publicIPAddresses/{2}",
							subscriptionId, resourceGroupName, publicIpName);

			var networkInterfaceName = String.Format("{0}_nic", resourceGroupName);
			var networkInterfaceId =
					String.Format(
							"/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Network/networkInterfaces/{2}",
							subscriptionId, resourceGroupName, networkInterfaceName);

			var ipName = String.Format("{0}_ip", resourceGroupName);
			var vitualMachineName = String.Format("{0}_vm", resourceGroupName);

			// Waiting Time (in seconds) For Resource to be Provisioned
			var retryAfter = 5;
			// Retry attempts for resource provisioning errors
			var retryAttempts = 5;
			var attempts = 0;

			// The following resources are to be created in order before creating a virtual machine.

			// #1 Create Resource Group
			do
			{
				var createResourceGroupResponse = AzureResourceManager.CreateResourceGroup(subscriptionId, resourceGroupName,
																																									 azureToken, correlationId);
				if (createResourceGroupResponse == null)
				{
					attempts++;
				}
				else
				{
					attempts = retryAttempts + 1;

					// Waiting for the resource group to be created, if the request is just Accepted and the creation is still pending.
					if ((int)(createResourceGroupResponse.StatusCode) == (int)HttpStatusCode.Accepted)
					{
						AzureResourceManager.WaitForResourceGroupProvisioning(subscriptionId, resourceGroupName, retryAfter,
								azureToken, correlationId);
					}
				}
			} while (attempts < retryAttempts);

			// #2 Create Storage Account
			// Register the subscription with Storage Resource Provider, for creating Storage Account
			// Storage Resource Provider
			const string storageProviderName = "Microsoft.Storage";
			AzureResourceManager.RegisterProvider(subscriptionId, storageProviderName, azureToken, correlationId);

			attempts = 0;
			do
			{
				var storageAccountResponse = AzureResourceManager.CreateStorageAccount(subscriptionId, resourceGroupName,
																																							 storageAccountName, azureToken, correlationId);
				if (storageAccountResponse == null)
				{
					attempts++;
				}
				else
				{
					attempts = retryAttempts + 1;
					// Waiting for the storage account to be created, if the request is just Accepted and the creation is still pending.
					if ((int)(storageAccountResponse.StatusCode) == (int)HttpStatusCode.Accepted)
					{
						var location = storageAccountResponse.Headers.Get("Location");
						retryAfter = Int32.Parse(storageAccountResponse.Headers.Get("Retry-After"));
						AzureResourceManager.WaitForStorageAccountProvisioning(location, resourceGroupName, retryAfter,
								azureToken, correlationId);
					}
				}
			} while (attempts < retryAttempts);

			// Register the subscription with Network Resource Provider for creating Network Resources - Netowrk Securtiy Group, Virtual Network, Subnet, Public IP and Network Interface
			// Network Resource Provider
			const string networkProviderName = "Microsoft.Network";
			AzureResourceManager.RegisterProvider(subscriptionId, networkProviderName, azureToken, correlationId);

			// #3 Create Network Security Group
			attempts = 0;
			do
			{
				var networkSecurityGroupResponse = AzureResourceManager.CreateNetworkSecurityGroup(subscriptionId,
																											resourceGroupName, networkSecurityGroupName, azureToken, correlationId);
				if (networkSecurityGroupResponse == null)
				{
					attempts++;
				}
				else
				{
					attempts = retryAttempts + 1;
					// Waiting for the network security group to be created, if the request is just Accepted and the creation is still pending.
					if ((int)(networkSecurityGroupResponse.StatusCode) == (int)HttpStatusCode.Accepted)
					{
						AzureResourceManager.WaitForNetworkSecurityGroupProvisioning(subscriptionId, resourceGroupName,
								networkSecurityGroupName, retryAfter, azureToken, correlationId);
					}
				}
			} while (attempts < retryAttempts);


			// #4 Create Virtual Network
			attempts = 0;
			do
			{
				var virtualNetworkResponse = AzureResourceManager.CreateVirtualNetwork(subscriptionId, resourceGroupName,
																																		virtualNetworkName, azureToken, correlationId);
				if (virtualNetworkResponse == null)
				{
					attempts++;
				}
				else
				{
					attempts = retryAttempts + 1;
					// Waiting for the virtual network to be created, if the request is just Accepted and the creation is still pending.
					if ((int)(virtualNetworkResponse.StatusCode) == (int)HttpStatusCode.Accepted)
					{
						AzureResourceManager.WaitForVirtualNetworkProvisioning(subscriptionId, resourceGroupName,
								virtualNetworkName, retryAfter, azureToken, correlationId);
					}
				}
			} while (attempts < retryAttempts);

			// #5 Create Subnet
			attempts = 0;
			do
			{
				var subNetResponse = AzureResourceManager.CreateSubNet(subscriptionId, resourceGroupName, virtualNetworkName,
																														networkSecurityGroupId, subnetName, azureToken, correlationId);
				if (subNetResponse == null)
				{
					attempts++;
				}
				else
				{
					attempts = retryAttempts + 1;
					// Waiting for the subnet to be created, if the request is just Accepted and the creation is still pending.
					if ((int)(subNetResponse.StatusCode) == (int)HttpStatusCode.Accepted)
					{
						AzureResourceManager.WaitForSubNetProvisioning(subscriptionId, resourceGroupName, virtualNetworkName,
								subnetName, retryAfter, azureToken, correlationId);
					}
				}
			} while (attempts < retryAttempts);

			// #6 Create Public IP Address
			attempts = 0;
			do
			{
				var publicIpResponse = AzureResourceManager.CreatePublicIpAddress(subscriptionId, resourceGroupName,
																																							publicIpName, azureToken, correlationId);
				if (publicIpResponse == null)
				{
					attempts++;
				}
				else
				{
					attempts = retryAttempts + 1;
					// Waiting for the public IP to be created, if the request is just Accepted and the creation is still pending.
					if ((int)(publicIpResponse.StatusCode) == (int)HttpStatusCode.Accepted)
					{
						AzureResourceManager.WaitForPublicIpProvisioning(subscriptionId, resourceGroupName, publicIpName,
								retryAfter, azureToken, correlationId);
					}
				}
			} while (attempts < retryAttempts);

			// #7 Create Network Interface
			attempts = 0;
			do
			{
				var networkInterfaceResponse = AzureResourceManager.CreateNetworkInterface(subscriptionId, resourceGroupName,
										networkInterfaceName, networkSecurityGroupId, ipName, publicIpId, subNetId, azureToken, correlationId);
				if (networkInterfaceResponse == null)
				{
					attempts++;
				}
				else
				{
					attempts = retryAttempts + 1;
					// Waiting for the network interface to be created, if the request is just Accepted and the creation is still pending.
					if ((int)(networkInterfaceResponse.StatusCode) == (int)HttpStatusCode.Accepted)
					{
						AzureResourceManager.WaitForNetworkInterfaceProvisioning(subscriptionId, resourceGroupName,
								networkInterfaceName, retryAfter, azureToken, correlationId);
					}
				}
			} while (attempts < retryAttempts);

			// #8 Create Azure Virtual Machine
			// Compute Resource Provider
			const string computeProviderName = "Microsoft.Compute";
			AzureResourceManager.RegisterProvider(subscriptionId, computeProviderName, azureToken, correlationId);
			var virtualMachineResponse = AzureResourceManager.CreateVirtualMachine(subscriptionId, resourceGroupName,
				networkInterfaceId, storageAccountName, vitualMachineName, azureToken, correlationId);
			if (virtualMachineResponse == null)
			{
				return;
			}

			// #9  Create a New User
			var newUserInfo = User.PopulateUserFromConsole();
			var newUser = AzureResourceManager.CreateUser(newUserInfo.displayName, newUserInfo.mailNickname,
																										newUserInfo.userPrincipalName, newUserInfo.password,
																										azureToken, correlationId);
			// #10 Add user to Owner role
			// Register the subscription with Authorization Provider
			const string authorizationProviderName = "Microsoft.Authorization";
			var registrationResult = AzureResourceManager.RegisterProvider(subscriptionId, authorizationProviderName, azureToken, correlationId);

			// Role Id for Role 'Owner'
			const string roleIdForOwner = "8e3af657-a8ff-443c-a75c-2fe8c4bcb635";
			var scope = String.Format("/subscriptions/{0}", subscriptionId);
			// Assigning 'Owner' role to the new user 
			var roleAssignment = AzureResourceManager.CreateRoleAssignment(azureToken, scope, newUser.objectId, roleIdForOwner,
					correlationId);
		}

		/// <summary>
		/// Creates a virtual machine in a new Azure subscription for a new customer. 
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="ptrCtrAppId">appId for this program that is registered in the Partner Center</param>
		/// <param name="ptrCtrAppSecret">app secret for this program that is registered in the Partner Center</param>
		/// <param name="resellerMicrosoftId">cid of the reseller</param>
		/// <param name="azureAppId">appId for this program that is registered in the reseller azure directory and configured for pre-consent</param>
		/// <param name="credentialName">Internet or network address label of entry in credential manager</param>
		public static void CreateVirtualMachineInNewSubscription(string defaultDomain, string ptrCtrAppId, string ptrCtrAppSecret,
																														 string resellerMicrosoftId, string azureAppId, string credentialName)
		{
			// Get Active Directory token first
			AuthorizationToken adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, ptrCtrAppId, ptrCtrAppSecret);

			// Using the ADToken get the sales agent token
			AuthorizationToken saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			// Get the Reseller Cid, you can cache this value
			string resellerCid = Reseller.GetCid(resellerMicrosoftId, saAuthorizationToken.AccessToken);

			var s = Stream.CreateStream(resellerCid, saAuthorizationToken.AccessToken);

			string newCustomerMicrosoftId = Orchestrator.CreateCustomer(defaultDomain, ptrCtrAppId, ptrCtrAppSecret, resellerMicrosoftId);
			string newSubscriptionId = Orchestrator.CreateOrderWithAzureTip(defaultDomain, ptrCtrAppId, ptrCtrAppSecret, newCustomerMicrosoftId, resellerMicrosoftId);

			bool continueProcess = Stream.WaitForSubscriptionEvent(resellerCid, saAuthorizationToken.AccessToken, newSubscriptionId, StreamEvents.Subscription.Provisioned);

			if (continueProcess)
			{
				CreateAzureVirtualMachine(azureAppId, credentialName, newCustomerMicrosoftId, newSubscriptionId);
			}

			Stream.DeleteStream(resellerCid, saAuthorizationToken.AccessToken);
		}
	}
}