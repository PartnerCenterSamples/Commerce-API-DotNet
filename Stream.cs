using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
	internal class Stream
	{
		// A partner should create one stream for each cid-for-reseller (in other words, for each market in which they sell). 
		// https://msdn.microsoft.com/en-us/library/partnercenter/dn974945.aspx
		private static string streamName = "stream-us";

		public static dynamic CreateStream(string resellerCid, string sa_Token)
		{
			var body = new
			{
				start_time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"),
				page_size = 100
			};

			var request = (HttpWebRequest)
				HttpWebRequest.Create(
						string.Format("https://api.cp.microsoft.com/{0}/subscription-streams/{0}/{1}", resellerCid, streamName));

			request.Method = "PUT";
			request.ContentType = "application/json";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + sa_Token);

			string content = Json.Encode(body);
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
					var createdStreamResponse = Json.Decode(responseContent);
					return createdStreamResponse;
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

		public static dynamic DeleteStream(string resellerCid, string sa_Token)
		{
			var request = (HttpWebRequest)
				HttpWebRequest.Create(
						string.Format("https://api.cp.microsoft.com/{0}/subscription-streams/{0}/{1}", resellerCid, streamName));

			request.Method = "DELETE";
			request.ContentType = "application/json";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + sa_Token);

			try
			{
				Utilities.PrintWebRequest(request, String.Empty);
				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
					var deletedStreamResponse = Json.Decode(responseContent);
					return deletedStreamResponse;
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

		private static dynamic GetStreamPage(string resellerCid, string sa_Token)
		{
			var request = (HttpWebRequest)
				HttpWebRequest.Create(
						string.Format("https://api.cp.microsoft.com/{0}/subscription-streams/{0}/{1}/pages", resellerCid, streamName));

			request.Method = "GET";
			request.ContentType = "application/json";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + sa_Token);

			try
			{
				Utilities.PrintWebRequest(request, String.Empty);
				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
					var streamPageResponse = Json.Decode(responseContent);
					return streamPageResponse;
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

		private static void MarkStreamPageComplete(string completionHref, string httpMethod, string sa_Token)
		{
			var request = (HttpWebRequest)
				HttpWebRequest.Create(
						string.Format("https://api.cp.microsoft.com{0}", completionHref));

			request.Method = httpMethod;
			request.ContentType = "application/json";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + sa_Token);

			try
			{
				Utilities.PrintWebRequest(request, String.Empty);
				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
					var pageCompleteResponse = Json.Decode(responseContent);
				}
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
					throw;
				}
			}
		}

		public static bool WaitForSubscriptionEvent(string resellerCid, string sa_Token, string subscriptionId, string subscriptionEventType)
		{
			int retryAfter = 30;   // seconds
			TimeSpan abortAfter = new TimeSpan(0, 5, 0);

			DateTime start = DateTime.Now;

			bool eventFound = false;
			do
			{
				var subscriptionEvents = GetStreamPage(resellerCid, sa_Token);
				foreach (var subscriptionEvent in subscriptionEvents.items)
				{
					var subscriptionUri = subscriptionEvent.subscription_uri;
					var sub = Subscription.GetSubscriptionByUri(subscriptionUri, sa_Token);
					if (sub.Id == subscriptionId)
					{
						if (subscriptionEvent.type == subscriptionEventType)
						{
							eventFound = true;
						}
					}
				}

				// event not found - mark page as read
				MarkStreamPageComplete(subscriptionEvents.links.completion.href, subscriptionEvents.links.completion.method, sa_Token);

				if (eventFound) { return true; }

				Thread.Sleep(retryAfter * 1000);
			} while (DateTime.Now < start + abortAfter);

			return false;
		}
	}

	internal static class StreamEvents
	{
		public static class Subscription
		{
			public const string Provisioned = "subscription_provisioned";
			public const string Deleted = "subscription_deleted";
			public const string Changed = "subscription_changed";
			public const string SuspensionAdded = "subscription_suspension_added";
			public const string SuspensionRemoved = "subscription_suspension_removed";
			public const string EndDateChanged = "subscription_end_date_changed";
			public const string QuantityChanged = "subscription_quantity_changed";
		}
	}
}
