/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Helpers;

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
	internal class AzureResourceManager
	{
		/// <summary>
		/// Registers the given ResourceProvider with the given subscription
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="providerName">Resource Provider</param>
		/// <param name="azureToken">Authorization Token<</param>
		/// <param name="correlationId">Correlation id for the scenario</param>
		/// <returns></returns>
		public static dynamic RegisterProvider(string subscriptionId, string providerName, string azureToken,
				string correlationId)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(
									string.Format(
											"https://management.azure.com/subscriptions/{0}/providers/{1}/register?api-version=2015-01-01",
											subscriptionId, providerName));

			request.Method = "POST";
			request.Accept = "application/json";
			request.ContentLength = 0;

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

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
		/// Creates a resource group in the specified subscription.
		/// </summary>
		/// <param name="subscriptionId">The subscriptionId for the Azure user</param>
		/// <param name="resourceGroupName">Name to be assigned to the resource group</param>
		/// <param name="azureToken">Authorization Token</param>
		/// <param name="correlationId">Correlation id for the scenario</param>
		/// <returns>HttpWebResponse object if the call succeeds. null if there is an exception.</returns>
		public static dynamic CreateResourceGroup(string subscriptionId, string resourceGroupName, string azureToken,
				string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourcegroups/{1}?api-version=2015-01-01",
					subscriptionId, resourceGroupName));

			request.Method = "PUT";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);
			string content = Json.Encode(AzureRequestBuilder.CreateResourceGroupRequestData());

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
				}
				return response;
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
			return null;
		}

		/// <summary>
		/// Creates a new storage account with the specified parameters.
		/// </summary>
		/// <param name="subscriptionId">The subscriptionId for the Azure user</param>
		/// <param name="resGroupName">The name of the resource group within the user’s subscription</param>
		/// <param name="accountName">The name of the storage account to be created</param>
		/// <param name="azureToken">Azure Authorization token</param>
		/// <param name="correlationId">Correlation Id for the scenario</param>
		/// <returns>HttpWebResponse object if the call succeeds. null if there is an exception.</returns>
		public static dynamic CreateStorageAccount(string subscriptionId, string resGroupName, string accountName,
				string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Storage/storageAccounts/{2}?&api-version=2015-05-01-preview",
					subscriptionId, resGroupName, accountName));

			request.Method = "PUT";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			string content = Json.Encode(AzureRequestBuilder.CreateStorageAccountRequestData());

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
				}
				return response;
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
			return null;
		}

		/// <summary>
		/// Creates a new network security group
		/// </summary>
		/// <param name="subscriptionId">The subscriptionId for the Azure user</param>
		/// <param name="resourceGroupName">The name of the resource group within the user’s subscription</param>
		/// <param name="networkSecurityGroupName">The name to be given to the network security group</param>
		/// <param name="azureToken">Azure Authorization token</param>
		/// <param name="correlationId">Correlation Id for the scenario</param>
		/// <returns>HttpWebResponse object if the call succeeds. null if there is an exception.</returns>
		public static dynamic CreateNetworkSecurityGroup(string subscriptionId, string resourceGroupName,
				string networkSecurityGroupName, string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.network/networkSecurityGroups/{2}?api-version=2015-05-01-preview",
					subscriptionId, resourceGroupName, networkSecurityGroupName));

			request.Method = "PUT";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			string content =
					Json.Encode(AzureRequestBuilder.CreateNetworkSecurityGroupRequestData(networkSecurityGroupName));

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
				}
				return response;
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
			return null;
		}

		/// <summary>
		/// Creates a new virtual network 
		/// </summary>
		/// <param name="subscriptionId">The subscriptionId for the Azure user</param>
		/// <param name="resourceGroupName">The name of the resource group within the user’s subscription</param>
		/// <param name="virtualNetworkName">The name to be given to the virtual network</param>
		/// <param name="azureToken">Azure Authorization token</param>
		/// <param name="correlationId">Correlation Id for the scenario</param>
		/// <returns>HttpWebResponse object if the call succeeds. null if there is an exception.</returns>
		public static dynamic CreateVirtualNetwork(string subscriptionId, string resourceGroupName,
				string virtualNetworkName, string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.network/virtualNetworks/{2}?api-version=2015-05-01-preview",
					subscriptionId, resourceGroupName, virtualNetworkName));

			request.Method = "PUT";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			string content = Json.Encode(AzureRequestBuilder.CreateVirtualNetworkRequestData());

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
				}
				return response;
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
			return null;
		}


		/// <summary>
		/// Creates an availability set 
		/// </summary>
		/// <param name="subscriptionId">The subscriptionId for the Azure user</param>
		/// <param name="resourceGroupName">The name of the resource group within the user’s subscription</param>
		/// <param name="virtualNetworkName">The name of the virtual network where the subnet is to be created</param>
		/// <param name="networkSecurityGroupId">network security group id</param>
		/// <param name="subNetName">The name to be given to the new subnet</param>
		/// <param name="azureToken">Azure Authorization token</param>
		/// <param name="correlationId">Correlation Id for the scenario</param>
		/// <returns>HttpWebResponse object if the call succeeds. null if there is an exception.</returns>
		public static dynamic CreateSubNet(string subscriptionId, string resourceGroupName, string virtualNetworkName,
				string networkSecurityGroupId, string subNetName, string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.network/virtualNetworks/{2}/subnets/{3}?api-version=2015-05-01-preview",
					subscriptionId, resourceGroupName, virtualNetworkName, subNetName));

			request.Method = "PUT";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			string content = Json.Encode(AzureRequestBuilder.CreateSubNetRequestData(networkSecurityGroupId));

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
				}
				return response;
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
			return null;
		}

		/// <summary>
		/// Creates an availability set 
		/// </summary>
		/// <param name="subscriptionId">The subscriptionId for the Azure user</param>
		/// <param name="resourceGroupName">The name of the resource group within the user’s subscription</param>
		/// <param name="publicIpName">The name to be given to the new public ip</param>
		/// <param name="azureToken">Azure Authorization token</param>
		/// <param name="correlationId">Correlation Id for the scenario</param>
		/// <returns>HttpWebResponse object if the call succeeds. null if there is an exception.</returns>
		public static dynamic CreatePublicIpAddress(string subscriptionId, string resourceGroupName, string publicIpName,
				string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.network/publicIPAddresses/{2}?api-version=2015-05-01-preview",
					subscriptionId, resourceGroupName, publicIpName));

			request.Method = "PUT";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			string content = Json.Encode(AzureRequestBuilder.CreatePublicIpRequestData());

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
				}
				int statusCode = (int)((HttpWebResponse)response).StatusCode;
				while (statusCode == 202)
				{
					Thread.Sleep(5000);
					statusCode = GetPublicIpStatus(subscriptionId, resourceGroupName, publicIpName, azureToken,
							correlationId);
				}
				return response;
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
			return null;
		}

		/// <summary>
		/// Creates a network interface card
		/// </summary>
		/// <param name="subscriptionId">The subscriptionId for the Azure user</param>
		/// <param name="resourceGroupName">The name of the resource group within the user’s subscription</param>
		/// <param name="networkInterfaceName">The name to be given to the new network interface</param>
		/// <param name="networkSecurityGroupId">network security group id</param>
		/// <param name="ipName">User-defined name of the IP</param>
		/// <param name="publicIp">Public IP Address</param>
		/// <param name="subNetName">Sub Net Name</param>
		/// <param name="azureToken">Azure Authorization token</param>
		/// <param name="correlationId">Correlation Id for the scenario</param>
		/// <returns>HttpWebResponse object if the call succeeds. null if there is an exception.</returns>
		public static dynamic CreateNetworkInterface(string subscriptionId, string resourceGroupName,
				string networkInterfaceName, string networkSecurityGroupId, string ipName, string publicIp,
				string subNetName, string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.network/networkInterfaces/{2}?api-version=2015-05-01-preview",
					subscriptionId, resourceGroupName, networkInterfaceName));

			request.Method = "PUT";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			string content =
					Json.Encode(AzureRequestBuilder.CreateNetworkInterfaceRequestData(networkSecurityGroupId, ipName,
							subNetName, publicIp));

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
				}
				return response;
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
			return null;
		}

		/// <summary>
		/// Creates a virtual machine in the specified subscription
		/// </summary>
		/// <param name="subscriptionId">The subscriptionId for the Azure user</param>
		/// <param name="resourceGroupName">The name of the resource group within the user’s subscription</param>
		/// /// <param name="networkInterfaceId">The network interface associated with the virtual machine</param>
		/// <param name="storageAccount">The storage account that the virtual machine should be assigned to</param>
		/// <param name="vmName">The name to be given to the new VM</param>
		/// <param name="azureToken">Azure Authorization token</param>
		/// <param name="correlationId">Correlation Id for the scenario</param>
		/// <returns>JSON object representing the VM</returns>
		public static dynamic CreateVirtualMachine(string subscriptionId, string resourceGroupName,
				string networkInterfaceId, string storageAccount, string vmName, string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}?validating=true&api-version=2015-05-01-preview",
					subscriptionId, resourceGroupName, vmName));

			request.Method = "PUT";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			string id =
					String.Format("/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}",
							subscriptionId, resourceGroupName, vmName);
			string storageUrl = String.Format("http://{0}.blob.core.windows.net/vhds/sampleosdisk.vhd", storageAccount);
			string content =
					Json.Encode(AzureRequestBuilder.CreateVirtualMachineRequestData(id, vmName, storageUrl,
							networkInterfaceId));

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
		/// Query provider registration status looking for a state of 'Registered'
		/// </summary>
		/// <param name="subscriptionId">The subscriptionId for the Azure user</param>
		/// <param name="providerNamespace">provider namespace to query</param>
		/// <param name="retryAfter">seconds to wait between calss</param>
		/// <param name="azureToken">Azure Authorization token</param>
		/// <param name="correlationId">Correlation Id for the scenario</param>
		public static void WaitForProviderRegistration(string subscriptionId, string providerNamespace,
																int retryAfter, string azureToken, string correlationId)
		{
			bool registered = false;
			do
			{
				Thread.Sleep(retryAfter * 1000);
				var registrationResult = GetProviderRegistrationState(subscriptionId, providerNamespace, azureToken, correlationId);
				registered = (registrationResult.registrationState == "Registered");
			} while (registered == false);
		}

		/// <summary>
		/// Waiting for the resource group to be created, if it is in provisioning state.
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the resource group</param>
		/// <param name="retryAfter">The delay (in seconds) that the client should use when checking for the status of the operation</param>
		/// <param name="azureToken">Azure authentication token</param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns></returns>
		public static void WaitForResourceGroupProvisioning(string subscriptionId, string resourceGroupName,
				int retryAfter, string azureToken, string correlationId)
		{
			int statusCode = 0;
			do
			{
				Thread.Sleep(retryAfter * 1000);
				statusCode = GetResourceGroupStatus(subscriptionId, resourceGroupName, azureToken, correlationId);
			} while (statusCode != (int)HttpStatusCode.OK);
		}

		/// <summary>
		/// Waiting for the StorageAccount to be created, if it is in provisioning state.
		/// </summary>
		/// <param name="locationHeaderUrl">The URL where the status of the long-running operation can be checked</param>
		/// <param name="resourceGroupName">Name of the resource group</param>
		/// <param name="retryAfter">The delay (in seconds) that the client should use when checking for the status of the operation</param>
		/// <param name="azureToken">Azure authentication token</param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns></returns>
		public static void WaitForStorageAccountProvisioning(string locationHeaderUrl, string resourceGroupName,
				int retryAfter, string azureToken, string correlationId)
		{
			int statusCode = 0;
			do
			{
				Thread.Sleep(retryAfter * 1000);
				var response = PollAsyncStorageOperation(locationHeaderUrl, azureToken, correlationId);
				statusCode = (int)((HttpWebResponse)response).StatusCode;
				if (statusCode == (int)HttpStatusCode.Accepted)
				{
					locationHeaderUrl = response.Headers.Get("Location");
					retryAfter = Int32.Parse(response.Headers.Get("Retry-After"));
				}
			} while (statusCode == (int)HttpStatusCode.Accepted);
		}

		/// <summary>
		/// Waiting for the Network Security Group to be created, if it is in provisioning state.
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the resource group</param>
		/// <param name="networkSecurityGroupName">the name of the network Security Group</param>
		/// <param name="retryAfter">The delay (in seconds) that the client should use when checking for the status of the operation</param>
		/// <param name="azureToken">Azure authentication token</param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns></returns>
		public static void WaitForNetworkSecurityGroupProvisioning(string subscriptionId, string resourceGroupName,
				string networkSecurityGroupName, int retryAfter, string azureToken, string correlationId)
		{
			int statusCode = 0;
			do
			{
				Thread.Sleep(retryAfter * 1000);
				statusCode = GetNetworkSecurityGroupStatus(subscriptionId, resourceGroupName, networkSecurityGroupName,
						azureToken, correlationId);
			} while (statusCode != (int)HttpStatusCode.OK);
		}

		/// <summary>
		/// Waiting for the Virtual Network to be created, if it is in provisioning state.
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the resource group</param>
		/// <param name="virtualNetworkName">the name of the virtual Network</param>
		/// <param name="retryAfter">The delay (in seconds) that the client should use when checking for the status of the operation</param>
		/// <param name="azureToken">Azure authentication token</param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns></returns>
		public static void WaitForVirtualNetworkProvisioning(string subscriptionId, string resourceGroupName,
				string virtualNetworkName, int retryAfter, string azureToken, string correlationId)
		{
			int statusCode = 0;
			do
			{
				Thread.Sleep(retryAfter * 1000);
				statusCode = GetVirtualNetworkStatus(subscriptionId, resourceGroupName, virtualNetworkName, azureToken,
						correlationId);
			} while (statusCode != (int)HttpStatusCode.OK);
		}

		/// <summary>
		/// Waiting for the Sub Net to be created, if it is in provisioning state.
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the resource group</param>
		/// <param name="virtualNetworkName">the name of the virtual Network</param>
		/// <param name="subNetName">Name of the sub net</param>
		/// <param name="retryAfter">The delay (in seconds) that the client should use when checking for the status of the operation</param>
		/// <param name="azureToken">Azure authentication token</param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns></returns>
		public static void WaitForSubNetProvisioning(string subscriptionId, string resourceGroupName,
				string virtualNetworkName, string subNetName, int retryAfter, string azureToken, string correlationId)
		{
			int statusCode = 0;
			do
			{
				Thread.Sleep(retryAfter * 1000);
				statusCode = GetSubNetStatus(subscriptionId, resourceGroupName, virtualNetworkName, subNetName,
						azureToken, correlationId);
			} while (statusCode != (int)HttpStatusCode.OK);
		}

		/// <summary>
		/// Waiting for the public Ip Address  to be created, if it is in provisioning state.
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the resource group</param>
		/// <param name="publicIpAddress">the name of the publicIpAddress</param>
		/// <param name="retryAfter">The delay (in seconds) that the client should use when checking for the status of the operation</param>
		/// <param name="azureToken">Azure authentication token</param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns></returns>
		public static void WaitForPublicIpProvisioning(string subscriptionId, string resourceGroupName,
				string publicIpAddress, int retryAfter, string azureToken, string correlationId)
		{
			int statusCode = 0;
			do
			{
				Thread.Sleep(retryAfter * 1000);
				statusCode = GetPublicIpStatus(subscriptionId, resourceGroupName, publicIpAddress, azureToken,
						correlationId);
			} while (statusCode != (int)HttpStatusCode.OK);
		}

		/// <summary>
		/// Waiting for the Network Interface to be created, if it is in provisioning state.
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the resource group</param>
		/// <param name="networkInterface">the name of the Network Interface</param>
		/// <param name="retryAfter">The delay (in seconds) that the client should use when checking for the status of the operation</param>
		/// <param name="azureToken">Azure authentication token</param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns></returns>
		public static void WaitForNetworkInterfaceProvisioning(string subscriptionId, string resourceGroupName,
				string networkInterface, int retryAfter, string azureToken, string correlationId)
		{
			int statusCode = 0;
			do
			{
				Thread.Sleep(retryAfter * 1000);
				statusCode = GetNetworkInterfaceStatus(subscriptionId, resourceGroupName, networkInterface, azureToken,
						correlationId);
			} while (statusCode != (int)HttpStatusCode.OK);
		}

		/// <summary>
		/// Get registration state of provider
		/// </summary>
		/// <param name="subscriptionId">The subscriptionId for the Azure user</param>
		/// <param name="providerNamespace">provider namespace to query</param>
		/// <param name="azureToken">Azure Authorization token</param>
		/// <param name="correlationId">Correlation Id for the scenario</param>
		/// <returns></returns>
		public static dynamic GetProviderRegistrationState(string subscriptionId, string providerNamespace, 
			string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(

			"https://management.azure.com/subscriptions/{0}/providers/{1}?api-version=2015-01-01",
			subscriptionId, providerNamespace));

			request.Method = "GET";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

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
		/// Gets the Status of a ResourceGroup
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the Resource Group</param>
		/// <param name="azureToken">Authorization Token</param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns>Status</returns>
		public static dynamic GetResourceGroupStatus(string subscriptionId, string resourceGroupName, string azureToken,
				string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourcegroups/{1}?api-version=2015-01-01",
					subscriptionId, resourceGroupName));

			request.Method = "GET";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			try
			{
				Utilities.PrintWebRequest(request, string.Empty);

				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
				}
				return (int)((HttpWebResponse)response).StatusCode;
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
		/// Polls to check if the asynchronous operation of creating storage account is completed.
		/// For asynchronous operations such as Create Storage Account, the Storage Resource Provider will return an Accepted HTTP Status code (HTTP 202), an empty response,
		/// and a Location header to poll for the operation status. Users should poll at the Location header URL until the service returns success or an expected failure
		/// </summary>
		/// <param name="locationHeaderUrl">Url to poll fpr operation status</param>
		/// <param name="azureToken">Authorization Token</param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns></returns>
		public static dynamic PollAsyncStorageOperation(string locationHeaderUrl, string azureToken,
				string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(locationHeaderUrl);

			request.Method = "GET";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			try
			{
				Utilities.PrintWebRequest(request, string.Empty);

				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
				}
				return response;
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
		/// Gets the Status of a NetworkSecurityGroup
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the Resource Group</param>
		/// <param name="nsgName">Name of the Network Security Group</param>
		/// <param name="azureToken">Authorization Token"></param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns>Status</returns>
		public static dynamic GetNetworkSecurityGroupStatus(string subscriptionId, string resourceGroupName,
				string nsgName, string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourcegroups/{1}/providers/microsoft.network/networkSecurityGroups/{2}?api-version=2015-01-01",
					subscriptionId, resourceGroupName, nsgName));

			request.Method = "GET";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			try
			{
				Utilities.PrintWebRequest(request, string.Empty);

				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
				}
				return (int)((HttpWebResponse)response).StatusCode;
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
		/// Gets the Status of a Virtual Network
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the Resource Group</param>
		/// <param name="virtualNetworkName">Name of the Virtual Network </param>
		/// <param name="azureToken">Authorization Token"></param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns>Status</returns>
		public static dynamic GetVirtualNetworkStatus(string subscriptionId, string resourceGroupName,
				string virtualNetworkName, string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourcegroups/{1}/providers/microsoft.network/virtualNetworks/{2}?api-version=2015-01-01",
					subscriptionId, resourceGroupName, virtualNetworkName));

			request.Method = "GET";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			try
			{
				Utilities.PrintWebRequest(request, string.Empty);

				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
				}
				return (int)((HttpWebResponse)response).StatusCode;
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
		/// Gets the Status of a Sub Net
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the Resource Group</param>
		/// <param name="virtualNetworkName">Name of the Virtual Network </param>
		/// <param name="subNetName">Name of the Sub Net</param>
		/// <param name="azureToken">Authorization Token"></param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns>Status</returns>
		public static dynamic GetSubNetStatus(string subscriptionId, string resourceGroupName, string virtualNetworkName,
				string subNetName, string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourcegroups/{1}/providers/microsoft.network/virtualNetworks/{2}/subnets/{3}?api-version=2015-01-01",
					subscriptionId, resourceGroupName, virtualNetworkName, subNetName));

			request.Method = "GET";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			try
			{
				Utilities.PrintWebRequest(request, string.Empty);

				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
				}
				return (int)((HttpWebResponse)response).StatusCode;
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
		/// Gets the Status of a Public Ip
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the Resource Group</param>
		/// <param name="publicIpName">Name of the PublicIp</param>
		/// <param name="azureToken">Authorization Token"></param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns>Status</returns>
		public static dynamic GetPublicIpStatus(string subscriptionId, string resourceGroupName, string publicIpName,
				string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourcegroups/{1}/providers/microsoft.network/publicIPAddresses/{2}?api-version=2015-01-01",
					subscriptionId, resourceGroupName, publicIpName));

			request.Method = "GET";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			try
			{
				Utilities.PrintWebRequest(request, string.Empty);

				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
				}
				return (int)((HttpWebResponse)response).StatusCode;
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
		/// Gets the Status of a Network Interface
		/// </summary>
		/// <param name="subscriptionId">Subscription Id</param>
		/// <param name="resourceGroupName">Name of the Resource Group</param>
		/// <param name="networkInterfaceName">Name of the Network Interface</param>
		/// <param name="azureToken">Authorization Token"></param>
		/// <param name="correlationId">correlation id for the scenario</param>
		/// <returns>Status</returns>
		public static dynamic GetNetworkInterfaceStatus(string subscriptionId, string resourceGroupName,
				string networkInterfaceName, string azureToken, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
					"https://management.azure.com/subscriptions/{0}/resourcegroups/{1}/providers/microsoft.network/networkInterfaces/{2}?api-version=2015-01-01",
					subscriptionId, resourceGroupName, networkInterfaceName));

			request.Method = "GET";
			request.Accept = "application/json";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			try
			{
				Utilities.PrintWebRequest(request, string.Empty);

				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
				}
				return (int)((HttpWebResponse)response).StatusCode;
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
		/// Creates a new User
		/// </summary>
		/// <param name="azureToken">Azure Authorization Token</param>
		/// <returns>the new user object</returns>
		public static dynamic CreateUser(string displayName, string mailNickName, string userPrincipalName, 
																		 string password, string azureToken, string correlationId)
		{

			var request = (HttpWebRequest)HttpWebRequest.Create("https://graph.windows.net/myorganization/users?api-version=2013-11-08");

			request.Method = "POST";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			string content = Json.Encode(AzureRequestBuilder.CreateUserRequestData(displayName, mailNickName,
																																						 userPrincipalName,password));

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
					var newUser = Json.Decode(responseContent);
					return newUser;
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
		/// Creates a Role Assignment to a user for the given scope
		/// </summary>
		/// <param name="azureToken">Azure Authorization Token</param>
		/// <param name="scope">scope for which the role assigment is made</param>
		/// <param name="userId">user id</param>
		/// <param name="roleId">role id</param>
		/// <returns></returns>
		public static dynamic CreateRoleAssignment(string azureToken, string scope, string userId, string roleId, string correlationId)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
				"https://management.azure.com{0}/providers/Microsoft.Authorization/roleAssignments/{1}?api-version=2015-07-01",
				scope, roleId));

			request.Method = "PUT";
			request.ContentType = "application/json";

			request.Headers.Add("x-ms-correlation-id", correlationId);
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + azureToken);

			string roleDefinitionId = String.Format("{0}/providers/Microsoft.Authorization/roleDefinitions/{1}", scope, roleId);
			string content = "{" +
														"\"properties\": {" +
																					 "\"roleDefinitionId\": \"" + roleDefinitionId + "\"," +
																					 "\"principalId\": \"" + userId + "\"," +
																					 "\"scope\": \"" + scope + "\"" +

															"}" +
												"}";


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
					var newUser = Json.Decode(responseContent);
					return newUser;
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

	}
}