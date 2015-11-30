# Commerce-API-DotNet

These are samples in C# for using the commerce APIs for Microsoft Partner Center. 
These CREST APIs are documented at https://msdn.microsoft.com/en-us/library/partnercenter/dn974944.aspx

A public forum for discussing the APIs is available at 
https://social.msdn.microsoft.com/Forums/en-US/home?forum=partnercenterapi

## Prerequisites

Review and complete the steps detaild in the [prerequisite page](Documentation/Prerequisite.md).

The Create VM scenario will require user credentials at run-time. This user must be a member of the
Admin Agents group in the reseller directory. To allow unattended operation, the sample application 
uses the Windows Credential Manager application to securely store the account and password of this user.
It is recommended that a "service account" be created rather than storing an actual user credentials.
The [Credential Manager](Documentation/CredentialManager.md) page includes steps for creating a 
stored credential.

## App Configuration

The sample application has several configurable settings in the app.config file. Set the values as
appropriate for your tenant.

Configuration Key | Description
------------ | -------------
scenario | The scenario to execute. A value of zero will run all scenarios.
AppId | The id of the application registered in the Partner Center. This id is used to access the CREST APIs.
Key | The key of the application registered in the Partner Center. 
MicrosoftId | The Microsoft Id of the reseller. Used to access CREST APIs.
DefaultDomain | The default domain of the reseller in Microsoft Azure. (This is typically an "onmicrosoft.com" domain.)
ExistingCustomerMicrosoftId | The Microsoft Id of a customer associated with the reseller. This is used in scenarios that require a customer id, such as GetSubscriptions and GetOrders.
AzureAppId | The id of the multi-tenant application registered in the reseller directory that provisions resources in the customer tenant.
CredentialName | The value of "Internet or network address" in Credential Manager that contains the service account credentials.
