using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Partner.CSP.Azure.Samples
{
	class Program
	{
		static void Main(string[] args)
		{
			// This is the Microsoft ID of the reseller
			// Please work with your Admin Agent to get it from https://partnercenter.microsoft.com/en-us/pc/AccountSettings/TenantProfile
			string microsoftId = ConfigurationManager.AppSettings["MicrosoftId"];

			// This is the default domain of the reseller
			// Please work with your Admin Agent to get it from https://partnercenter.microsoft.com/en-us/pc/AccountSettings/TenantProfile
			//string defaultDomain = ConfigurationManager.AppSettings["DefaultDomain"];


			// This is the clientId for this application in Azure Active Directory
			// This is only available at the time you created a new app at
			// "https://manage.windowsazure.com/#Workspaces/ActiveDirectoryExtension/directory"

			//  Configure application 
			//		Multi-tenant
			//    Permission to Windows Azure Service Management API
			//		Pre-consent
			//  
			string appId = ConfigurationManager.AppSettings["AppId"];

			string ExistingCustomerMicrosoftId = ConfigurationManager.AppSettings["ExistingCustomerMicrosoftId"];


			string credentialName = ConfigurationManager.AppSettings["CredentialName"];



			// Prompts the user to edit the config parametres if its not already done.
			Utilities.ValidateConfiguration(microsoftId, appId, ExistingCustomerMicrosoftId);

			try
			{
				var subscriptionId = ConfigurationManager.AppSettings["AzureSubscriptionId"];
				if (string.IsNullOrWhiteSpace(subscriptionId) || subscriptionId.Equals("Azure Subscription Id"))
					Utilities.PromptForSubscriptionId();
				CreateAzureVirtualMachine(appId, credentialName, ExistingCustomerMicrosoftId, subscriptionId);
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

		/// <summary>
		/// Get the token for authenticating requests to Azure Resource Manager.
		/// </summary>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">This is the key for this application in Azure Active Directory</param>
		/// <param name="customerTenantId">cid of the customer</param>
		/// <returns>Azure Auth Token</returns>
		private static string GetAzureAuthToken(string appId, string credentialName, string customerTenantId)
		{
			string token = String.Empty;
			AuthenticationResult authResult = null;
			string resource = "https://management.core.windows.net/";

			var authenticationContext = new AuthenticationContext("https://login.windows.net/" + customerTenantId);
			try
			{
				authResult = authenticationContext.AcquireTokenSilent(resource: resource, clientId: appId);
			}
			catch (AdalException aex)
			{
				if (aex.ErrorCode == "failed_to_acquire_token_silently")
				{
					UserCredential uc = CredentialManager.GetCredential(credentialName);
					authResult = authenticationContext.AcquireToken(resource: resource, clientId: appId, userCredential: uc);
					token = authResult.AccessToken;
				}
			}
			catch (Exception ex)
			{
				throw;
			}

			return token;
		}


		/// <summary>
		/// Creates a virtual machine in Azure. 
		/// Creates all the required resources before creating VM.
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="credentialName">Internet name/address used to identify entry in Credential Manager</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="customerTenantId">Id or Domain of the customer Azure tenant</param>
		public static void CreateAzureVirtualMachine(string appId, string credentialName, string customerTenantId, string subscriptionId)
		{
			// Get Azure Authentication Token
			string azureToken = GetAzureAuthToken(appId, credentialName, customerTenantId);
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

			// The following resources are to be created in order before creating a virtual machine.

			// #1 Create Resource Group
			var createResourceGroupResponse = AzureResourceManager.CreateResourceGroup(subscriptionId, resourceGroupName,
					azureToken, correlationId);
			if (createResourceGroupResponse == null)
			{
				return;
			}
			// Waiting for the resource group to be created, if the request is just Accepted and the creation is still pending.
			if ((int)(createResourceGroupResponse.StatusCode) == (int)HttpStatusCode.Accepted)
			{
				AzureResourceManager.WaitForResourceGroupProvisioning(subscriptionId, resourceGroupName, retryAfter,
						azureToken, correlationId);
			}

			// #2 Create Storage Account
			// Register the subscription with Storage Resource Provider, for creating Storage Account
			// Storage Resource Provider
			const string storageProviderName = "Microsoft.Storage";
			AzureResourceManager.RegisterProvider(subscriptionId, storageProviderName, azureToken, correlationId);
			var storageAccountResponse = AzureResourceManager.CreateStorageAccount(subscriptionId, resourceGroupName,
					storageAccountName, azureToken, correlationId);
			if (storageAccountResponse == null)
			{
				return;
			}
			// Waiting for the storage account to be created, if the request is just Accepted and the creation is still pending.
			if ((int)(storageAccountResponse.StatusCode) == (int)HttpStatusCode.Accepted)
			{
				var location = storageAccountResponse.Headers.Get("Location");
				retryAfter = Int32.Parse(storageAccountResponse.Headers.Get("Retry-After"));
				AzureResourceManager.WaitForStorageAccountProvisioning(location, resourceGroupName, retryAfter,
						azureToken, correlationId);
			}

			// Register the subscription with Network Resource Provider for creating Network Resources - Netowrk Securtiy Group, Virtual Network, Subnet, Public IP and Network Interface
			// Network Resource Provider
			const string networkProviderName = "Microsoft.Network";
			AzureResourceManager.RegisterProvider(subscriptionId, networkProviderName, azureToken, correlationId);

			// #3 Create Network Security Group
			var networkSecurityGroupResponse = AzureResourceManager.CreateNetworkSecurityGroup(subscriptionId,
					resourceGroupName, networkSecurityGroupName, azureToken, correlationId);
			if (networkSecurityGroupResponse == null)
			{
				return;
			}
			// Waiting for the network security group to be created, if the request is just Accepted and the creation is still pending.
			if ((int)(networkSecurityGroupResponse.StatusCode) == (int)HttpStatusCode.Accepted)
			{
				AzureResourceManager.WaitForNetworkSecurityGroupProvisioning(subscriptionId, resourceGroupName,
						networkSecurityGroupName, retryAfter, azureToken, correlationId);
			}

			// #4 Create Virtual Network
			var virtualNetworkResponse = AzureResourceManager.CreateVirtualNetwork(subscriptionId, resourceGroupName,
					virtualNetworkName, azureToken, correlationId);
			if (virtualNetworkResponse == null)
			{
				return;
			}
			// Waiting for the virtual network to be created, if the request is just Accepted and the creation is still pending.
			if ((int)(virtualNetworkResponse.StatusCode) == (int)HttpStatusCode.Accepted)
			{
				AzureResourceManager.WaitForVirtualNetworkProvisioning(subscriptionId, resourceGroupName,
						virtualNetworkName, retryAfter, azureToken, correlationId);
			}
			// #5 Create Subnet
			var subNetResponse = AzureResourceManager.CreateSubNet(subscriptionId, resourceGroupName, virtualNetworkName,
					networkSecurityGroupId, subnetName, azureToken, correlationId);
			if (subNetResponse == null)
			{
				return;
			}
			// Waiting for the subnet to be created, if the request is just Accepted and the creation is still pending.
			if ((int)(subNetResponse.StatusCode) == (int)HttpStatusCode.Accepted)
			{
				AzureResourceManager.WaitForSubNetProvisioning(subscriptionId, resourceGroupName, virtualNetworkName,
						subnetName, retryAfter, azureToken, correlationId);
			}
			// #6 Create Public IP Address
			var publicIpResponse = AzureResourceManager.CreatePublicIpAddress(subscriptionId, resourceGroupName,
					publicIpName, azureToken, correlationId);
			if (publicIpResponse == null)
			{
				return;
			}
			// Waiting for the public IP to be created, if the request is just Accepted and the creation is still pending.
			if ((int)(publicIpResponse.StatusCode) == (int)HttpStatusCode.Accepted)
			{
				AzureResourceManager.WaitForPublicIpProvisioning(subscriptionId, resourceGroupName, publicIpName,
						retryAfter, azureToken, correlationId);
			}
			// #7 Create Network Interface
			var networkInterfaceResponse = AzureResourceManager.CreateNetworkInterface(subscriptionId, resourceGroupName,
					networkInterfaceName, networkSecurityGroupId, ipName, publicIpId, subNetId, azureToken, correlationId);
			if (networkInterfaceResponse == null)
			{
				return;
			}
			// Waiting for the network interface to be created, if the request is just Accepted and the creation is still pending.
			if ((int)(networkInterfaceResponse.StatusCode) == (int)HttpStatusCode.Accepted)
			{
				AzureResourceManager.WaitForNetworkInterfaceProvisioning(subscriptionId, resourceGroupName,
						networkInterfaceName, retryAfter, azureToken, correlationId);
			}
			// #8 Create Azure Virtual Machine
			// Compute Resource Provider
			const string computeProviderName = "Microsoft.Compute";
			AzureResourceManager.RegisterProvider(subscriptionId, computeProviderName, azureToken, correlationId);
			var virtualMachineResponse = AzureResourceManager.CreateVirtualMachine(subscriptionId, resourceGroupName,
					networkInterfaceId, storageAccountName, vitualMachineName, azureToken, correlationId);

			// Create a New User
			var newUser = User.CreateUser(azureToken, correlationId);

			// Role Id for Role 'Owner'
			const string roleIdForOwner = "8e3af657-a8ff-443c-a75c-2fe8c4bcb635";
			var scope = String.Format("/subscriptions/{0}/", subscriptionId);
			// Assigning 'Owner' role to the new user 
			// This is not working now.
			var roleAssignment = User.CreateRoleAssignment(azureToken, scope, newUser.objectId, roleIdForOwner,
					correlationId);
		}
	}
}
