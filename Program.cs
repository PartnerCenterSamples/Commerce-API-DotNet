/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
	using System;
	using System.Configuration;

	static class Program
	{

		static void Main()
		{
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

			string ExistingCustomerMicrosoftId = ConfigurationManager.AppSettings["ExistingCustomerMicrosoftId"];


			// This is the clientId for this application in Azure Active Directory
			// This is only available at the time you created a new app at
			// "https://manage.windowsazure.com/#Workspaces/ActiveDirectoryExtension/directory"

			//  Configure application 
			//		Multi-tenant
			//    Permission to Windows Azure Service Management API
			//		Pre-consent
			//  
			string azureAppId = ConfigurationManager.AppSettings["AzureAppId"];
			string AzureSubscriptionId = ConfigurationManager.AppSettings["AzureSubscriptionId"];
			string credentialName = ConfigurationManager.AppSettings["CredentialName"];



			// Prompts the user to edit the config parametres if its not already done.
			Utilities.ValidateConfiguration(microsoftId, defaultDomain, appId, key, ExistingCustomerMicrosoftId);

			// Gets the scenario from app config. If an invalid entry is found, prompts the user to enter a valid value.
			string scenario = ConfigurationManager.AppSettings["scenario"];
			int intScenario;
			bool isParsed = int.TryParse(scenario, out intScenario);
			if (!isParsed || intScenario < 0 || intScenario > 8)
			{
				Utilities.PromptForScenario();
			}

			try
			{
				if (intScenario == Constants.SCENARIO_ONE || intScenario == Constants.ALL_SCENARIOS)
				{
					var customer = Orchestrator.GetCustomer(defaultDomain, appId, key, ExistingCustomerMicrosoftId, microsoftId);
				}
				if (intScenario == Constants.SCENARIO_TWO || intScenario == Constants.ALL_SCENARIOS)
				{
					Orchestrator.GetSubscriptions(defaultDomain, appId, key, ExistingCustomerMicrosoftId, microsoftId);
				}
				if (intScenario == Constants.SCENARIO_THREE || intScenario == Constants.ALL_SCENARIOS)
				{
					Orchestrator.GetOrders(defaultDomain, appId, key, ExistingCustomerMicrosoftId, microsoftId);
				}
				if (intScenario == Constants.SCENARIO_FOUR || intScenario == Constants.ALL_SCENARIOS)
				{
					Orchestrator.CreateOrder(defaultDomain, appId, key, ExistingCustomerMicrosoftId, microsoftId);
				}
				if (intScenario == Constants.SCENARIO_FIVE || intScenario == Constants.ALL_SCENARIOS)
				{
					Orchestrator.GetRateCardAndUsage(defaultDomain, appId, key, ExistingCustomerMicrosoftId, microsoftId);
				}
				if (intScenario == Constants.SCENARIO_SIX || intScenario == Constants.ALL_SCENARIOS)
				{
					var subscriptionId = ConfigurationManager.AppSettings["Office365SubscriptionId"];
					if (string.IsNullOrWhiteSpace(subscriptionId) || subscriptionId.Equals("Office365 Subscription Id"))
						Utilities.PromptForSubscriptionId();
					Orchestrator.TransitionToNewSKU(subscriptionId, defaultDomain, appId, key, ExistingCustomerMicrosoftId, microsoftId);
				}
				if (intScenario == Constants.SCENARIO_SEVEN || intScenario == Constants.ALL_SCENARIOS)
				{
					Orchestrator.ListAndDeleteAllCustomers(defaultDomain, appId, key, microsoftId, ExistingCustomerMicrosoftId);
				}
				if (intScenario == Constants.SCENARIO_SEVEN || intScenario == Constants.ALL_SCENARIOS)
				{
					Orchestrator.CreateCustomer(defaultDomain, appId, key, microsoftId);
				}
				if (intScenario == Constants.SCENARIO_EIGHT || intScenario == Constants.ALL_SCENARIOS)
				{
					Orchestrator.CreateVirtualMachineInNewSubscription(defaultDomain, appId, key, microsoftId, azureAppId, credentialName);
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
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("\n An unrecoverable error occurred:");
				Console.WriteLine(ex.Message);
			}

			Console.ResetColor();
			Console.Write("\n\n\nHit enter to exit the app...");
			Console.ReadLine();
		}
	}
}
