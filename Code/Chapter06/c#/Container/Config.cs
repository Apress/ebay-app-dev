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

  
 Description
*/



using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace com.lothlorien.ebaysdkbook
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class ConfigDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox devid;
		private System.Windows.Forms.TextBox appid;
		private System.Windows.Forms.TextBox crtid;
		private System.Windows.Forms.Button ok;
		private System.Windows.Forms.Button cancel;
		private string user;
		private string password;
		private string dbConfig;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ConfigDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.ok.Click += new System.EventHandler(this.ok_Click);

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.devid = new System.Windows.Forms.TextBox();
			this.appid = new System.Windows.Forms.TextBox();
			this.crtid = new System.Windows.Forms.TextBox();
			this.ok = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "eBay Developer ID:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "eBay Certificate ID:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(32, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "eBay Application ID:";
			// 
			// devid
			// 
			this.devid.Location = new System.Drawing.Point(24, 24);
			this.devid.Name = "devid";
			this.devid.Size = new System.Drawing.Size(128, 20);
			this.devid.TabIndex = 3;
			this.devid.Text = "devid";
			// 
			// appid
			// 
			this.appid.Location = new System.Drawing.Point(24, 64);
			this.appid.Name = "appid";
			this.appid.Size = new System.Drawing.Size(128, 20);
			this.appid.TabIndex = 4;
			this.appid.Text = "appid";
			// 
			// crtid
			// 
			this.crtid.Location = new System.Drawing.Point(24, 104);
			this.crtid.Name = "crtid";
			this.crtid.Size = new System.Drawing.Size(128, 20);
			this.crtid.TabIndex = 5;
			this.crtid.Text = "appid";
			// 
			// ok
			// 
			this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ok.Location = new System.Drawing.Point(8, 136);
			this.ok.Name = "ok";
			this.ok.TabIndex = 6;
			this.ok.Text = "OK";
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(96, 136);
			this.cancel.Name = "cancel";
			this.cancel.TabIndex = 7;
			this.cancel.Text = "Cancel";
			// 
			// ConfigDialog
			// 
			this.AcceptButton = this.ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(176, 166);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.cancel,
																		  this.ok,
																		  this.crtid,
																		  this.appid,
																		  this.devid,
																		  this.label3,
																		  this.label2,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ConfigDialog";
			this.Text = "Settings";
			this.Load += new System.EventHandler(this.config_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void config_Load(object sender, System.EventArgs e)
		{
				FileStream stream;
				StreamReader streamreader;
				String keystring; 
				String [] keys;
				Char [] eol = {'\n'};
				try
				{
					stream = 
						new FileStream( "c:\\Documents and Settings\\All Users\\ebayrc", System.IO.FileMode.Open );
					streamreader = new StreamReader( stream );
					keystring = streamreader.ReadToEnd();
					streamreader.Close();
					stream.Close();
					keys = keystring.Split( eol );
					devid.Text = keys[0].Trim();
					appid.Text = keys[1].Trim();
					crtid.Text = keys[2].Trim();
					user = keys[3].Trim();
					password = keys[4].Trim();
					dbConfig = keys[5].Trim();
				}
				catch
				{
					devid.Text = "Deveoper ID";
					appid.Text = "Application ID";
					crtid.Text = "Certificate ID";
				}
		}


		private void ok_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			// Save settings
			try
			{
				FileStream stream = File.OpenWrite( "c:\\Documents and Settings\\All Users\\ebayrc" );
				StreamWriter streamWriter = new StreamWriter( stream );
				streamWriter.Write( devid.Text );
				streamWriter.Write( "\n" );
				streamWriter.Write( appid.Text );
				streamWriter.Write( "\n" );
				streamWriter.Write( crtid.Text );
				streamWriter.Write( "\n" );
				streamWriter.Write( user );
				streamWriter.Write( "\n" );
				streamWriter.Write( password );
				streamWriter.Write( "\n" );
				streamWriter.Write( dbConfig );
				streamWriter.Write( "\n" );
				streamWriter.Flush();
				streamWriter.Close();
				stream.Close();
			}
			catch
			{
				MessageBox.Show( this, "Could not save settings.", "Error" );
			}
		}
	}
}
