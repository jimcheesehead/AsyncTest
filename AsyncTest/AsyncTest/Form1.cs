﻿using System;
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
        string text; // Temporary storage

        public static int FileCount { get; set; }

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


            totalDirs = 0;
            fileCount = 0;

            totalDirs = totalFiles = 0;

            DirOps.DirInfo info;
            DirOps.Options options = GetOptions();

            info = DirOps.GetDirInfo(srcPath, options); /*************************************************************************/
            totalFiles = info.totalFiles;

            // Show the the status of the background copying

            if (options.HasFlag(DirOps.Options.TopDirectoryOnly))
            {
                text = String.Format("Copying {0} files ({1})",
                    info.totalFiles, GetBytesReadable(info.totalBytes));
            }
            else
            {
                text = String.Format("Copying {0} files, {1} folders ({2})",
                    info.totalFiles, info.totalDirs, GetBytesReadable(info.totalBytes));
                if (info.badLinks.Count() > 0)
                    text += String.Format(" - {0} bad links", info.badLinks.Count());
            }
            lblStatus.Text = text;

            DirOps.DirInfo inf = DirOps.CountDirs(srcPath);
            text = String.Format("Contains {0} files, {1} folders", inf.totalFiles, inf.totalDirs);
            MessageBox.Show(text);

            ProgressBar.Visible = true;
            lblPct.Visible = true;

            backgroundWorker1.RunWorkerAsync();

            // Copy the source directory to the destination directory asynchronously
            info = await DirOps.AsyncDirectoryCopy(srcPath, dstPath, 
                progressCallback, options);
            Task.WaitAll(); // Is this needed?

            text = String.Format("Done!\nCopyied {0} files, in {1} folders ({2})",
                info.totalFiles, info.totalDirs, GetBytesReadable(info.totalBytes));
            if (info.badLinks.Count() > 0)
                text += String.Format(" - {0} bad links", info.badLinks.Count());

            MessageBox.Show(text);

            /***************************
             * Need to stop the background worker here if it's running
             */

            backgroundWorker1.CancelAsync();

            lblStatus.Text = "Ready";

            int endTotalFiles = fileCount;

            //ProgressBar.Visible = false;
            //lblPct.Visible = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Cancel Copy", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                backgroundWorker1.CancelAsync();
            }
        }

        private void progressCallback(DirOps.DirInfo obj)
        {
            fileCount = obj.totalFiles;
        }

        private int getDirFileCount(string srcPath) // NOT USED ANYMORE
        {
            var dirs = Directory.EnumerateDirectories(srcPath);
            var files = Directory.EnumerateFiles(srcPath);
            totalDirs += dirs.Count();
            totalFiles += files.Count();

            // Now do the subdirectories
            foreach (string path in dirs)
            {
                getDirFileCount(path);
            }

            return totalFiles;
        }

        private async Task DirectoryCopy(string srcPath, string dstPath)
        {
            var dirs = Directory.EnumerateDirectories(srcPath);
            var files = Directory.EnumerateFiles(srcPath);


            // Does not process linked subdirectories yet!

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
                string dirName = path.Substring(path.LastIndexOf('\\'));
                string fullDirName = dstPath + dirName;
                // string tmpPath = Path.Combine(dstPath, path.Substring(path.LastIndexOf('\\')));


                // If the subdirectory doesn't exist, create it.
                if (!Directory.Exists(fullDirName))
                {
                    Directory.CreateDirectory(fullDirName);
                }


                await DirectoryCopy(path, fullDirName);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int percentComplete, highestPercentageReached = 0;

            while (true)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

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
            string text;

            if ((e.Cancelled == true))
            {
                text = "Canceled!";
            }

            else if (!(e.Error == null))
            {
                text = ("Error: " + e.Error.Message);
            }

            else
            {
                text = "Done!";
            }

            //MessageBox.Show("Background Worker " + text);

            //ProgressBar.Value = 0;
        }

        DirOps.Options GetOptions()
        {
            DirOps.Options options = new DirOps.Options();

            options = DirOps.Options.None;
            if (chkBoxOverwrite.Checked)
            {
                options |= DirOps.Options.OverWriteFiles;
            }
            if (chkBoxTopDirOnly.Checked)
            {
                options |= DirOps.Options.TopDirectoryOnly;
            }
            return options;
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

        private string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }

    }
}
