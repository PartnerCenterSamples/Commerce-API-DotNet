using System.Collections.Generic;

namespace Microsoft.Partner.CSP.Azure.Samples
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
                        osDisk = new
                        {
                            name = "sampleosdisk",
                            osType = "windows",
                            vhd = new
                            {
                                uri = storageUrl
                            },
                            createOption = "Attach"
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
    }
}