# Partner Center Samples - Prerequisites

- Partner Center access

    Setting up the following items requires access to the Microsoft Partner Center.

  - Integration Sandbox account

        An account in the Integration Sandbox is recommended for developing applications 
that consume CREST APIs.

  - API access enabled

        Enabling API access includes registering an application. The application 
will be assigned an App ID. In addition, a key is created to use for identifying 
the application at runtime. This App ID and Key will be added to the configuration 
file of the sample application.

- Microsoft Azure subscription  

    Managing the Azure Active Directory (the reseller directory) that supports the Partner Center 
requires a Microsoft Azure subscription. An existing subscription can be linked to the reseller 
directory by performing the following steps:

  1. Sign in to the Management Portal using you Microsoft account.
  2. Click New > App services > Active Directory > Directory > Custom Create.
  3. Click Use existing directory and check I am ready to be signed out now and click the check mark 
to complete the action.
  4. Sign in to the Management Portal by using and account that is global admin the work or school 
directory.
  5. When prompted to Use the Contoso directory with Azure?, and click continue.
  6. Click Sign out now.
  7. Sign back in to the Management Portal using your Microsoft account. Both directories will appear 
in the Active Directory extension.

    If you do not have an Azure subscription, go to [http://aka.ms/accessaad](http://aka.ms/accessaad) to sign up for a $0 Azure 
subscription. The subscription cannot generate usage and therefore won't cost you anything, but it 
allows you to login to Azure Management Portal.

- Registered Azure AD application

    The application registered in the Partner Center is actually registed in the reseller's Azure AD.
In order to adminster customer tenants, an application must be registered in each of those tenants, or
a multi-tenant app can be registered in the reseller tenant. Register a multi-tenant app by performing 
the following steps in the Management Portal:

  1.	Select partner tenant directory.
  2.	Click APPLICATIONS (top navigation bar).  
  3.	Click ADD (bottom).
  4.	Select Add an application my organization is developing.
  5.	Enter a name for your application.
  6.	For Type, select NATIVE CLIENT APPLICATION.
  7.	Click Next.
  8.	Provide a Redirect URI. 
  9.	Click Confirm to create the Application object.
  10.	While your application is selected, Click CONFIGURE (top navigation bar).
  11.	Note down your CLIENT ID.
  12.	Scroll down to section permissions to other applications and click Add application.
  13.	In the pop up window, select Windows Azure Service Management API and click Complete.
  14.	Back to the section permissions to other applications, you will see a new entry 
Windows Azure Service Management API. Click on Delegated Permissions:0. In the dropdown box, 
select Access Azure Service Management (preview).
  15.	Click SAVE (bottom).

    Granting consent for this application to update resources in customer tenants will be required
when the application first runs in each tenant. To ease the administrative burden, the application
object can be configured for "pre-consent." [Navigate here to view the Pre-Consent instructions.](Documentation/PreConsent.md)
