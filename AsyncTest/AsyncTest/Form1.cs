using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncTest
{
    public partial class Form1 : Form
    {
        string srcPath, dstPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "Ready";
            ProgressBar.Visible = false;
            lblPct.Visible = false;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {

        }

        private void btnInpBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtSrcInput.Text = srcPath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnDstBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtSrcInput.Text = srcPath = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
