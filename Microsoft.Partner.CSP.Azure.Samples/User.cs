using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Microsoft.Partner.CSP.Azure.Samples
{
    class User
    {
        /// <summary>
        /// Creates a new User
        /// </summary>
        /// <param name="azureToken">Azure Authorization Token</param>
        /// <returns>the new user object</returns>
        public static dynamic CreateUser(string azureToken, string correlationId)
        {
            // Taking input from user
            Console.WriteLine("Enter a display name for New User:");
            string displayName = Console.ReadLine().Trim();
            Console.WriteLine("Enter a mail nick name:");
            string mailNickName = Console.ReadLine().Trim();
            Console.WriteLine("Enter a password:");
            string password = Console.ReadLine().Trim();
            Console.WriteLine("Enter a userPrincipalName:");
            string userPrincipalName = Console.ReadLine().Trim();

            var request = (HttpWebRequest)HttpWebRequest.Create("https://graph.windows.net/myorganization/users?api-version=2013-11-08");

            request.Method = "POST";
            request.ContentType = "application/json";

            request.Headers.Add("x-ms-correlation-id", correlationId);
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + azureToken);

            string content = "{\"accountEnabled\": true," +
                             "\"displayName\": \"" + displayName + "\"," +
                             "\"mailNickname\": \"" + mailNickName + "\"," +
                             "\"passwordProfile\": {\"password\":\"" + password + "\", \"forceChangePasswordNextLogin\": false}," +
                             "\"userPrincipalName\": \"" + userPrincipalName + "\"}";

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
        /// <param name="scopeId">scope for which the role assigment is made</param>
        /// <param name="userId">user id</param>
        /// <param name="roleId">role id</param>
        /// <returns></returns>
        public static dynamic CreateRoleAssignment(string azureToken, string scopeId, string userId, string roleId, string correlationId)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
              "https://management.azure.com{0}providers/Microsoft.Authorization/roleAssignments/{1}?api-version=2015-05-01-preview",
              scopeId, roleId));

            request.Method = "PUT";
            request.ContentType = "application/json";

            request.Headers.Add("x-ms-correlation-id", correlationId);
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + azureToken);

            string roleDefinitionId = String.Format("{0}providers/Microsoft.Authorization/roleDefinitions/{1}", scopeId, roleId);
            string content = "{" +
                                  "\"properties\": {" +
                                                 "\"roleDefinitionId\": \"" + roleDefinitionId + "\"," +
                                                 "\"principalId\": \"" + userId + "\"," +
                                                 "\"scope\": \"" + scopeId + "\"" +
                                  
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
