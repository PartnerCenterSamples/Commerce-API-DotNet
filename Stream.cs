/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Helpers;

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
	internal class Stream
	{
		// A partner should create one stream for each cid-for-reseller (in other words, for each market in which they sell). 
		// https://msdn.microsoft.com/en-us/library/partnercenter/dn974945.aspx
		private static string streamName = "stream-us";

		/// <summary>
		/// Streams are forward moving cursors with no ability to retrieve a completed page on a given stream. As new events are published, they will become available for consumption on the stream.
		/// </summary>
		/// <param name="resellerCid">cid of the reseller</param>
		/// <param name="sa_Token">sales agent token</param>
		/// <returns>object describing the stream</returns>
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

/// <summary>
/// Delete an event stream
/// </summary>
/// <param name="resellerCid">cid of the reseller</param>
/// <param name="sa_Token">sales agent token</param>
/// <returns>object the describes the deleted stream</returns>
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

		/// <summary>
		/// Retreives the next page of events
		/// </summary>
		/// <param name="resellerCid">cid of the reseller</param>
		/// <param name="sa_Token">sales agent token</param>
		/// <returns>array of items describing the events</returns>
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

		/// <summary>
		/// Signals that events have been processed and can be removed from stream. Un-marked pages are resent.
		/// </summary>
		/// <param name="completionHref">endpoint to call to mark page as read</param>
		/// <param name="httpMethod">Http method to use in call</param>
		/// <param name="sa_Token">sales agent token</param>
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

		/// <summary>
		/// Block thread, waiting for specified event type to occur on specified subscription. Events for other subscriptions are discarded.
		/// </summary>
		/// <param name="resellerCid">cid of the reseller</param>
		/// <param name="sa_Token">sales agent token</param>
		/// <param name="subscriptionId">id of the subscription to watch</param>
		/// <param name="subscriptionEventType">event type to watch</param>
		/// <returns></returns>
		public static bool WaitForSubscriptionEvent(string resellerCid, string sa_Token, string subscriptionId, string subscriptionEventType)
		{
			int retryAfter = 60;   // seconds  - Make a new call after this interval
			TimeSpan abortAfter = new TimeSpan(0, 10, 0);  // stop waiting after this interval

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

				if (eventFound)
				{
					// wait for X minutes...
					//Thread.Sleep(5 * 60 * 1000);
					return true;
				}

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
