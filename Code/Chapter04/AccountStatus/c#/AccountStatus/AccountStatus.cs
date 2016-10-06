/*
 AccountStatus.cs

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
  eBay.SDK.Model.Account classes and eBay.SDK.API.GetAccount.
*/

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using eBay.SDK.Model.Account;
using eBay.SDK.API;

namespace com.lothlorien.ebaysdkbook
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class AccountStatus  : com.lothlorien.ebaysdkbook.eBaySampleApplication
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ListBox viewtype;
		private System.Windows.Forms.ListBox pagenumber;
		private System.Windows.Forms.DateTimePicker startdate;
		private System.Windows.Forms.DateTimePicker enddate;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox userid;
		private System.Windows.Forms.Button findButton;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RichTextBox result;
		
		public AccountStatus()
		{
			InitializeComponent();
			// A reasonable default user
			this.userid.Text = this.ApiSession.RequestUserId;
			this.Gui = this.panel1;
		}

		public int Month
		{
			get
			{
				return startdate.Value.Month;
			}
		}

		public int Year
		{
			get
			{
				return startdate.Value.Year;
			}
		}

		private void find_Click(object sender, System.EventArgs e)
		{
			IGetAccountCall api = new GetAccountCall();
			IAccount theAccount = null;
			string resultString = null;

			// We're busy.
			this.Busy = true;
			this.StatusText = "Downloading...";

			// Set the API for this call.
			api.CompatibilityLevel = 319;
			api.ErrorLevel = 
				ErrorLevelEnum.BothShortAndLongErrorStrings;
			api.ApiCallSession = this.ApiSession;
			api.PageNumber = System.Convert.ToInt32( this.pagenumber.SelectedItem );
			api.ViewType = (AccountViewEnum)this.viewtype.SelectedIndex;
			
			switch( api.ViewType )
			{
				case AccountViewEnum.ViewByPeriod:
					api.PeriodViewOption = new PeriodViewSettings();
					api.PeriodViewOption.DateRange = new DateRangeImpl();
					api.PeriodViewOption.DateRange.BeginDate = this.startdate.Value;
					api.PeriodViewOption.DateRange.EndDate = this.enddate.Value;
					break;

				case AccountViewEnum.ViewByInvoice:
					api.InvoiceViewOption = new InvoiceViewSettings();
					api.InvoiceViewOption.InvoiceMonth = (InvoiceMonthEnum)this.Month;
					api.InvoiceViewOption.InvoiceYear = this.Year;
					break;
			}

			try
			{
				theAccount = api.GetAccount();
			}
			catch( Exception ex )
			{
				resultString =  "**Error**\n" + ex.Message;
			}

			// Create a nice string with the results.
			if ( theAccount != null )
			{
				resultString = "Account: " + theAccount.Id + "\n";
				// Summary of invoice view.
				if ( theAccount.InvoiceView != null )
				{
					resultString += "Amount Due:\t" + theAccount.InvoiceView.InvoiceBalance + "\n";
					resultString += "Invoice Date:\t" + theAccount.InvoiceView.InvoiceDate.ToShortDateString() + "\n";
				}
				// Summary of period view.
				if ( theAccount.PeriodView != null )
				{
					resultString += "Last Invoice Date:\t\t" + theAccount.PeriodView.LastInvoiceDate.ToString() + "\n";
					resultString += "Last Invoice Amount:\t" + theAccount.PeriodView.LastInvoiceAmount.ToString() + "\n";
					resultString += "Last Payment Date:\t\t" + theAccount.PeriodView.LastPaymentDate + "\n";
					resultString += "Last Payment Amount:\t" + theAccount.PeriodView.LastAmountPaid + "\n";
					resultString += "Current Balance:\t\t" + theAccount.PeriodView.CurrentBalance + "\n";
				}
				// Enumerate each of the account activities
				resultString += "\nActivity History\n";
				foreach ( AccountActivity activity in theAccount.Activities )
				{
					resultString += activity.Date.ToShortDateString() + "\t";
					resultString += activity.Id.ToString() + "\t" + activity.ItemId + " " + activity.Memo + "\t";
					resultString += activity.Credit != 0 ? activity.Credit.ToString() : "";
					resultString += activity.Debit != 0 ? "(" + activity.Debit.ToString() + ")" : "";
					resultString += "\n";
				}
			}

			// Update the pagenumber menu with the list pages
			this.pagenumber.Items.Clear();
			for ( int i = 0; i <= api.TotalPages; i++ )
			{
				this.pagenumber.Items.Add( (i+1).ToString() );
			}
			this.pagenumber.SelectedIndex = 0;

			// Update the UI.
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.viewtype = new System.Windows.Forms.ListBox();
			this.pagenumber = new System.Windows.Forms.ListBox();
			this.startdate = new System.Windows.Forms.DateTimePicker();
			this.enddate = new System.Windows.Forms.DateTimePicker();

			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
				this.panel2,
				this.findButton,
				this.userid,
				this.label1,
			    this.label2,
			    this.label3,
				this.viewtype,
				this.pagenumber,
				this.startdate,
				this.enddate });
			this.panel1.Name = "InputPanel";
			this.panel1.Size = new System.Drawing.Size(488, 248);
			this.panel1.TabIndex = 0;
			
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 16);
			this.label1.Text = "User";

			// 
			// userid
			// 
			this.userid.Location = new System.Drawing.Point(50, 6);
			this.userid.Name = "userid";
			this.userid.Size = new System.Drawing.Size(60, 20);
			this.userid.Text = "userid";
			this.userid.ReadOnly = true;

			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(118, 10);
			this.label2.Name = "label1";
			this.label2.Size = new System.Drawing.Size(32, 16);
			this.label2.Text = "View";

			
			// 
			// viewtype
			// 
			this.viewtype.Items.Add("Period");
			this.viewtype.Items.Add("Invoice");
			this.viewtype.SelectedIndex = 0;
			this.viewtype.Location = new System.Drawing.Point(148, 8);
			this.viewtype.Name = "viewtype";
			this.viewtype.Size = new System.Drawing.Size(100, 20);
			this.viewtype.TabIndex = 1;

			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(262, 10);
			this.label3.Name = "label1";
			this.label3.Size = new System.Drawing.Size(36, 16);
			this.label3.Text = "Page";

			// 
			// pagenumber
			// 
			this.pagenumber.Items.Add("1");
			this.pagenumber.SelectedIndex = 0;
			this.pagenumber.Location = new System.Drawing.Point(298, 8);
			this.pagenumber.Name = "pagenumber";
			this.pagenumber.Size = new System.Drawing.Size(60, 20);
			this.pagenumber.TabIndex = 1;

			// 
			// startdate
			// 
			this.startdate.Location = new System.Drawing.Point(16, 40);
			this.startdate.Name = "Start";
			this.startdate.Size = new System.Drawing.Size(200, 20);

			// 
			// enddate
			// 
			this.enddate.Location = new System.Drawing.Point(245, 40);
			this.enddate.Name = "End";
			this.enddate.Size = new System.Drawing.Size(200, 20);

			// 
			// findButton
			// 
			this.findButton.Location = new System.Drawing.Point(370, 6);
			this.findButton.Name = "findButton";
			this.findButton.TabIndex = 3;
			this.findButton.Text = "Find";
			this.findButton.Click += new System.EventHandler(this.find_Click);

			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
				this.result});
			this.panel2.Location = new System.Drawing.Point(0, 80);
			this.panel2.Name = "ResultPanel";
			this.panel2.Size = new System.Drawing.Size(456, 150);
			this.panel2.TabIndex = 4;
			// 
			// result
			// 
			this.result.Location = new System.Drawing.Point(8, 0);
			this.result.Name = "result";
			this.result.ReadOnly = true;
			this.result.Size = new System.Drawing.Size(440, 140);
			this.result.TabIndex = 0;
			this.result.Text = "";
			this.result.Enabled = false;


			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
		}

		[STAThread]
		static void Main() 
		{
			Application.Run(new AccountStatus());
		}
	}
}
