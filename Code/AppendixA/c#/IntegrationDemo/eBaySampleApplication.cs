/*
 eBaySampleApplication.cs

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
  This package provides a wrapper application to demonstrate
  application development with the eBay service.
*/

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using eBay.SDK.API;

namespace com.lothlorien.ebaysdkbook
{
	/// <summary>
	/// SampleApp provides a container application for the sample.
	/// </summary>
	public class eBaySampleApplication : System.Windows.Forms.Form
	{
		// eBay
		private IApiSession apiSession;

		// UI components
		private System.Windows.Forms.Button config;
		private System.Windows.Forms.Label status;
		private ConfigDialog configDialog;

		// Members specific to this example go here

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// The APISession for this application.
		/// </summary>
		public IApiSession ApiSession
		{
			get
			{
				return apiSession;
			}
		}
		private System.Windows.Forms.Control gui;
		/// <summary>
		/// The Windows Form subclass to place within our bounds
		/// </summary>
		public System.Windows.Forms.Control Gui
		{
			get
			{
				return gui;
			}
			set
			{
				int x = 10;
				int y = 10;
				Rectangle r = this.Bounds;
				int dx = r.X + r.Width - 20;
				r = this.status.Bounds;
				int dy = r.Y - y;

				if ( value == null ) return;
				this.SuspendLayout();
				value.Location = new System.Drawing.Point( x, y);
				value.Size = new System.Drawing.Size( dx, dy );
				this.Controls.AddRange(new System.Windows.Forms.Control[] { value });
				this.ResumeLayout(false);
				gui = value;
			}
		}

		/// <summary>
		/// Set text for the status line
		/// </summary
		public string StatusText
		{
			get
			{
				return status.Text;
			}
			set
			{
				status.Text = value;
			}
		}

		/// <summary>
		/// Application state 
		/// </summary>
		public bool Busy
		{
			get
			{
				return Busy;
			}
			set
			{
				if ( value )
				{
					Cursor.Current = Cursors.WaitCursor;
				}
				else
				{
					Cursor.Current = Cursors.Default;
				}
			}
		}

		/// <summary>
		///  The constructor. Initialize our components 
		///  and our eBay support.
		/// </summary>
		public eBaySampleApplication()
		{
			// Init our components, including the eBay one.
			InitializeComponent();
			InitializeEBayComponent();
		}

		private void InitializeEBayComponent( )
		{
			// Create the API session
			apiSession = new ApiSession();
			apiSession.LogCallXml = true;
			apiSession.Log.Open( "c:\\ebaylog.txt" );
			apiSession.Url = "https://api.sandbox.ebay.com/ws/api.dll";
			// Set the API session's access credentials
			// Load our access info.
			LoadKeys();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.config = new System.Windows.Forms.Button();
			this.status = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// config
			// 
			this.config.Location = new System.Drawing.Point(400, 240);
			this.config.Name = "config";
			this.config.TabIndex = 0;
			this.config.Text = "Settings";
			this.config.Click += new System.EventHandler(this.config_Click);
			// 
			// status
			// 
			this.status.Location = new System.Drawing.Point(10, 245);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(184, 24);
			this.status.TabIndex = 1;
			// 
			// SampleApp
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(480, 266);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.status,
																		  this.config});
			this.Name = "SampleApp";
			this.Text = "Container";
			this.ResumeLayout(false);
		}
		#endregion

		private void config_Click(object sender, System.EventArgs e)
		{
			if ( configDialog == null )
				configDialog = new ConfigDialog();
			configDialog.ShowDialog( this );
			// Blocking; this happens when the dialog closes.
			LoadKeys();
		}

		private void LoadKeys()
		{
			FileStream stream;
			StreamReader streamreader;
			String keystring; 
			String [] keys;
			Char [] eol = {'\n'};
	
			apiSession.Developer = "unknown";
			apiSession.Application = "unknown";
			apiSession.Certificate = "unknown";
			apiSession.RequestUserId = "unknown";
			apiSession.RequestPassword = "unknown";
			
			try
			{
				stream = 
					new FileStream( "c:\\Documents and Settings\\All Users\\ebayrc", System.IO.FileMode.Open );
				streamreader = new StreamReader( stream );
				keystring = streamreader.ReadToEnd();
				streamreader.Close();
				stream.Close();
				keys = keystring.Split( eol );
				apiSession.Developer = keys[0].Trim();
				apiSession.Application = keys[1].Trim();
				apiSession.Certificate = keys[2].Trim();
				apiSession.RequestUserId = keys[3].Trim();
				apiSession.RequestPassword = keys[4].Trim();
			}
			catch
			{
				MessageBox.Show( this, "Please set your eBay Developer Keys by pressing 'Settings' and entering your keys in the dialog box.", "Error" );
			}
		}
	}
}

