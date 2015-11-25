/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
	using System.Collections.Generic;

	public class OfferCatalog
	{
		/// <summary>
		/// Contains offers grouped by offertype
		/// </summary>
		public IEnumerable<GroupedOffers> GroupedOffersCollection { get; private set; }

		private static OfferCatalog offerCatalog { get; set; }

		private static object singletonObject = new object();

		private OfferCatalog()
		{
			//// Added a couple of offers for the sample, the idea is to add all of the offers from the excel sheet and use them
			GroupedOffersCollection = new List<GroupedOffers>
			{
				new GroupedOffers
				{
					OfferType = OfferType.Azure,
					Offers = new List<Offer>
					{
						//// This offer is only available to the integration sandbox
						//// Uncomment the section below for Azure.
						new Offer
						{
							Name = "Microsoft Azure For integration sandbox",
							Uri = "/fbf178a5-144e-46d1-aa81-612c2d3f97f4/offers/MS-AZR-0146P",
							Id = "MS-AZR-0146P",
							MinimumQuantity = 1,
							MaximumQuantity = 999999999,
						},

						////// If this offer is being purchased for an integration sandbox, you will get an error
						//new Offer
						//{
						//		Name = "Microsoft Azure",
						//		Uri = "/fbf178a5-144e-46d1-aa81-612c2d3f97f4/offers/MS-AZR-0145P",
						//		Id = "MS-AZR-0145P",
						//		MinimumQuantity = 1,
						//		MaximumQuantity = 999999999,
						//}
					}
				},
				new GroupedOffers
				{
					OfferType = OfferType.IntuneAndOffice,
					Offers = new List<Offer>
					{
						new Offer
						{
							Name = "Azure Active Directory Basic",
							Uri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/84A03D81-6B37-4D66-8D4A-FAEA24541538",
							Id = "84a03d81-6b37-4d66-8d4a-faea24541538",
							MinimumQuantity = 1,
							MaximumQuantity = 10000000,
						},
						new Offer
						{
							Name = "Azure Active Directory Premium",
							Uri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/16C9F982-A827-4003-A88E-E75DF1927F27",
							Id = "16c9f982-a827-4003-a88e-e75df1927f27",
							MinimumQuantity = 1,
							MaximumQuantity = 10000000,
						},
						new Offer
						{
							Name = "Intune",
							Uri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/51E95709-DC35-4780-9040-22278CB7C0E1",
							Id = "51e95709-dc35-4780-9040-22278cb7c0e1",
							MinimumQuantity = 1,
							MaximumQuantity = 10000000,
						},
						new Offer
						{
							Name = "Enterprise Mobility Suite",
							Uri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/79C29AF7-3CD0-4A6F-B182-A81E31DEC84E",
							Id = "79c29af7-3cd0-4a6f-b182-a81e31dec84e",
							MinimumQuantity = 1,
							MaximumQuantity = 10000000,
						},
						new Offer
						{
							Name = "Exchange Online (Plan 1)",
							Uri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/195416C1-3447-423A-B37B-EE59A99A19C4",
							Id = "195416c1-3447-423a-b37b-ee59a99a19c4",
							MinimumQuantity = 1,
							MaximumQuantity = 10000000,
							CanConvertTo = new List<string>
							{
								"796B6B5F-613C-4E24-A17C-EBA730D49C02",
								"031C9E47-4802-4248-838E-778FB1D2CC05",
								"BD938F12-058F-4927-BBA3-AE36B1D2501C",
								"91FD106F-4B2C-4938-95AC-F54F74E9A239"
							}
						},
						new Offer
						{
							Name = "Lync Online (Plan 1)",
							Uri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/2F707C7C-2433-49A5-A437-9CA7CF40D3EB",
							Id = "aca0c06c-890d-4abb-83cf-bc519a2565e5",
							MinimumQuantity = 1,
							MaximumQuantity = 10000000,
							CanConvertTo = new List<string>
							{
								"796B6B5F-613C-4E24-A17C-EBA730D49C02",
								"031C9E47-4802-4248-838E-778FB1D2CC05",
								"BD938F12-058F-4927-BBA3-AE36B1D2501C",
								"91FD106F-4B2C-4938-95AC-F54F74E9A239"
							}
						},
						new Offer
						{
							Name = "Office 365 Enterprise E1",
							Uri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/91FD106F-4B2C-4938-95AC-F54F74E9A239",
							Id = "91fd106f-4b2c-4938-95ac-f54f74e9a239",
							MinimumQuantity = 1,
							MaximumQuantity = 10000000
						},
						new Offer
						{
							Name = "Office 365 Enterprise K1",
							Uri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/6FBAD345-B7DE-42A6-B6AB-79B363D0B371",
							Id = "6FBAD345-B7DE-42A6-B6AB-79B363D0B371",
							MinimumQuantity = 1,
							MaximumQuantity = 10000000
						},
						new Offer
						{
							Name = "Exchange Online Archiving for Exchange Online",
							Uri = "/3c95518e-8c37-41e3-9627-0ca339200f53/offers/2828BE95-46BA-4F91-B2FD-0BEF192ECF60",
							Id = "2828be95-46ba-4f91-b2fd-0bef192ecf60",
							MinimumQuantity = 1,
							MaximumQuantity = 10000000,
							DependsOn = new List<string>
							{
								"35A36B80-270A-44BF-9290-00545D350866",
								"6FBAD345-B7DE-42A6-B6AB-79B363D0B371",
								"91FD106F-4B2C-4938-95AC-F54F74E9A239",
								"837F8912-2321-41D1-A50A-9F3C4F43C93B",
								"195416C1-3447-423A-B37B-EE59A99A19C4"
							}
						},
					}
				}
			};
		}

		/// <summary>
		/// Gets an instance of the Offer Catalog
		/// </summary>
		public static OfferCatalog Instance
		{
			get
			{
				if (offerCatalog != null)
				{
					return offerCatalog;
				}

				lock (singletonObject)
				{
					if (offerCatalog == null)
					{
						offerCatalog = new OfferCatalog();
					}
				}

				return offerCatalog;
			}
		}
	}

	public class GroupedOffers
	{
		/// <summary>
		/// Offer type for this group of offers
		/// </summary>
		public OfferType OfferType { get; set; }
		public IEnumerable<Offer> Offers { get; set; }
	}

	public class Offer
	{
		/// <summary>
		/// Name for the offer
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// URI of the offer used while placing the order
		/// </summary>
		public string Uri { get; set; }

		/// <summary>
		/// Unique identifier for the offer
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Minimum quantity for placing the order
		/// </summary>
		public int MinimumQuantity { get; set; }

		/// <summary>
		/// Maximum quantity for placing the order
		/// </summary>
		public int MaximumQuantity { get; set; }

		/// <summary>
		/// A collection of offers ids that this offer can be converted to
		/// </summary>
		public IEnumerable<string> CanConvertTo { get; set; }

		/// <summary>
		/// A collection of offers that this offer depends on
		/// If any of the offers in this collection is purchased by a customer, then this offer can be purchased as well
		/// </summary>
		public IEnumerable<string> DependsOn { get; set; }
	}

	public enum OfferType
	{
		/// <summary>
		/// All offers in Azure
		/// Currently, an Order can be placed with offers of only one offertype
		/// </summary>
		Azure,

		/// <summary>
		/// All offers in Intune and Office
		/// </summary>
		IntuneAndOffice
	}
}