/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

using System;

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
	class User
	{
		/// <summary>
		/// Read user information from console
		/// </summary>
		/// <returns>object describing user to be created</returns>
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
