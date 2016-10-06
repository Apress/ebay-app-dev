using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using eBay.SDK;
using eBay.SDK.Integration;

namespace com.lothlorien.ebaysdkbook
{
	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public class LoginForm : System.Windows.Forms.Form
	{
		ArrayList userID;
		ArrayList userPassword;
		ArrayList userEmail;
		ArrayList userAppId;
	
		private SampleApp app;
		public SampleApp App
		{
			get 
			{
				return this.app;
			}
			set 
			{
				this.app = value;
			}
		}

		#region User Interface Elements
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox userlist;
		private System.Windows.Forms.TextBox username;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox email;
		private System.Windows.Forms.TextBox password;
		private System.Windows.Forms.Button login;
		#endregion


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LoginForm( SampleApp owner )
		{
			int i;

			// Link us to the app.
			this.app = owner;

			// Required to initialize the form
			InitializeComponent();

			// Set up the user cache and user list.
			LoadUsers();
			userlist.Items.Clear();
			for ( i = 0; i < userID.Count; i++ )
				userlist.Items.Add( userID[i].ToString() );
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
			this.userlist = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.username = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.email = new System.Windows.Forms.TextBox();
			this.password = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.login = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// userlist
			// 
			this.userlist.Location = new System.Drawing.Point(8, 8);
			this.userlist.Name = "userlist";
			this.userlist.Size = new System.Drawing.Size(120, 108);
			this.userlist.TabIndex = 0;
			this.userlist.SelectedIndexChanged += new System.EventHandler(this.userlist_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(136, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "username:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// username
			// 
			this.username.Location = new System.Drawing.Point(200, 8);
			this.username.Name = "username";
			this.username.Size = new System.Drawing.Size(136, 20);
			this.username.TabIndex = 2;
			this.username.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(136, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "email:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// email
			// 
			this.email.Location = new System.Drawing.Point(200, 32);
			this.email.Name = "email";
			this.email.Size = new System.Drawing.Size(136, 20);
			this.email.TabIndex = 4;
			this.email.Text = "";
			// 
			// password
			// 
			this.password.Location = new System.Drawing.Point(200, 56);
			this.password.Name = "password";
			this.password.Size = new System.Drawing.Size(136, 20);
			this.password.TabIndex = 6;
			this.password.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(136, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 16);
			this.label3.TabIndex = 5;
			this.label3.Text = "password:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// login
			// 
			this.login.Location = new System.Drawing.Point(256, 96);
			this.login.Name = "login";
			this.login.TabIndex = 7;
			this.login.Text = "Login";
			this.login.Click += new System.EventHandler(this.login_Click);
			// 
			// LoginForm
			// 
			this.AcceptButton = this.login;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(344, 134);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.login,
																		  this.password,
																		  this.label3,
																		  this.email,
																		  this.label2,
																		  this.username,
																		  this.label1,
																		  this.userlist});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Login";
			this.ResumeLayout(false);

		}
		#endregion

		private void LoadUsers()
		{
			FileStream stream;
			StreamReader streamreader;
			String users; 
			String [] lines;
			String [] fields;
			Char [] eol = {'\n'};
			Char [] colon = {':'};
			int i;
			
			userID = new ArrayList();
			userPassword = new ArrayList();
			userEmail = new ArrayList();
			userAppId = new ArrayList();

			try
			{
				stream = 
					new FileStream( "c:\\Documents and Settings\\All Users\\integrationusers", System.IO.FileMode.Open );
				streamreader = new StreamReader( stream );
				users = streamreader.ReadToEnd();
				streamreader.Close();
				stream.Close();

				lines = users.Split( eol );
				for( i = 0; i < lines.Length; i++ )
				{
					fields = lines[i].Split( colon );
					if ( fields.Length == 5 )
					{
						userID.Add( fields[0] );
						userPassword.Add( fields[1] );
						userEmail.Add( fields[3] );
						userAppId.Add( fields[2] );
					}
				}
			}
			catch
			{

			}
		}

		private void SaveUsers()
		{
			try
			{
				FileStream stream = File.OpenWrite( "c:\\Documents and Settings\\All Users\\integrationusers" );
				StreamWriter streamWriter = new StreamWriter( stream );
				int i;

				for( i = 0; i < userID.Count; i++ )
				{
					streamWriter.Write( userID[i] );
					streamWriter.Write( ":" );
					streamWriter.Write( userPassword[i] );
					streamWriter.Write( ":" );
					streamWriter.Write( userAppId[i].ToString() );
					streamWriter.Write( ":" );
					streamWriter.Write( userEmail[i] );
					streamWriter.Write( ":" );
					streamWriter.Write( "\n" );
				}
				streamWriter.Flush();
				streamWriter.Close();
				stream.Close();
			}
			catch
			{
				MessageBox.Show( this, "Could not save users.", "Error" );
			}
		}

		private void login_Click(object sender, System.EventArgs e)
		{
			int i;
			IAppUser appUser;
			bool found = false;

			if ( username.Text == null || username.Text.Length == 0 ||
				 email.Text == null || email.Text.Length == 0 ||
				 password.Text == null || password.Text.Length == 0 )
				return;

			// First, if the item's not in the user array, add it.
			for ( i = 0; i < userID.Count; i++ )
			{
				if ( userID[i].ToString() == username.Text )
				{
					found = true;
					break;
				}
			}
			if ( found )
			{
				appUser = App.LoadUser( userAppId[ i ].ToString() );
			}
			else
			{
				appUser = App.NewUser( username.Text, password.Text, email.Text );
				userID.Add( username.Text );
				userPassword.Add( password.Text );
				userEmail.Add( email.Text );
				userAppId.Add( appUser.AppUserId.ToString() );
				SaveUsers();
			}

			// Next, set the application's user ID field.
			App.AppUser = appUser;

			// Finally, close us.
			this.Close();
			// And tell the user interface to log into eBay.
			App.Login( true );
		}


		private void userlist_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int i;
			string item = userlist.SelectedItem.ToString();
			if ( userlist.SelectedIndex < userID.Count )
			{
				for ( i = 0; i < userID.Count; i++ )
				{
					{
						username.Text = userID[i].ToString();
						email.Text = userEmail[i].ToString();
						// Remove this line if you don't want to auto-provide passwords
						password.Text = userPassword[i].ToString();
						break;
					}
				}
			}
		}
	}
}
