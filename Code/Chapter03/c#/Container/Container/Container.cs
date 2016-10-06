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
  - Multiple threads to control application startu.
  - eBay.SDK.API.GetCategoriesCall
  - eBay.SDK.Category
  - eBay.SDK.CategoryCollection
  
 Description
  - This application demonstrates using the eBay Category
    API to traverse the category hierarchy.
*/

using System;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using eBay.SDK;
using eBay.SDK.API;
using eBay.SDK.Model;
 
namespace com.lothlorien.ebaysdkbook
{
	public class CategorySample : System.Windows.Forms.Form
	{
		// eBay Session
		IApiSession apiSession;
		// UI components
		private System.Windows.Forms.Button config;
		private System.Windows.Forms.Label status;
		private ConfigDialog configDialog;
		private System.ComponentModel.Container components = null;

		// This application's fields.
		private System.Windows.Forms.TreeView tree;

		private delegate void 
				DelegateAddRootNodes( ICategoryCollection categories );
		private DelegateAddRootNodes AddRootNodesFunc;

		public CategorySample()
		{
			Thread loadRootNodesThread;
			
			// Init our components, including the eBay one.
			InitializeComponent();
			InitializeEBayComponent();
			// Set up the tree's event handlers
			this.tree.BeforeExpand += 
				new TreeViewCancelEventHandler( this.tree_BeforeExpand );
			
			AddRootNodesFunc = 
					new DelegateAddRootNodes( AddRootNodes );		
			loadRootNodesThread = 
					new Thread( new ThreadStart( LoadRootNodes ) );
			loadRootNodesThread.Name = "eBay Categories Thread";
			loadRootNodesThread.Start();
			// Get the root nodes
		}

		private void LoadRootNodes()
		{
			ICategoryCollection categories;
			categories = RootNodes();
			this.Invoke( AddRootNodesFunc, 
						 new object[] { categories } );
		}

		private ICategoryCollection RootNodes()
		{

			Cursor.Current = Cursors.WaitCursor;
			status.Text = "Downloading...";

			return GetCategories( 0, 2 );
		}

		private void AddRootNodes( ICategoryCollection categories )
		{
			TreeNode node;

			if ( categories != null )
			{
				tree.BeginUpdate();
				foreach( ICategory category in categories )
				{
					if ( category.CategoryName != "" && 
						category.CategoryId == category.CategoryParentId ) 
					{
						// This is a toplevel node.
						node = new TreeNode( category.CategoryName );
						node.Tag = category.CategoryId;
						tree.Nodes.Add( node );
						foreach( ICategory subcategory in categories )
						{
							if ( subcategory.CategoryName != "" &&
						subcategory.CategoryId != subcategory.CategoryParentId && 
						subcategory.CategoryParentId == category.CategoryId ) 
							{
								// This node is a child of the current parent node.
								TreeNode child = 
									new TreeNode( subcategory.CategoryName );
								child.Tag = subcategory.CategoryId;
								node.Nodes.Add( child );
							}
						}			
					}
				}
				tree.EndUpdate();
				status.Text = "";
				status.Refresh();
				Cursor.Current = Cursors.Default;
			}
		}


		
		private ICategoryCollection GetCategories( int parent, 
												   int level )	
		{
			ICategoryCollection categories = null;
			GetCategoriesCall	getCategoriesCall;

			// Set up the API we'll use
			getCategoriesCall = 
				new GetCategoriesCall( apiSession );
			getCategoriesCall.ErrorLevel = 
				ErrorLevelEnum.BothShortAndLongErrorStrings;
		    getCategoriesCall.CompatibilityLevel = 319;
			getCategoriesCall.CategoryParent = parent;
			getCategoriesCall.DetailLevel = 1;
			getCategoriesCall.LevelLimit = level;
			try
			{
				categories = getCategoriesCall.GetCategories();
			}
			catch( APIException e )
			{
				MessageBox.Show( this, 
								 "APIException - GetCategories call failed: " + 
								 e.Message, "Error" );
			}
			catch( Exception e )
			{
				MessageBox.Show( this, 
								 "Exception - GetCategories call failed: " + 
								 e.Message, "Error" );
			}
			return categories;
		}

		private void tree_BeforeExpand( object sender, 
										TreeViewCancelEventArgs e)
		{
			ICategoryCollection subcategories;
			int baseLevel = -1;

			Cursor.Current = Cursors.WaitCursor;
			status.Text = "Downloading...";
			status.Refresh();
			subcategories = GetCategories( (int)e.Node.Tag, 3 );
			
			tree.BeginUpdate();
			foreach( ICategory subcategory in subcategories )
			{
				// Find the appropriate node to contain this subcategory.
				if ( baseLevel == -1 ) 
						baseLevel = subcategory.CategoryLevel;
				if ( subcategory.CategoryLevel == baseLevel + 2 ) 
				{
					foreach( TreeNode child in e.Node.Nodes )
					{
						if ( (int)child.Tag == subcategory.CategoryParentId )
						{
							TreeNode newChild = 
								new TreeNode( subcategory.CategoryName );
							newChild.Tag = subcategory.CategoryId;
							child.Nodes.Add( newChild );
						}
					}
				}
			}
			tree.EndUpdate();
			status.Text = "";
			status.Refresh();
			Cursor.Current = Cursors.Default;
		}

		private void InitializeEBayComponent( )
		{
			// Create the API session
			apiSession = new ApiSession();
			apiSession.LogCallXml = true;
			apiSession.Log = new LogFile( );
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
					apiSession.Log.Close();
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
			this.tree = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// config
			// 
			this.config.Location = new System.Drawing.Point(266, 296);
			this.config.Name = "config";
			this.config.Size = new System.Drawing.Size(96, 29);
			this.config.TabIndex = 0;
			this.config.Text = "Settings";
			this.config.Click += new System.EventHandler(this.config_Click);
			// 
			// status
			// 
			this.status.Location = new System.Drawing.Point(13, 302);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(235, 30);
			this.status.TabIndex = 1;
			this.status.Text = "Downloading...";
			// 
			// tree
			// 
			this.tree.ImageIndex = -1;
			this.tree.Location = new System.Drawing.Point(10, 10);
			this.tree.Name = "tree";
			this.tree.PathSeparator = ".";
			this.tree.SelectedImageIndex = -1;
			this.tree.Size = new System.Drawing.Size(348, 276);
			this.tree.TabIndex = 2;
			// 
			// CategorySample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(373, 328);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.tree,
																		  this.status,
																		  this.config});
			this.Name = "CategorySample";
			this.Text = "Categories";
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new CategorySample());
		}

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
					new FileStream( 
						"c:\\Documents and Settings\\All Users\\ebayrc", 
						System.IO.FileMode.Open );
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
				MessageBox.Show( this, 
					"Please set your eBay Developer Keys by pressing " +
					"'Settings' and entering your keys in the dialog box.", 
					"Error" );
			}
		}
	}
}

