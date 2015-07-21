/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
    using System;
    using System.Net;

    class Utilities
    {
        /// <summary>
        /// This method prints the web request to console
        /// </summary>
        /// <param name="request">request object</param>
        /// <param name="content">content in the request</param>
        public static void PrintWebRequest(HttpWebRequest request, string content)
        {
            Console.WriteLine("================================================================================");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("{0} {1} HTTP/{2}", request.Method, request.RequestUri, request.ProtocolVersion);
            foreach (var webHeaderName in request.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", webHeaderName, request.Headers[webHeaderName]);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            if (request.Method != "GET")
            {
                Console.WriteLine("\n{0}", content);
            }

            Console.ResetColor();
        }

        /// <summary>
        /// This method is for printing error responses
        /// </summary>
        /// <param name="response">response object</param>
        /// <param name="content">content in the response</param>
        public static void PrintErrorResponse(HttpWebResponse response, string content)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n\nHTTP/{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription);
            foreach (var webHeaderName in response.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", webHeaderName, response.Headers[webHeaderName]);
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n{0}", content);

            Console.ResetColor();
        }

        /// <summary>
        /// This method is for printing web responses
        /// </summary>
        /// <param name="response">response object</param>
        /// <param name="content">content in the web response</param>
        public static void PrintWebResponse(HttpWebResponse response, string content)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n\nHTTP/{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription);
            foreach (var webHeaderName in response.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", webHeaderName, response.Headers[webHeaderName]);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n{0}", content);

            Console.ResetColor();
        }
    }
}
