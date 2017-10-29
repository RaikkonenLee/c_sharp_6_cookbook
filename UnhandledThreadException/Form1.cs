using System;
using System.Windows.Forms;
using System.Threading;

namespace UnhandledThreadException
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        void GoBoom()
        {
            throw new Exception("BOOM!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // throw exception directly from event handler
            //GoBoom();
            
            // throw exception from secondary thread
            Thread t = new Thread(GoBoom);
            t.Start();
        }
    }
}