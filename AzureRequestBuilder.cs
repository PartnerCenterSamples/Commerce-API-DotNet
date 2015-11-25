using System.Collections.Generic;

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
	internal class AzureRequestBuilder
	{
		/// <summary>
		///  Builds the request body for CreateResourceGroup request, as a dynamic object
		/// </summary>
		/// <returns>object</returns>
		public static dynamic CreateResourceGroupRequestData()
		{
			return new
			{
				location = "East US"
			};
		}

		/// <summary>
		/// Builds the request body for CreateStorageAccount request, as a dynamic object
		/// </summary>
		/// <returns>object</returns>
		public static dynamic CreateStorageAccountRequestData()
		{
			return new
			{
				location = "East US",
				properties = new
				{
					// Account types - Standard_LRS|Standard_ZRS|Standard_GRS|Standard_RAGRS|Premium_LRS
					accountType = "Standard_LRS"
				}
			};
		}

		/// <summary>
		/// Builds the request body for CreateNetworkSecurityGroup request, as a dynamic object
		/// </summary>
		/// <param name="networkSecurityGroupName">The name of the network security group</param>
		/// <returns>object</returns>
		public static dynamic CreateNetworkSecurityGroupRequestData(string networkSecurityGroupName)
		{
			return new
			{
				location = "East US",
				properties = new
				{
					securityRules = new List<dynamic>
					{
						new
						{
							name = networkSecurityGroupName,
							properties = new
							{
								description = "Sample Rule",
								protocol = "Tcp",
								sourcePortRange = 0,
								destinationPortRange = 3389,
								sourceAddressPrefix = "*",
								destinationAddressPrefix = "*",
								access = "Allow",
								priority = 1000,
								direction = "Inbound"
							}
						}
					}
				}
			};
		}

		/// <summary>
		/// Builds the request body for CreateVirtualNetwork request, as a dynamic object
		/// </summary>
		/// <returns>object</returns>
		public static dynamic CreateVirtualNetworkRequestData()
		{
			return new
			{
				location = "East US",
				properties = new
				{
					addressSpace = new
					{
						addressPrefixes = new List<dynamic>
						{
							"10.1.0.0/16",
							"10.2.0.0/16"
						}
					}
				}
			};
		}

		/// <summary>
		/// Builds the request body for CreateSubNet request, as a dynamic object
		/// </summary>
		/// <param name="networkSecurityGroupId">network security group id</param>
		/// <returns>object</returns>
		public static dynamic CreateSubNetRequestData(string networkSecurityGroupId)
		{
			return new
			{
				properties = new
				{
					addressPrefix = "10.1.0.0/24",
					networkSecurityGroup = new
					{
						id = networkSecurityGroupId
					}
				}
			};
		}

		/// <summary>
		/// Builds the request body for CreatePublicIp request, as a dynamic object
		/// </summary>
		/// <returns>object</returns>
		public static dynamic CreatePublicIpRequestData()
		{
			return new
			{
				location = "East US",
				properties = new
				{
					publicIPAllocationMethod = "Dynamic",
					idleTimeoutInMinutes = 4
				}
			};
		}

		/// <summary>
		/// Builds the request body for CreateNetworkInterface request, as a dynamic object
		/// </summary>
		/// <param name="networkSecurityGroupId">network security group id</param>
		/// <param name="ipName">User-defined name of the IP</param>
		/// <param name="subNetName">name of the subnet</param>
		/// <param name="publicIp">public ip address</param>
		/// <returns>object</returns>
		public static dynamic CreateNetworkInterfaceRequestData(string networkSecurityGroupId, string ipName,
				string subNetName, string publicIp)
		{
			return new
			{
				location = "East US",
				properties = new
				{
					networkSecurityGroup = new
					{
						id = networkSecurityGroupId
					},
					ipConfigurations = new List<dynamic>
					{
						new
						{
							name = ipName,
							properties = new
							{
								subnet = new
								{
									id = subNetName
								},
								privateIPAllocationMethod = "Dynamic",
								publicIPAddress = new
								{
									id = publicIp
								}
							}
						}
					}
				}
			};
		}

		/// <summary>
		/// Builds the request body for CreateVirtualMachine request, as a dynamic object
		/// </summary>
		/// <param name="id">Virtual Machine id</param>
		/// <param name="virtualMachineName">name of the virtual machine</param>
		/// <param name="storageUrl">storage url</param>
		/// <param name="networkInterfaceId">network interface id</param>
		/// <returns>object</returns>
		public static dynamic CreateVirtualMachineRequestData(string id, string virtualMachineName, string storageUrl,
				string networkInterfaceId)
		{
			return new
			{
				id = id,
				name = virtualMachineName,
				type = "Microsoft.Compute/virtualMachines",
				location = "eastus",
				properties = new
				{
					hardwareProfile = new
					{
						vmSize = "Standard_A0"
					},
					storageProfile = new
					{
						imageReference = new
						{
							publisher = "MicrosoftWindowsServer",
							offer = "WindowsServer",
							sku = "2012-R2-Datacenter",
							version = "latest"
						},
						osDisk = new
						{
							name = "sampleosdisk",
							vhd = new
							{
								uri = storageUrl
							},
							createOption = "FromImage"
						}
					},
					osProfile = new
					{
						computerName = "cspsample",
						adminUsername = "cspvmadmin",
						adminPassword = "CSPVM4dm!n",
						windowsConfiguration = new
						{
							provisionVMAgent = true,
							enableAutomaticUpdates = true
						}
					},
					networkProfile = new
					{
						networkInterfaces = new List<dynamic>
						{
							new
							{
								id = networkInterfaceId
							}
						}
					}
				}
			};
		}

		/// <summary>
		/// Builds the request body for CreateUser request, as a dynamic object
		/// </summary>
		/// <param name="displayName">Display name of user</param>
		/// <param name="mailNickName">Mail nickname of user</param>
		/// <param name="userPrincipalName">UPN for user. (Must be unique in directory)</param>
		/// <param name="password">Password to assign user. Must meet password complexity requirements</param>
		/// <returns>object</returns>
		public static dynamic CreateUserRequestData(string displayName, string mailNickName, 
																								string userPrincipalName,string password)
		{
			return new
			{
				accountEnabled = true,
				displayName =displayName,
				mailNickname = mailNickName,
				passwordProfile = new
				{
					password = password,
					forceChangePasswordNextLogin = false
				},
				userPrincipalName = userPrincipalName
			};
		}
	}
}