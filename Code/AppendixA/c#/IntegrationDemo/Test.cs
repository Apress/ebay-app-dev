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
	public class Test : com.lothlorien.ebaysdkbook.eBaySampleApplication
	{
		public Test()
		{
			System.Windows.Forms.Label label = new System.Windows.Forms.Label();
			System.Windows.Forms.Control  container = new System.Windows.Forms.Control();
			label.Location = new System.Drawing.Point(10, 10);
			label.Name = "Example";
			label.Size = new System.Drawing.Size(180, 24);
			label.Text = "Hello World";

			container.SuspendLayout();
			container.Controls.AddRange(new System.Windows.Forms.Control[] {label} );
			container.ResumeLayout(false);

			this.Gui = container;

			this.StatusText = "Nothing";
		}

		[STAThread]
		static void Main() 
		{
			Application.Run(new Test());
		}
	}
}
