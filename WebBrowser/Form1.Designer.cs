namespace WebBrowser
{
	partial class CheapoBrowser
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
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private void InitializeComponent()
		{
            this._webBrowser = new System.Windows.Forms.WebBrowser();
            this._btnBack = new System.Windows.Forms.Button();
            this._btnForward = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._lblAddress = new System.Windows.Forms.Label();
            this._txtAddress = new System.Windows.Forms.TextBox();
            this._btnGo = new System.Windows.Forms.Button();
            this._btnHome = new System.Windows.Forms.Button();
            this.btnAddHTML = new System.Windows.Forms.Button();
            this.chkEnableContextBrowser = new System.Windows.Forms.CheckBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _webBrowser
            // 
            this._webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._webBrowser.Location = new System.Drawing.Point(0, 129);
            this._webBrowser.Name = "_webBrowser";
            this._webBrowser.Size = new System.Drawing.Size(504, 462);
            this._webBrowser.TabIndex = 14;
            this._webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this._webBrowser_Navigated);
            // 
            // _btnBack
            // 
            this._btnBack.Enabled = false;
            this._btnBack.Location = new System.Drawing.Point(0, 12);
            this._btnBack.Name = "_btnBack";
            this._btnBack.Size = new System.Drawing.Size(75, 23);
            this._btnBack.TabIndex = 1;
            this._btnBack.Text = "&Back";
            this._btnBack.Click += new System.EventHandler(this._btnBack_Click);
            // 
            // _btnForward
            // 
            this._btnForward.Enabled = false;
            this._btnForward.Location = new System.Drawing.Point(81, 12);
            this._btnForward.Name = "_btnForward";
            this._btnForward.Size = new System.Drawing.Size(75, 23);
            this._btnForward.TabIndex = 2;
            this._btnForward.Text = "&Forward";
            this._btnForward.Click += new System.EventHandler(this._btnForward_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.Location = new System.Drawing.Point(260, 12);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 3;
            this._btnCancel.Text = "&Cancel";
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // _lblAddress
            // 
            this._lblAddress.AutoSize = true;
            this._lblAddress.Location = new System.Drawing.Point(0, 106);
            this._lblAddress.Name = "_lblAddress";
            this._lblAddress.Size = new System.Drawing.Size(48, 13);
            this._lblAddress.TabIndex = 4;
            this._lblAddress.Text = "Address:";
            // 
            // _txtAddress
            // 
            this._txtAddress.Location = new System.Drawing.Point(45, 103);
            this._txtAddress.Name = "_txtAddress";
            this._txtAddress.Size = new System.Drawing.Size(390, 20);
            this._txtAddress.TabIndex = 5;
            this._txtAddress.Text = "http://shop.oreilly.com/product/0636920037347.do";
            this._txtAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._txtAddress_KeyPress);
            // 
            // _btnGo
            // 
            this._btnGo.Location = new System.Drawing.Point(441, 103);
            this._btnGo.Name = "_btnGo";
            this._btnGo.Size = new System.Drawing.Size(51, 23);
            this._btnGo.TabIndex = 6;
            this._btnGo.Text = "&Go";
            this._btnGo.Click += new System.EventHandler(this._btnGo_Click);
            // 
            // _btnHome
            // 
            this._btnHome.Location = new System.Drawing.Point(162, 12);
            this._btnHome.Name = "_btnHome";
            this._btnHome.Size = new System.Drawing.Size(75, 23);
            this._btnHome.TabIndex = 8;
            this._btnHome.Text = "&Home";
            this._btnHome.Click += new System.EventHandler(this._btnHome_Click);
            // 
            // btnAddHTML
            // 
            this.btnAddHTML.Location = new System.Drawing.Point(357, 12);
            this.btnAddHTML.Name = "btnAddHTML";
            this.btnAddHTML.Size = new System.Drawing.Size(75, 23);
            this.btnAddHTML.TabIndex = 10;
            this.btnAddHTML.Text = "&Add HTML";
            this.btnAddHTML.Click += new System.EventHandler(this.btnAddHTML_Click);
            // 
            // chkEnableContextBrowser
            // 
            this.chkEnableContextBrowser.AutoSize = true;
            this.chkEnableContextBrowser.Checked = true;
            this.chkEnableContextBrowser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableContextBrowser.Location = new System.Drawing.Point(12, 54);
            this.chkEnableContextBrowser.Name = "chkEnableContextBrowser";
            this.chkEnableContextBrowser.Size = new System.Drawing.Size(177, 17);
            this.chkEnableContextBrowser.TabIndex = 11;
            this.chkEnableContextBrowser.Text = "&Enable context menu in browser";
            this.chkEnableContextBrowser.CheckedChanged += new System.EventHandler(this.chkEnableContextBrowser_CheckedChanged);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(204, 54);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 12;
            this.btnPrint.Text = "&Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Location = new System.Drawing.Point(298, 54);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAs.TabIndex = 13;
            this.btnSaveAs.Text = "&Save As";
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // CheapoBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 591);
            this.Controls.Add(this.btnSaveAs);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.chkEnableContextBrowser);
            this.Controls.Add(this.btnAddHTML);
            this.Controls.Add(this._btnHome);
            this.Controls.Add(this._btnGo);
            this.Controls.Add(this._txtAddress);
            this.Controls.Add(this._lblAddress);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnForward);
            this.Controls.Add(this._btnBack);
            this.Controls.Add(this._webBrowser);
            this.Name = "CheapoBrowser";
            this.Text = "Cheapo-Browser";
            this.Load += new System.EventHandler(this.CheapoBrowser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.WebBrowser _webBrowser;
		private System.Windows.Forms.Button _btnBack;
		private System.Windows.Forms.Button _btnForward;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Label _lblAddress;
		private System.Windows.Forms.TextBox _txtAddress;
		private System.Windows.Forms.Button _btnGo;
		private System.Windows.Forms.Button _btnHome;
		private System.Windows.Forms.Button btnAddHTML;
		private System.Windows.Forms.CheckBox chkEnableContextBrowser;
		private System.Windows.Forms.Button btnPrint;
		private System.Windows.Forms.Button btnSaveAs;
	}
}

