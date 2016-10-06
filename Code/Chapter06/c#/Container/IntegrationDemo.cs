/*
 Container.cs

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
 


	 *  Things for the chapter...
	 * Item status from Saved to end of auction
	 * Setting vs. storage.
	 * Are there page APIs in the sync library?
*/



using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;


using eBay.SDK;
using eBay.SDK.Model;
using eBay.SDK.Model.User;
using eBay.SDK.Model.Item;
using eBay.SDK.Integration;

namespace com.lothlorien.ebaysdkbook
{
	/// <summary>
	/// SampleApp provides a container application for the sample.
	/// </summary>
	public class SampleApp : System.Windows.Forms.Form
	{
		// eBay access stuff
		private string devid;
		private string appid;
		private string crtid;
		private string dbConfig;

		bool bNewUser;

		// User credentials for this user
		private IUser user;
		private IAppUser appUser;
		public IAppUser AppUser
		{
			get 
			{
				return this.appUser;
			}
			set 
			{
				this.appUser = value;
				try
				{
					session = dataStore.GetEBaySession( appUser );

					// Create a new status listener
					StatusListener l = new StatusListener();
	                l.RegisterStatusUpdate(
							new StatusListener.StatusUpdateDelegate(UpdateStatus) );
					session.StatusPublisher.AddListener( l );
				}
				catch
				{
					session = null;
				}
			}
		}

		// Integration Library info
		eBay.SDK.Integration.IDataStore dataStore;
		eBay.SDK.Integration.IEnvironment environment;
		public eBay.SDK.Integration.IEnvironment EBayEnvironment
		{
			get 
			{
				return this.environment;
			}
		}

		eBay.SDK.Integration.IEBaySession session;
		public IEBaySession Session
		{
			get
			{	
				return this.session;
			}
		}
		#region UI components
		private System.Windows.Forms.Button config;
		private System.Windows.Forms.Label status;
		private ConfigDialog configDialog;
		private System.Windows.Forms.ListView itemView;
		private System.Windows.Forms.ColumnHeader itemNumber;
		private System.Windows.Forms.ColumnHeader itemTitle;
		private System.Windows.Forms.ColumnHeader itemPrice;
		private System.Windows.Forms.ColumnHeader auctionState;

		private LoginForm loginForm;
		#endregion	
		private System.Windows.Forms.Button loginButton;
		private System.Windows.Forms.Button newItemButton;
		private System.Windows.Forms.Button updateCategoriesButton;
		private System.Windows.Forms.StatusBar statusBar1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SampleApp()
		{
			InitializeComponent();

			bNewUser = false;

			// Load our configuration
			LoadKeys();
			// Load our environment from the database
			LoadEnvironment();

			// Prompt for login information
			loginForm = new LoginForm( this );
			loginButton.Text = "Log In";
			itemView.Hide();
			newItemButton.Hide();
			updateCategoriesButton.Hide();

			loginForm.ShowDialog();
		}

		private void LoadEnvironment()
		{
			// Create the integration datastore.
			dataStore = new eBay.SDK.Integration.DataStore();
			// Connect it to the default database.
			try
			{
				dataStore.Connect( dbConfig );
				// Load the database's environment
				environment = dataStore.LoadEnvironment( (int)eBay.SDK.Integration.EnvironmentEnum.SANDBOX );
			}
			catch
			{
				MessageBox.Show( this, "Could not connect to the Integration Database.", "Error"  );
				this.Close();
				dataStore.Disconnect();
			}
		}


		public IAppUser LoadUser( string id )
		{
			IAppUser result = null;

			try
			{
				result =  environment.UserManager.LoadUser( int.Parse( id ) ); 
			}
			catch
			{
				result = null;
			}
			return result;
		}

		public IAppUser NewUser( string id, string password, string email )
		{
			IAppUser ebayUser = new AppUser( EBayEnvironment );
			ebayUser.EBayUserId = id;
			ebayUser.Password = password;
			ebayUser.Email = email;
			
			try
			{
				EBayEnvironment.UserManager.SaveUser( ebayUser );
				bNewUser = true;
				return ebayUser;
			}
			catch
			{
				return null;
			}
		}	

		public void DownloadUser()
		{
			// Set the cursor to busy.
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				// Update the user info.
				session.EBaySynchronizer.UpdateUserDetailsFromEBay();
				user = session.LoadBuyer(appUser.EBayUserId);

				if ( bNewUser )
				{
					session.EBaySynchronizer.ImportItemsFromEBay( );
					session.EBaySynchronizer.GetFeedbacksFromEBay( eBay.SDK.Model.SiteIdEnum.US );
				}
			}
			catch
			{
				Logoff( false );
				MessageBox.Show( this, "Could not connect to eBay.", "Error" );
			}

			bNewUser = false;

			// Set the cursor to normal.
			Cursor.Current = Cursors.Default;
		}


		public void UploadItems()
		{
			// Set the cursor to busy.
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				session.EBaySynchronizer.ListItemsToEBay();
			}
			catch
			{
				MessageBox.Show( this, "Could not connect to eBay.", "Error" );
			}

			// Set the cursor to normal.
			Cursor.Current = Cursors.Default;
		}

		public void DownloadItems()
		{
			// Set the cursor to busy.
			Cursor.Current = Cursors.WaitCursor;

			// Update the status line.
			// Do the sync.
			try
			{
				session.EBaySynchronizer.UpdatePricesFromEBay();
				session.EBaySynchronizer.UpdateSalesFromEBay();
			}
			catch
			{
				Logoff( false );
				MessageBox.Show( this, "Could not connect to eBay.", "Error" );
			}

			// Update the status line.
			// Set the cursor to normal.
			Cursor.Current = Cursors.Default;
		}


		public void UpdateItemView()
		{
			IItemCollection activeItems = null;
			IItemCollection soldItems = null;
			IItemCollection pendingItems = null;
			IItemCollection savedItems = null;

			itemView.Items.Clear();

			// First, snag the data from eBay.
			try
			{
				activeItems = session.LoadItems( AppStatusEnum.Active );
				soldItems = session.LoadItems( AppStatusEnum.Sold );
				pendingItems = session.LoadItems( AppStatusEnum.PendingAdd );
				savedItems = session.LoadItems( AppStatusEnum.Saved );
			}
			catch
			{
				MessageBox.Show( this, "Could not connect to the Integration Database.", "Error" );
			}
			
			// Now, popuplate the ListView
			foreach( IItem item in pendingItems )
			{			
				ListViewItem viewItem = new ListViewItem( item.ItemId.ToString(), 0 );
				viewItem.SubItems.Add( item.Title );
				viewItem.SubItems.Add( "Pending" );
				viewItem.SubItems.Add( item.CurrentPrice.ToString() );
				itemView.Items.Add( viewItem );
			}

			foreach( IItem item in activeItems )
			{			
				ListViewItem viewItem = new ListViewItem( item.ItemId.ToString(), 0 );
				viewItem.SubItems.Add( item.Title );
				viewItem.SubItems.Add( "Active" );
				viewItem.SubItems.Add( item.CurrentPrice.ToString() );
				itemView.Items.Add( viewItem );
			}
			foreach( IItem item in soldItems )
			{			
				ListViewItem viewItem = new ListViewItem( item.ItemId.ToString(), 0 );
				viewItem.SubItems.Add( item.Title );
				viewItem.SubItems.Add( "Sold" );
				viewItem.SubItems.Add( item.CurrentPrice.ToString() );
				itemView.Items.Add( viewItem );
			}
			foreach( IItem item in savedItems )
			{			
				ListViewItem viewItem = new ListViewItem( item.ItemId.ToString(), 0 );
				viewItem.SubItems.Add( item.Title );
				viewItem.SubItems.Add( "Saved" );
				viewItem.SubItems.Add( item.CurrentPrice.ToString() );
				itemView.Items.Add( viewItem );
			}
		}

		public void Login( bool bSyncEBay )
		{
			// Update the contents from eBay
			if ( bSyncEBay )
			{
				DownloadUser();
				DownloadItems();
				// Now update the user interface
				UpdateItemView();
			}

			itemView.Show();
			newItemButton.Show();
			updateCategoriesButton.Show();
			loginButton.Text = "Log Off";
		}

		public void Logoff( bool bSyncEBay )
		{
			if ( bSyncEBay )
			{
				// Update items to eBay.
				UploadItems();
			}
			itemView.Hide();
			newItemButton.Hide();
			updateCategoriesButton.Hide();
			loginButton.Text = "Log In";
			loginForm.ShowDialog();
			dataStore.Disconnect();
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
			this.loginButton = new System.Windows.Forms.Button();
			this.itemView = new System.Windows.Forms.ListView();
			this.itemNumber = new System.Windows.Forms.ColumnHeader();
			this.itemTitle = new System.Windows.Forms.ColumnHeader();
			this.auctionState = new System.Windows.Forms.ColumnHeader();
			this.itemPrice = new System.Windows.Forms.ColumnHeader();
			this.newItemButton = new System.Windows.Forms.Button();
			this.updateCategoriesButton = new System.Windows.Forms.Button();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.SuspendLayout();
			// 
			// config
			// 
			this.config.Location = new System.Drawing.Point(184, 240);
			this.config.Name = "config";
			this.config.TabIndex = 3;
			this.config.Text = "Settings...";
			this.config.Click += new System.EventHandler(this.config_Click);
			// 
			// status
			// 
			this.status.Location = new System.Drawing.Point(10, 245);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(142, 24);
			this.status.TabIndex = 1;
			this.status.Visible = false;
			// 
			// loginButton
			// 
			this.loginButton.Location = new System.Drawing.Point(352, 240);
			this.loginButton.Name = "loginButton";
			this.loginButton.TabIndex = 0;
			this.loginButton.Text = "Log Off";
			this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
			// 
			// itemView
			// 
			this.itemView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.itemNumber,
																					   this.itemTitle,
																					   this.auctionState,
																					   this.itemPrice});
			this.itemView.FullRowSelect = true;
			this.itemView.GridLines = true;
			this.itemView.Location = new System.Drawing.Point(8, 8);
			this.itemView.Name = "itemView";
			this.itemView.Size = new System.Drawing.Size(416, 224);
			this.itemView.TabIndex = 4;
			this.itemView.View = System.Windows.Forms.View.Details;
			this.itemView.Visible = false;
			// 
			// itemNumber
			// 
			this.itemNumber.Text = "Item #";
			this.itemNumber.Width = 80;
			// 
			// itemTitle
			// 
			this.itemTitle.Text = "Title";
			this.itemTitle.Width = 132;
			// 
			// auctionState
			// 
			this.auctionState.Text = "Auction";
			this.auctionState.Width = 80;
			// 
			// itemPrice
			// 
			this.itemPrice.Text = "Current Price";
			this.itemPrice.Width = 120;
			// 
			// newItemButton
			// 
			this.newItemButton.Location = new System.Drawing.Point(8, 240);
			this.newItemButton.Name = "newItemButton";
			this.newItemButton.TabIndex = 1;
			this.newItemButton.Text = "New...";
			this.newItemButton.Click += new System.EventHandler(this.newItemButton_Click);
			// 
			// updateCategoriesButton
			// 
			this.updateCategoriesButton.Location = new System.Drawing.Point(96, 240);
			this.updateCategoriesButton.Name = "updateCategoriesButton";
			this.updateCategoriesButton.TabIndex = 9;
			this.updateCategoriesButton.Text = "Categories";
			this.updateCategoriesButton.Click += new System.EventHandler(this.updateCategoriesButton_Click);
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 272);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(432, 22);
			this.statusBar1.TabIndex = 10;
			// 
			// SampleApp
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(432, 294);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.statusBar1,
																		  this.updateCategoriesButton,
																		  this.config,
																		  this.newItemButton,
																		  this.itemView,
																		  this.loginButton,
																		  this.status});
			this.Name = "SampleApp";
			this.Text = "IntegrationDemo";
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new SampleApp());
		}

		private void loginButton_Click(object sender, System.EventArgs e)
		{
			Logoff( true );
		}

		private void config_Click(object sender, System.EventArgs e)
		{
			if ( configDialog == null )
				configDialog = new ConfigDialog();
			configDialog.ShowDialog( this );
		}

		private void LoadKeys()
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
				devid = keys[0].Trim();
				appid = keys[1].Trim();
				crtid = keys[2].Trim();
				dbConfig = keys[5].Trim();
			}
			catch
			{
				devid = "unknown Deveoper ID";
				appid = "unknown Application ID";
				crtid = "unknown Certificate ID";
				dbConfig = "unknown DB Configuration";
				MessageBox.Show( this, "Please set your eBay Developer Keys by pressing 'Settings' and entering your keys in the dialog box.", "Error" );
			}
		}

		private void newItemButton_Click(object sender, System.EventArgs e)
		{
			// Prompt the user to create a new item.
			NewItemForm newItemForm = new NewItemForm();

			newItemForm.App = this;
			newItemForm.ShowDialog( );
		}

		private void updateCategoriesButton_Click(object sender, System.EventArgs e)
		{
			try
			{
				session.EBaySynchronizer.SynchronizeCategories( eBay.SDK.Model.SiteIdEnum.US, true );
			}
			catch
			{
				MessageBox.Show( this, "Could not connect to eBay." );
			}
		}

		void UpdateStatus( string message, int percent )
		{
			status.Text = message;
			System.Console.WriteLine( message );
		}
	}
}



