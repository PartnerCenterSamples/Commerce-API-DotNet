/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.Partner.CSP.Azure.Samples
{
	using System;
	using System.Net;

	public class Utilities
	{
		/// <summary>
		/// This method prints the web request to console
		/// </summary>
		/// <param name="request">request object</param>
		/// <param name="content">content in the request</param>
		public static void PrintWebRequest(HttpWebRequest request, string content)
		{
			Console.WriteLine("================================================================================");
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine("{0} {1} HTTP/{2}", request.Method, request.RequestUri, request.ProtocolVersion);
			foreach (var webHeaderName in request.Headers.AllKeys)
			{
				Console.WriteLine("{0}: {1}", webHeaderName, (webHeaderName == "Authorization" ? "<token suppressed>" : request.Headers[webHeaderName]));
			}

			Console.ForegroundColor = ConsoleColor.Gray;
			if (request.Method != "GET")
			{
				Console.WriteLine("\n{0}", content);
			}

			Console.ResetColor();
		}

		/// <summary>
		/// This method is for printing error responses
		/// </summary>
		/// <param name="response">response object</param>
		/// <param name="content">content in the response</param>
		public static void PrintErrorResponse(HttpWebResponse response, string content)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\n\nHTTP/{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription);
			foreach (var webHeaderName in response.Headers.AllKeys)
			{
				Console.WriteLine("{0}: {1}", webHeaderName, response.Headers[webHeaderName]);
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\n{0}", content);

			Console.ResetColor();
		}

		/// <summary>
		/// This method is for printing web responses
		/// </summary>
		/// <param name="response">response object</param>
		/// <param name="content">content in the web response</param>
		public static void PrintWebResponse(HttpWebResponse response, string content)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine("\n\nHTTP/{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription);
			foreach (var webHeaderName in response.Headers.AllKeys)
			{
				Console.WriteLine("{0}: {1}", webHeaderName, response.Headers[webHeaderName]);
			}

			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine("\n{0}", content);

			Console.ResetColor();
		}

		/// <summary>
		/// Prompts the user to edit the config paramerters in app.config
		/// </summary>
		public static void ValidateConfiguration(string microsoftId, string appId, string ExistingCustomerMicrosoftId)
		{
			if (microsoftId.Equals("Your Organization Microsoft Id") || 
					appId.Equals("Your App Id") || 
					ExistingCustomerMicrosoftId.Equals("Your existing customer ID"))
			{
				Console.Write(
						"\nHave you updated the app.config, with the settings from https://partnercenter.microsoft.com (y/n)? ");
				string response = Console.ReadLine().Trim().ToUpperInvariant();
				if (response != "Y" && response != "YES")
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine(
							"\nUpdate AppId, MicrosoftId, DefaultDomain in the app.config and run the app again");

					Console.ResetColor();
					Console.Write("\n\n\nHit enter to exit the app now");
					Console.ReadLine();
					return;
				}
			}
		}

		/// <summary>
		/// Prompts the user to enter a subscription id in app.config
		/// </summary>
		public static void PromptForSubscriptionId()
		{
			Console.Write("\nPlease enter the SubscriptionId in the app.config and run the app again.");
			Console.Write("\n\n\nHit enter to exit the app now.");
			Console.ReadLine();
			return;

		}
	}
}
