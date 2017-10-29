namespace PartialClassAddIn
{
	partial class frmInputs
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				components?.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblClassName = new System.Windows.Forms.Label();
			this.lblAttribs = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtClassName = new System.Windows.Forms.TextBox();
			this.lbAttribs = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// lblDescription
			// 
			this.lblDescription.AutoSize = true;
			this.lblDescription.Location = new System.Drawing.Point(12, 12);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(213, 14);
			this.lblDescription.TabIndex = 1;
			this.lblDescription.Text = "Create a class with attributes built right in!";
			// 
			// lblClassName
			// 
			this.lblClassName.AutoSize = true;
			this.lblClassName.Location = new System.Drawing.Point(13, 52);
			this.lblClassName.Name = "lblClassName";
			this.lblClassName.Size = new System.Drawing.Size(188, 14);
			this.lblClassName.TabIndex = 2;
			this.lblClassName.Text = "Enter the name of the class to create";
			// 
			// lblAttribs
			// 
			this.lblAttribs.AutoSize = true;
			this.lblAttribs.Location = new System.Drawing.Point(13, 97);
			this.lblAttribs.Name = "lblAttribs";
			this.lblAttribs.Size = new System.Drawing.Size(205, 14);
			this.lblAttribs.TabIndex = 3;
			this.lblAttribs.Text = "Select the attributes to add to your class";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(269, 7);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(269, 36);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// txtClassName
			// 
			this.txtClassName.Location = new System.Drawing.Point(13, 71);
			this.txtClassName.Name = "txtClassName";
			this.txtClassName.Size = new System.Drawing.Size(238, 20);
			this.txtClassName.TabIndex = 6;
			// 
			// lbAttribs
			// 
			this.lbAttribs.FormattingEnabled = true;
			this.lbAttribs.Location = new System.Drawing.Point(12, 117);
			this.lbAttribs.Name = "lbAttribs";
			this.lbAttribs.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbAttribs.Size = new System.Drawing.Size(327, 316);
			this.lbAttribs.Sorted = true;
			this.lbAttribs.TabIndex = 7;
			// 
			// frmInputs
			// 
			this.ClientSize = new System.Drawing.Size(351, 450);
			this.Controls.Add(this.lbAttribs);
			this.Controls.Add(this.txtClassName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lblAttribs);
			this.Controls.Add(this.lblClassName);
			this.Controls.Add(this.lblDescription);
			this.Name = "frmInputs";
			this.Text = "Attributed Class Wizard";
			this.Load += new System.EventHandler(this.frmInputs_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Label lblClassName;
		private System.Windows.Forms.Label lblAttribs;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtClassName;
		private System.Windows.Forms.ListBox lbAttribs;
	}
}