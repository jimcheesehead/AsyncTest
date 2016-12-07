using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Threading;

namespace AsyncTest
{
    public partial class Form1 : Form
    {
        string srcPath, dstPath;
        int fileCount, totalFiles;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            srcPath = txtSrcInput.Text;
            dstPath = txtDstInput.Text;

            lblStatus.Text = "Ready";
            ProgressBar.Visible = false;
            lblPct.Visible = false;
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
                txtDstInput.Text = dstPath = folderBrowserDialog1.SelectedPath;
            }
        }

        private async void btnCopy_Click(object sender, EventArgs e)
        {
            fileCount = 0;
            var files = Directory.EnumerateFiles(srcPath);
            totalFiles = files.Count();

            lblStatus.Text = "Copying " + totalFiles.ToString() + " files";
            ProgressBar.Visible = true;
            lblPct.Visible = true;

            backgroundWorker1.RunWorkerAsync();

            foreach (string filename in files)
            {
                using (FileStream SourceStream = File.Open(filename, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(dstPath + filename.Substring(filename.LastIndexOf('\\'))))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                    }
                }
                fileCount++;
            }

            MessageBox.Show("Done!");

            lblStatus.Text = "Ready";
            ProgressBar.Visible = false;
            lblPct.Visible = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int percentComplete, highestPercentageReached = 0;

            while (true)
            {
                percentComplete = (int)((float)fileCount / (float)totalFiles * 100);
                if (percentComplete >= 100)
                    break;

                if (percentComplete > highestPercentageReached)
                {
                    highestPercentageReached = percentComplete;
                    backgroundWorker1.ReportProgress(percentComplete);
                }
                Thread.Sleep(100); // Wait 100 milliseconds;
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            ProgressBar.Value = e.ProgressPercentage;
            // Show percentage complete
            lblPct.Text = e.ProgressPercentage.ToString() + "% complete";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //ProgressBar.Value = 0;
        }

    }
}
