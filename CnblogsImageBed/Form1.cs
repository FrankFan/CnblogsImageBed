using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CnblogsImageBed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.pnlLogin.Visible = false;
            this.pnlUpload.Visible = true;
            this.pnlUpload.Location = new System.Drawing.Point(10, 10);
        }
    }
}
