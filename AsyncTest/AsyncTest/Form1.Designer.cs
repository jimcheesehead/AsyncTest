namespace AsyncTest
{
    partial class Form1
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
        private void InitializeComponent()
        {
            this.txtSrcInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblPct = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnInpBrowse = new System.Windows.Forms.Button();
            this.btnDstBrowse = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.txtDstInput = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSrcInput
            // 
            this.txtSrcInput.Location = new System.Drawing.Point(136, 3);
            this.txtSrcInput.Name = "txtSrcInput";
            this.txtSrcInput.Size = new System.Drawing.Size(382, 20);
            this.txtSrcInput.TabIndex = 0;
            this.txtSrcInput.Text = "L:\\Pictures\\Public\\Model Shoots\\Skips Studio\\Christina Banks";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Source Directory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Destination Directory";
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(136, 85);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 4;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.ProgressBar,
            this.lblPct});
            this.statusStrip1.Location = new System.Drawing.Point(0, 239);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(608, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 17);
            this.lblStatus.Text = "Ready";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // lblPct
            // 
            this.lblPct.Name = "lblPct";
            this.lblPct.Size = new System.Drawing.Size(118, 17);
            this.lblPct.Text = "toolStripStatusLabel2";
            // 
            // btnInpBrowse
            // 
            this.btnInpBrowse.Location = new System.Drawing.Point(524, 1);
            this.btnInpBrowse.Name = "btnInpBrowse";
            this.btnInpBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnInpBrowse.TabIndex = 6;
            this.btnInpBrowse.Text = "Browse";
            this.btnInpBrowse.UseVisualStyleBackColor = true;
            this.btnInpBrowse.Click += new System.EventHandler(this.btnInpBrowse_Click);
            // 
            // btnDstBrowse
            // 
            this.btnDstBrowse.Location = new System.Drawing.Point(524, 27);
            this.btnDstBrowse.Name = "btnDstBrowse";
            this.btnDstBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnDstBrowse.TabIndex = 7;
            this.btnDstBrowse.Text = "Browse";
            this.btnDstBrowse.UseVisualStyleBackColor = true;
            this.btnDstBrowse.Click += new System.EventHandler(this.btnDstBrowse_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // txtDstInput
            // 
            this.txtDstInput.Location = new System.Drawing.Point(136, 27);
            this.txtDstInput.Name = "txtDstInput";
            this.txtDstInput.Size = new System.Drawing.Size(382, 20);
            this.txtDstInput.TabIndex = 8;
            this.txtDstInput.Text = "M:\\tmp";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 261);
            this.Controls.Add(this.txtDstInput);
            this.Controls.Add(this.btnDstBrowse);
            this.Controls.Add(this.btnInpBrowse);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSrcInput);
            this.Name = "Form1";
            this.Text = "Asynchronous File Copy";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSrcInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel lblPct;
        private System.Windows.Forms.Button btnInpBrowse;
        private System.Windows.Forms.Button btnDstBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox txtDstInput;
    }
}

