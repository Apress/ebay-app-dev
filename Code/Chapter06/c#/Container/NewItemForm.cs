using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using eBay.SDK;
using eBay.SDK.Model;
using eBay.SDK.Model.Item;
using eBay.SDK.Integration;

namespace com.lothlorien.ebaysdkbook
{
	/// <summary>
	/// Summary description for NewItemForm.
	/// </summary>
	public class NewItemForm : System.Windows.Forms.Form
	{
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


		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox title;
		private System.Windows.Forms.TextBox openingPrice;
		private System.Windows.Forms.RichTextBox description;
		private System.Windows.Forms.TextBox category;
		private System.Windows.Forms.Button list;
		private System.Windows.Forms.Button cancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NewItemForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.title = new System.Windows.Forms.TextBox();
			this.openingPrice = new System.Windows.Forms.TextBox();
			this.description = new System.Windows.Forms.RichTextBox();
			this.list = new System.Windows.Forms.Button();
			this.category = new System.Windows.Forms.TextBox();
			this.cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Title";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 104);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Description";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 16);
			this.label4.TabIndex = 3;
			this.label4.Text = "Category";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(96, 16);
			this.label5.TabIndex = 4;
			this.label5.Text = "Opening Price";
			// 
			// title
			// 
			this.title.Location = new System.Drawing.Point(104, 8);
			this.title.Name = "title";
			this.title.Size = new System.Drawing.Size(184, 20);
			this.title.TabIndex = 1;
			this.title.Text = "";
			// 
			// openingPrice
			// 
			this.openingPrice.Location = new System.Drawing.Point(104, 80);
			this.openingPrice.Name = "openingPrice";
			this.openingPrice.Size = new System.Drawing.Size(184, 20);
			this.openingPrice.TabIndex = 3;
			this.openingPrice.Text = "";
			// 
			// description
			// 
			this.description.Location = new System.Drawing.Point(16, 120);
			this.description.Name = "description";
			this.description.Size = new System.Drawing.Size(272, 96);
			this.description.TabIndex = 4;
			this.description.Text = "";
			// 
			// list
			// 
			this.list.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.list.Location = new System.Drawing.Point(128, 232);
			this.list.Name = "list";
			this.list.TabIndex = 0;
			this.list.Text = "List";
			this.list.Click += new System.EventHandler(this.list_Click);
			// 
			// category
			// 
			this.category.Location = new System.Drawing.Point(104, 40);
			this.category.Name = "category";
			this.category.Size = new System.Drawing.Size(184, 20);
			this.category.TabIndex = 2;
			this.category.Text = "";
			// 
			// cancel
			// 
			this.cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.cancel.Location = new System.Drawing.Point(216, 232);
			this.cancel.Name = "cancel";
			this.cancel.TabIndex = 5;
			this.cancel.Text = "Cancel";
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// NewItemForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(304, 266);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.cancel,
																		  this.category,
																		  this.list,
																		  this.description,
																		  this.openingPrice,
																		  this.title,
																		  this.label5,
																		  this.label4,
																		  this.label2,
																		  this.label1});
			this.Name = "NewItemForm";
			this.Text = "New Item";
			this.ResumeLayout(false);

		}
		#endregion

		private void NewItem()
		{
			IItem item = new Item();
			ItemAppData appData = new ItemAppData();

			appData.AppUserId = App.AppUser.AppUserId;
			item.AppData = appData;

			// Always, always set the status to PendingAdd or Save!
			appData.AppStatus = AppStatusEnum.PendingAdd;
			
			// Now populate its fields.
			item.SiteId = SiteIdEnum.US;
			item.Type = ItemTypes.Auction;
			item.Title = title.Text;
			item.Description = description.Text;
			item.Currency = CurrencyEnum.USDollar;
			item.Location = "Santa Cruz, CA";
			item.Country = "us";

			item.CategoryId = int.Parse( category.Text );

			item.Quantity = 1;
			item.Duration = 5;

			item.MinimumToBid = decimal.Parse( openingPrice.Text );
			item.ReservePrice = 0;
			item.PaymentTerms.SeeDescription = true;
			item.ShippingOptions.ShippingRange = ShippingRangeEnum.SiteOnly;
			item.ShippingOptions.ShippingPayment.SeeDescription = true;
			item.Uuid = new Uuid(true) ;
			
			App.Session.SaveItem( item );
			App.UpdateItemView( );
		}

		private void list_Click(object sender, System.EventArgs e)
		{
			NewItem();
			Close();
		}

		private void cancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

	}
}
