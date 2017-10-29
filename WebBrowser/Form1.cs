using System;
using System.Windows.Forms;

namespace WebBrowser
{
    public partial class CheapoBrowser : Form
	{
		public CheapoBrowser()
		{
			InitializeComponent();
            // suppress script errors as we aren't debugging
            this._webBrowser.ScriptErrorsSuppressed = true;
		}

		private void NavigateToCurrentUri()
		{
			try
			{
				Uri uri = new Uri(this._txtAddress.Text);
				this._webBrowser.Navigate(uri);
			}
			catch (UriFormatException ufe)
			{
				MessageBox.Show(
                    $"Error during navigation to {_txtAddress.Text} : {ufe.Message}");
			}
		}

		private void CheapoBrowser_Load(object sender, EventArgs e)
		{
			NavigateToCurrentUri();
		}

		private void _btnBack_Click(object sender, EventArgs e)
		{
			if (this._webBrowser.CanGoBack)
			{
				this._webBrowser.GoBack();
			}
		}

		private void _btnForward_Click(object sender, EventArgs e)
		{
			if (this._webBrowser.CanGoForward)
			{
				this._webBrowser.GoForward();
			}
		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			this._webBrowser.Stop();
		}

		private void _btnGo_Click(object sender, EventArgs e)
		{
			NavigateToCurrentUri();
		}

		private void _txtAddress_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)13)
			{
				NavigateToCurrentUri();
				e.Handled = true;
			}
		}

		private void _btnHome_Click(object sender, EventArgs e)
		{
			this._webBrowser.GoHome();
		}

		private void _webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			// update with where we ended up in case of redirection 
			// from the original URI
			this._txtAddress.Text = e.Url.ToString();
			// set up the buttons if we can go back or forth
			this._btnBack.Enabled = this._webBrowser.CanGoBack;
			this._btnForward.Enabled = this._webBrowser.CanGoForward;
		}

		private void btnAddHTML_Click(object sender, EventArgs e)
		{
            this._txtAddress.Text = string.Empty;
            this._webBrowser.Document.Body.InnerHtml = "<h1>Hey you added some HTML!</h1>";
		}

		private void chkEnableContextBrowser_CheckedChanged(object sender, EventArgs e)
		{
			// should we show the context menu?
			this._webBrowser.IsWebBrowserContextMenuEnabled = chkEnableContextBrowser.Checked;
		}

		private void btnPrint_Click(object sender, EventArgs e)
		{
			// just print
			this._webBrowser.Print();
			// show page setup to modify printing settings
			this._webBrowser.ShowPageSetupDialog();
			// show the print dialog
			this._webBrowser.ShowPrintDialog();
			// show print preview
			this._webBrowser.ShowPrintPreviewDialog();
		}

		private void btnSaveAs_Click(object sender, EventArgs e)
		{
			this._webBrowser.ShowSaveAsDialog();
		}
	}
}