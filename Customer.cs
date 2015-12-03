/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
	using System;
	using System.IO;
	using System.Net;
	using System.Web.Helpers;

	public static class Customer
	{
		/// <summary>
		/// Method to populate the customer from Console which will be used to create customer
		/// </summary>
		/// <returns>object that can be passed to the CreateCustomer method to Create a Customer</returns>
		public static dynamic PopulateCustomerFromConsole()
		{
			Console.Clear();
			Console.ResetColor();
			Console.WriteLine("=========================================");
			Console.WriteLine("Create New Customer");
			Console.WriteLine("=========================================");

			Console.Write("Enter domain prefix to be created (do not 'onmicrosoft.com'):");
			var domainprefix = Console.ReadLine().Trim();

			Console.Write("Company Name\t: ");
			var company_name = Console.ReadLine().Trim();

			Console.Write("First Name\t: ");
			var first_name = Console.ReadLine().Trim();

			Console.Write("Last Name\t: ");
			var last_name = Console.ReadLine().Trim();

			Console.Write("Email\t\t: ");
			var email = Console.ReadLine().Trim();

			Console.Write("Address Line1\t: ");
			var address_line1 = Console.ReadLine().Trim();

			Console.Write("Address Line2\t: ");
			var address_line2 = Console.ReadLine().Trim();

			Console.Write("City\t\t: ");
			var city = Console.ReadLine().Trim();

			Console.Write("State\t\t: ");
			var region = Console.ReadLine().Trim();

			Console.Write("ZipCode\t\t: ");
			var postal_code = Console.ReadLine().Trim();

			Console.Write("PhoneNumber\t: ");
			var phone_number = Console.ReadLine().Trim();

			Console.Write("Country\t\t: ");
			var country = Console.ReadLine().Trim();

			return new
			{
				domain_prefix = domainprefix,
				user_name = "admin",
				password = "Password!1",
				profile = new
				{
					first_name,
					last_name,
					email,
					company_name,
					culture = "en-US",
					language = "en",
					type = "organization",
					default_address = new
					{
						first_name,
						last_name,
						address_line1,
						address_line2,
						city,
						region,
						postal_code,
						country,
						phone_number
					}
				}
			};
		}

		/// <summary>
		/// Given the Customer Microsoft ID, and the Reseller Microsoft ID, this method will retrieve the customer cid that can be used to perform transactions on behalf of the customer using the partner APIs
		/// </summary>
		/// <param name="customerMicrosoftId">Microsoft ID of the customer, this is expected to be available to the reseller</param>
		/// <param name="resellerMicrosoftId">Microsoft ID of the reseller</param>
		/// <param name="sa_Token">unexpired access token to call the partner apis</param>
		/// <returns>customer cid that can be used to perform transactions on behalf of the customer by the reseller</returns>
		public static string GetCustomerCid(string customerMicrosoftId, string resellerMicrosoftId, string sa_Token)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(
									string.Format(
											"https://api.cp.microsoft.com/customers/get-by-identity?provider=AAD&type=external_group&tid={0}&etid={1}",
											customerMicrosoftId, resellerMicrosoftId));

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
					return Json.Decode(responseContent).id;
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
		/// This method is used to get the customer token given a customer cid and the ad token
		/// </summary>
		/// <param name="customerCid">cid of the customer</param>
		/// <param name="adAuthorizationToken">active directory authorization token</param>
		/// <param name="customerAuthorizationToken">customer authorization token if available</param>
		/// <returns>customer authorization token</returns>
		public static AuthorizationToken GetCustomer_Token(string customerCid, AuthorizationToken adAuthorizationToken,
				AuthorizationToken customerAuthorizationToken = null)
		{
			if (customerAuthorizationToken == null ||
					(customerAuthorizationToken != null && customerAuthorizationToken.IsNearExpiry()))
			{
				//// Refresh the token on one of two conditions
				//// 1. If the token has never been retrieved
				//// 2. If the token is near expiry

				var customerToken = GetCustomer_Token(customerCid, adAuthorizationToken.AccessToken);
				customerAuthorizationToken = new AuthorizationToken(customerToken.access_token,
						Convert.ToInt64(customerToken.expires_in));
			}

			return customerAuthorizationToken;
		}

		/// <summary>
		/// This method is used to create a customer in the Microsoft reseller ecosystem by the reseller
		/// </summary>
		/// <param name="customer">customer information: domain, admin credentials for the new tenant, address, primary contact info</param>
		/// <param name="resellerCid">reseller cid</param>
		/// <param name="sa_Token">unexpired access token to access the partner apis</param>
		/// <returns>the newly created customer information: all of the above from customer, customer cid, customer microsoft id</returns>
		public static dynamic CreateCustomer(dynamic customer, string resellerCid, string sa_Token)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(
									string.Format("https://api.cp.microsoft.com/{0}/customers/create-reseller-customer", resellerCid));

			request.Method = "POST";
			request.ContentType = "application/json";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + sa_Token);
			string content = Json.Encode(customer);
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
					var createdCustomerResponse = Json.Decode(responseContent);
					PrintCreatedCustomer(createdCustomerResponse);
					return createdCustomerResponse;
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
		/// This method is used to get customer info in the Microsoft reseller ecosystem by the reseller
		/// </summary>
		/// <param param name="customerCid">cid of the customer whose information we are trying to retrieve</param>
		/// <param name="customer_Token">unexpired access token to access the partner apis</param>
		/// <returns>the created customer information: customer cid, customer microsoft id</returns>
		public static dynamic GetCustomer(string customerCid, string customer_Token)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/customers/{0}", customerCid));

			request.Method = "GET";
			request.ContentType = "application/json";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + customer_Token);

			try
			{
				Utilities.PrintWebRequest(request, string.Empty);
				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
					var customer = Json.Decode(responseContent);
					PrintCustomerInfo(customer);
					return customer;
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
		/// This method is used to get all the customers for the reseller
		/// </summary>
		/// <param name="resellerCid"cid of the reseller></param>
		/// <param name="sa_Token">sales agent token</param>
		/// <returns>list of customers</returns>
		public static dynamic GetAllCustomers(string resellerCid, string sa_Token)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(string.Format("https://graph.windows.net/{0}/contracts?api-version=1.6",
									resellerCid));

			request.Method = "GET";
			request.ContentType = "application/json";
			request.Accept = "application/json";

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
		/// Delete a customer account from the integration sandbox
		/// </summary>
		/// <param name="resellerCid">Cid of reseller</param>
		/// <param param name="customerCid">cid of the customer whose information we are trying to retrieve</param>
		/// <param name="sa_Token">sales agent token</param>
		/// <returns>the created customer information: customer cid, customer microsoft id</returns>
		public static dynamic DeleteCustomer(string resellerCid, string customerCid, string sa_Token)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(
									string.Format("https://api.cp.microsoft.com/{0}/customers/delete-tip-reseller-customer",
											resellerCid));

			request.Method = "POST";
			request.ContentType = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + sa_Token);

			string content = Json.Encode(new { customer_id = customerCid });
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
		/// Given the Azure AD token for a reseller, this method retrieves the customer token for accessing customer profile or entitlements
		/// </summary>
		/// <param name="ad_Token">Azure AD token for a reseller</param>
		/// <returns>the customer_token object which contains access_token, expiration duration to perform actions on the customer object</returns>
		private static dynamic GetCustomer_Token(string customerCid, string ad_Token)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/{0}/tokens", customerCid));

			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + ad_Token);
			string content = "grant_type=client_credentials";

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
		/// This method prints the customer returned from crest
		/// </summary>
		/// <param name="createdCustomerResponse">customer that was created</param>
		private static void PrintCreatedCustomer(dynamic createdCustomerResponse)
		{
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("=========================================");
			Console.WriteLine("Created Customer Information");
			Console.WriteLine("=========================================");

			PrintCustomerInfo(createdCustomerResponse.customer);

			Console.WriteLine("\nActive Directory Info");
			Console.WriteLine("\tDomain: \t{0}.onmicrosoft.com", createdCustomerResponse.domain_prefix);
			Console.WriteLine("\tUserName: \t{0}", createdCustomerResponse.user_name);
			Console.WriteLine("\tPassword: \t{0}", createdCustomerResponse.password);

			PrintCustomerProfile(createdCustomerResponse.profile);

			Console.WriteLine("=========================================");
			Console.ResetColor();
		}

		/// <summary>
		/// Prints the customer information
		/// </summary>
		/// <param name="customer">customer information object</param>
		private static void PrintCustomerInfo(dynamic customer)
		{
			Console.WriteLine("\nCustomer Info");
			Console.WriteLine("\tMicrosoftId\t: {0}", customer.identity.data.tid);
			Console.WriteLine("\tCid\t\t: {0}", customer.id);
		}

		/// <summary>
		/// Prints the customer profile information
		/// </summary>
		/// <param name="profile">profile to be printed</param>
		private static void PrintCustomerProfile(dynamic profile)
		{
			Console.WriteLine("\nProfile Info");
			Console.WriteLine("\tProfile Id\t: {0}", profile.id);
			Console.WriteLine("\tEmail\t\t: {0}", profile.email);
			Console.WriteLine("\tCompany Name\t: {0}", profile.company_name);
			Console.WriteLine("\tCulture\t: {0}", profile.culture);
			Console.WriteLine("\tLanguage\t: {0}", profile.language);
			Console.WriteLine("\tetag\t\t: {0}", profile.etag);

			Console.WriteLine("\nAddress Info");
			Console.WriteLine("\tFirstName\t: {0}", profile.default_address.first_name);
			Console.WriteLine("\tLastName\t: {0}", profile.default_address.last_name);
			Console.WriteLine("\tAddressLine1\t: {0}", profile.default_address.address_line1);
			Console.WriteLine("\tAddressLine2\t: {0}", profile.default_address.address_line1);
			Console.WriteLine("\tCity\t\t: {0}", profile.default_address.city);
			Console.WriteLine("\tRegion\t: {0}", profile.default_address.region);
			Console.WriteLine("\tZipCode\t: {0}", profile.default_address.postal_code);
			Console.WriteLine("\tCountry\t: {0}", profile.default_address.country);
		}
	}
}