/*
 validuser.cs

  Created by Ray Rischpater on 12 July 2003
  Copyright (c) 2003 by the author. All rights reserved.
  This file is provided without any warranty, express or 
  implied, including but not limited to fitness of purpose.
  You may use this file so long as the copyright attribution
  remains.

 This file is part of the book "Developing Applications with 
 eBay" written by Ray Rischpater and available from 
 Apress, LP.
   
 Dependencies
  - eBay SDK
  - Microsoft .NET
  
 Demonstrates
  - eBay.SDK.API.ApiSession
  - eBay.SDK.API.ValidateTestUserRegistrationCall;
  
 Description
  This application demonstrates the use of the eBay SDK to 
  validate a test user.
*/

// dependencies
using System;
using System.IO;
using eBay.SDK.API;


namespace com.lothlorien.ebaysdkbook
{
	class ValidateUser
	{
		private static string	devID;
		private static string	appID;
		private static string	crtID;

		static void Main(string[] args)
		{
			IApiSession apiSession;
			ValidateTestUserRegistrationCall api;

			//create ApiSession object.
			Console.Write( "Creating eBay.SDK.API.ApiSession " );
			Console.WriteLine( "object..." );
			apiSession = new ApiSession();
			apiSession.LogCallXml = true;
			apiSession.Url = "https://api.sandbox.ebay.com/ws/api.dll ";			
			// Get the eBay keys
			getKeys();
			apiSession.Developer = devID;
			apiSession.Application = appID;
			apiSession.Certificate = crtID;

			// Find out which account we should validate.
			Console.Write( "Enter eBay User ID of " );
			Console.Write( "the seller account: ");
			apiSession.RequestUserId = Console.ReadLine();

			Console.Write( "Enter password to the " );
			Console.Write( "seller account: " );
			apiSession.RequestPassword = Console.ReadLine();

			// Create the API Request
			Console.Write( "Creating " );
			Console.Write( "eBay.SDK.API." );
			Console.Write( "ValidateTestUserRegistrationCall ");
			Console.WriteLine( "object..." );
			api = new ValidateTestUserRegistrationCall();
			api.ErrorLevel = 
					ErrorLevelEnum.BothShortAndLongErrorStrings;
			api.ApiCallSession = apiSession;

			// Issue the call
			Console.Write( "Issuing ValidateTestUserRegistration() ");
			Console.WriteLine( "call..." );
			try
			{
				api.ValidateTestUserRegistration();
				Console.Write( "Congratulations! " );
				Console.Write( "The user has been validated " );
				Console.WriteLine( "successfully!" );
			}
			catch( Exception e )
			{
				Console.Write( "**Error**: " );
				Console.Write( e.Message );
			}
			
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
