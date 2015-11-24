# Commerce-API-DotNet

These are samples in C# for using the commerce APIs for Microsoft Partner Center. These CREST APIs are documented at https://msdn.microsoft.com/en-us/library/partnercenter/dn974944.aspx

A public forum for discussing the APIs is available at https://social.msdn.microsoft.com/Forums/en-US/home?forum=partnercenterapi

Before you build these samples, please update the keys in App.config which represent your Organization Id and the Azure AD application for your integration sandbox account.

The samples were built with Visual Studio 2013. 

If you're using Visual Studio 2015, the sample may not compile because you're missing a reference to System.Web.Helpers. To fix this, use NuGet to download the latest version of Microsoft.AspNet.Mvc. (Go to the solution explorer, expand Microsoft.Partner.CSP.Api.Samples, right-click References, and choose "Manage NuGet packages...". Then search for "MVC" and download. When it's done, you'll see System.Web.Helpers in your References, and compiling should work.)
