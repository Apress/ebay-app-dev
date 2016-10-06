/*
 UserInfo.cs

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
  
 Description
  This package provides an application that demonstrates 
  eBay.SDK.Model.User.User and eBay.SDK.API.GetUserCall.
*/

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using eBay.SDK.Model.User;
using eBay.SDK.API;

namespace com.lothlorien.ebaysdkbook
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class UserInfo  : com.lothlorien.ebaysdkbook.eBaySampleApplication
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ListBox detaillevel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox userid;
		private System.Windows.Forms.Button findButton;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RichTextBox result;
		
		public UserInfo()
		{
			InitializeComponent();
			// A reasonable default user
			this.userid.Text = this.ApiSession.RequestUserId;
			this.Gui = this.panel1;
		}

		private void find_Click(object sender, System.EventArgs e)
		{
			IGetUserCall api = new GetUserCall();
			IUser theUser = null;
			string resultString = null;

			// We're busy.
			this.Busy = true;
			this.StatusText = "Downloading...";

			// Set the API for this call.
		    api.CompatibilityLevel = 319;
			api.ErrorLevel = 
				ErrorLevelEnum.BothShortAndLongErrorStrings;
			api.ApiCallSession = this.ApiSession;

			api.UserId = this.userid.Text;
			api.DetailLevel = System.Convert.ToInt32( 
			  this.detaillevel.SelectedItem );
			
			try
			{
				theUser = api.GetUser();
			}
			catch( Exception ex )
			{
				resultString =  "**Error**\n" + ex.Message;
			}

			if ( theUser != null )
			{
				resultString = theUser.UserId + "\n" +
					theUser.Email + "\n" +
					theUser.PaymentAddress + "\n" +
					theUser.RegistrationAddress + "\n" +
					"Feedback score: " + theUser.FeedbackScore.ToString() + "\n\n" +  
					"The user is " + 
						( theUser.IdVerified ? "" : "not" ) + 
						"verified.\n" + 
					"The user " + 
						( theUser.AboutMe ? "has" : "doesn't have" ) + 
						" an AboutMe page.\n";
			}

			this.result.SuspendLayout();
			result.Text = resultString ;
			this.result.Enabled = true;
			this.result.ResumeLayout();

			// We're no longer busy.
			this.StatusText = "";
			this.Busy = false;
		}


		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.result = new System.Windows.Forms.RichTextBox();
			this.findButton = new System.Windows.Forms.Button();
			this.userid = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.detaillevel = new System.Windows.Forms.ListBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.panel2,
																				 this.findButton,
																				 this.userid,
																				 this.label2,
																				 this.label1,
																				 this.detaillevel});
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(488, 248);
			this.panel1.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.result});
			this.panel2.Location = new System.Drawing.Point(0, 40);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(456, 190);
			this.panel2.TabIndex = 4;
			// 
			// result
			// 
			this.result.Location = new System.Drawing.Point(8, 0);
			this.result.Name = "result";
			this.result.ReadOnly = true;
			this.result.Size = new System.Drawing.Size(440, 180);
			this.result.TabIndex = 0;
			this.result.Text = "";
			this.result.Enabled = false;
			// 
			// findButton
			// 
			this.findButton.Location = new System.Drawing.Point(376, 6);
			this.findButton.Name = "findButton";
			this.findButton.TabIndex = 3;
			this.findButton.Text = "Find";
			this.findButton.Click += new System.EventHandler(this.find_Click);
			// 
			// userid
			// 
			this.userid.Location = new System.Drawing.Point(96, 6);
			this.userid.Name = "userid";
			this.userid.Size = new System.Drawing.Size(136, 20);
			this.userid.TabIndex = 1;
			this.userid.Text = "userid";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(240, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "DetailLevel";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "eBay User ID";
			// 
			// detaillevel
			// 
			this.detaillevel.Items.AddRange(new object[] {
															 "0",
															 "2",
															 "4",
															 "6",
															 "8",
															 "10"});
			this.detaillevel.Location = new System.Drawing.Point(312, 8);
			this.detaillevel.Name = "detaillevel";
			this.detaillevel.Size = new System.Drawing.Size(56, 20);
			this.detaillevel.TabIndex = 2;
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
		}

		[STAThread]
		static void Main() 
		{
			Application.Run(new UserInfo());
		}
	}
}
