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
using FileShortcutHelper;

namespace AsyncTest
{
    public partial class Form1 : Form
    {
        string srcPath, dstPath;
        int totalDirs;
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
            srcPath = txtSrcInput.Text.TrimEnd(new[] { '\\', '/' });
            dstPath = txtDstInput.Text.TrimEnd(new[] { '\\', '/' });

            // The source path must be vaild and exist. The source and destination paths cannot
            // be the same.
            if (checkPathErrors())
            {
                return;
            }

            // If destination directory does not exist ask to create it
            if (!Directory.Exists(dstPath))
            {
                DialogResult dialogResult = MessageBox.Show(
                   String.Format("Destination directory {0} doesn\'t exist\nCreate it?", dstPath),
                   "Warning!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                    return;

                // Create the directory
                try
                {
                    Directory.CreateDirectory(dstPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                    return;
                }
            }

            //fileCount = 0;

            //var dirs = Directory.EnumerateDirectories(srcPath);
            //var files = Directory.EnumerateFiles(srcPath);
            //totalDirs = dirs.Count();
            //totalFiles = files.Count();

            //lblStatus.Text = "Copying " + totalFiles.ToString() + " files";
            //ProgressBar.Visible = true;
            //lblPct.Visible = true;

            //backgroundWorker1.RunWorkerAsync();

            //// Single level copy (does not traverse subdirectories)

            //foreach (string filename in files)
            //{
            //    string path = filename;

            //    if (chkBoxDereferenceLinks.Checked && ShortcutHelper.IsShortcut(filename))
            //    {
            //        path = ShortcutHelper.ResolveShortcut(filename);
            //        if (!File.Exists(path)) // Ignore bad link
            //            continue;
            //    }

            //    using (FileStream SourceStream = File.Open(path, FileMode.Open))
            //    {
            //        using (FileStream DestinationStream = File.Create(dstPath + path.Substring(path.LastIndexOf('\\'))))
            //        {
            //            await SourceStream.CopyToAsync(DestinationStream);
            //            fileCount++;
            //        }
            //    }
            //}

            //// Now copy the subdirectories recursively
            //foreach (string path in dirs)
            //{
            //}

            await DirectoryCopy(srcPath, dstPath);
            Task.WaitAll();

            MessageBox.Show("Done!");

            lblStatus.Text = "Ready";
            ProgressBar.Visible = false;
            lblPct.Visible = false;
        }

        private async Task DirectoryCopy(string srcPath, string dstPath)
        {
            fileCount = 0;

            var dirs = Directory.EnumerateDirectories(srcPath);
            var files = Directory.EnumerateFiles(srcPath);
            totalDirs = dirs.Count();
            totalFiles = files.Count();

            lblStatus.Text = "Copying " + totalFiles.ToString() + " files";
            ProgressBar.Visible = true;
            lblPct.Visible = true;

            backgroundWorker1.RunWorkerAsync();

            // Single level copy (does not traverse subdirectories)

            foreach (string filename in files)
            {
                string path = filename;

                if (chkBoxDereferenceLinks.Checked && ShortcutHelper.IsShortcut(filename))
                {
                    path = ShortcutHelper.ResolveShortcut(filename);
                    if (!File.Exists(path)) // Ignore bad link
                        continue;
                }

                using (FileStream SourceStream = File.Open(path, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(dstPath + path.Substring(path.LastIndexOf('\\'))))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                        fileCount++;
                    }
                }
            }

            // Now copy the subdirectories recursively
            foreach (string path in dirs)
            {
            }
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

        private bool checkPathErrors()
        {
            string errMsg;

            if (string.IsNullOrEmpty(srcPath))
            {
                errMsg = "Source path not specified";
                txtSrcInput.Focus();
            }
            else if (!Directory.Exists(srcPath))
            {
                errMsg = "Soruce path is not a vaild directory";
                txtSrcInput.Focus();
            }
            else if (string.IsNullOrEmpty(dstPath))
            {
                errMsg = "Destination path not specified";
                txtDstInput.Focus();
            }
            //else if (!Directory.Exists(dstPath))
            //{
            //    errMsg = "Destination path is not a vaild directory";
            //    txtDstInput.Focus();
            //}
            else if (srcPath == dstPath)
            {
                errMsg = "Source path and destination path are the same";
                txtDstInput.Focus();
            }
            else
            {
                return false; // No errors
            }

            MessageBox.Show(errMsg);
            return true;
        }

    }
}
