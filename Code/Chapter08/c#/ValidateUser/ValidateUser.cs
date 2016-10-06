/*
 validuser.cs

  Created by Ray Rischpater on 2 November 2003
  Copyright (c) 2003 by the author. All rights reserved.
  This file is provided without any warranty, express or 
  implied, including but not limited to fitness of purpose.
  You may use this file so long as the copyright attribution
  remains.

 This file is part of the book "Developing Applications with 
 eBay" written by Ray Rischpater and available from 
 Apress, LP.
   
 Dependencies
  - eBay API
  - Microsoft .NET
  
 Demonstrates
  - Using the eBay API.
  
 Description
  This application demonstrates the use of the eBay API to 
  validate a test user.
*/

// dependencies
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;


namespace com.lothlorien.ebaysdkbook
{
	class ValidateUser
	{
		private static string	devID;
		private static string	appID;
		private static string	crtID;

		static void Main(string[] args)
		{
			const string ebayAPIUrl = "https://api.sandbox.ebay.com/ws/api.dll";
			const string detailLevel = "0";
			const string siteId = "0";
			const string verb = "ValidateTestUserRegistration";
			const string compatLevel = "327";
			Uri requestUri = new Uri( ebayAPIUrl );
			HttpWebRequest  request;
			HttpWebResponse response = null;			
			string userid, password;
			string xmlRequest;
			StreamWriter writer;
			XmlTextReader parser;
			string result;

			// The initial XML template.
			xmlRequest = "<?xml version=\"1.0\"?><request>" +
				"<RequestUserId>^userid^</RequestUserId>" +
				"<RequestPassword>^password^</RequestPassword>" +
  				"<ErrorLevel>1</ErrorLevel>" +
				"<DetailLevel>^detaillevel^</DetailLevel>" +
				"<Verb>^verb^</Verb>" +
				"<SiteId>^siteid^</SiteId>" +
				"</request>";

			// Create the web client object
			request = (HttpWebRequest) WebRequest.Create( requestUri );
			// CAUTION: This is case-sensitive!
			request.Method = "POST";
			
			// Get the eBay keys
			getKeys();

			// Find out which account we should validate.
			Console.Write( "Enter eBay User ID of " );
			Console.Write( "the seller account: ");
			userid = Console.ReadLine();

			Console.Write( "Enter password to the " );
			Console.Write( "seller account: " );
			password = Console.ReadLine();

			// Create the request XML.
			xmlRequest = xmlRequest.Replace( "^userid^", userid );
			xmlRequest = xmlRequest.Replace( "^password^", password );
			xmlRequest = xmlRequest.Replace( "^detaillevel^", detailLevel );
			xmlRequest = xmlRequest.Replace( "^verb^", verb );
			xmlRequest = xmlRequest.Replace( "^siteid^", siteId );
			
			// Create the API Request HTTP Headers
			request.ContentType = "text/xml";
			request.ContentLength = xmlRequest.Length;
			request.Headers.Set("X-EBAY-API-COMPATIBILITY-LEVEL", compatLevel );
			request.Headers.Set("X-EBAY-API-SESSION-CERTIFICATE", 
				devID + ";" + appID + ";" + crtID );
			request.Headers.Set("X-EBAY-API-DEV-NAME" , devID );
			request.Headers.Set("X-EBAY-API-APP-NAME", appID );
			request.Headers.Set("X-EBAY-API-CERT-NAME" , crtID );
			request.Headers.Set("X-EBAY-API-CALL-NAME" , verb );
			request.Headers.Set("X-EBAY-API-SITEID" , siteId );
			request.Headers.Set("X-EBAY-API-DETAIL-LEVEL" , detailLevel );

			// Create the request
			writer = new StreamWriter( request.GetRequestStream() ); 
			writer.Write( xmlRequest ); 
			writer.Close(); 

			// Issue the call
			Console.Write( "Issuing ValidateTestUserRegistration ");
			Console.WriteLine( "API call..." );
			try
			{
				response = (HttpWebResponse)request.GetResponse( );
						
				// Parse XML
				parser = new XmlTextReader( response.GetResponseStream() );

				result = "Congratulations! " +
					"The user has been validated successfully!";

				while( parser.Read() )
				{
					if ( parser.NodeType == XmlNodeType.Element ) 
					{
						if ( parser.Name == "LongMessage" )
						{
							parser.Read();
							result = parser.Value;
						}
					}
				}
				parser.Close();
				Console.WriteLine( result );
			}
			catch( Exception e )
			{
				Console.Write( "**Error**: " );
				Console.Write( e.Message );
			}


			// Show the results			
			Console.ReadLine();
		}

		/*
		 * Fetches the eBay developer keys from either
		 * a dotfile or from the input line if the dotfile
		 * isn't availalble.
		 */
		static void getKeys( )
		{
			FileStream stream;
			StreamReader streamreader;
			String keystring; 
			String [] keys;
			Char [] eol = {'\n'};
			try
			{
				stream = 
					new FileStream( ".ebayrc", System.IO.FileMode.Open );
				streamreader = new StreamReader( stream );
				keystring = streamreader.ReadToEnd();
				keys = keystring.Split( eol );
				devID = keys[0].Trim();
				appID = keys[1].Trim();
				crtID = keys[2].Trim();
			}
			catch
			{
				Console.Write( "Enter API Developer Id: " );
				devID = Console.ReadLine();
				Console.Write( "Enter API Application Id: " );
				appID = Console.ReadLine();
				Console.Write( "Enter API Certificate: " );
				crtID = Console.ReadLine();
			}
		}
	}
}
