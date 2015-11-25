using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
		class User
		{
		public static dynamic PopulateUserFromConsole()
		{
			Console.Clear();
			Console.ResetColor();
			Console.WriteLine("=========================================");
			Console.WriteLine("Create New User");
			Console.WriteLine("=========================================");

			// Taking input from user
			Console.WriteLine("Enter a display name for New User:");
			string displayName = Console.ReadLine().Trim();
			Console.WriteLine("Enter a mail nick name:");
			string mailNickname = Console.ReadLine().Trim();
			Console.WriteLine("Enter a password:");
			string password = Console.ReadLine().Trim();
			Console.WriteLine("Enter a userPrincipalName:");
			string userPrincipalName = Console.ReadLine().Trim();

			return new
			{
				displayName = displayName,
				mailNickname = mailNickname,
				userPrincipalName = userPrincipalName,
				password = password
			};
		}
	}
}
