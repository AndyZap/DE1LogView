namespace DE1LogView
{
    partial class FormBigPlot
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBigPlot));
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTopL1 = new System.Windows.Forms.Label();
            this.splitBigPlot = new System.Windows.Forms.SplitContainer();
            this.richLog = new System.Windows.Forms.RichTextBox();
            this.labelTopR = new System.Windows.Forms.Label();
            this.labelTopL = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitBigPlot)).BeginInit();
            this.splitBigPlot.Panel2.SuspendLayout();
            this.splitBigPlot.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelTopL1);
            this.panel1.Controls.Add(this.splitBigPlot);
            this.panel1.Controls.Add(this.labelTopR);
            this.panel1.Controls.Add(this.labelTopL);
            this.panel1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1067, 623);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // labelTopL1
            // 
            this.labelTopL1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTopL1.BackColor = System.Drawing.SystemColors.Window;
            this.labelTopL1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTopL1.Location = new System.Drawing.Point(3, 24);
            this.labelTopL1.Name = "labelTopL1";
            this.labelTopL1.Size = new System.Drawing.Size(964, 18);
            this.labelTopL1.TabIndex = 6;
            // 
            // splitBigPlot
            // 
            this.splitBigPlot.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitBigPlot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitBigPlot.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitBigPlot.Location = new System.Drawing.Point(0, 359);
            this.splitBigPlot.Name = "splitBigPlot";
            // 
            // splitBigPlot.Panel2
            // 
            this.splitBigPlot.Panel2.Controls.Add(this.richLog);
            this.splitBigPlot.Size = new System.Drawing.Size(1067, 264);
            this.splitBigPlot.SplitterDistance = 507;
            this.splitBigPlot.SplitterWidth = 8;
            this.splitBigPlot.TabIndex = 5;
            // 
            // richLog
            // 
            this.richLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richLog.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richLog.Location = new System.Drawing.Point(0, 0);
            this.richLog.Name = "richLog";
            this.richLog.Size = new System.Drawing.Size(552, 264);
            this.richLog.TabIndex = 5;
            this.richLog.TabStop = false;
            this.richLog.Text = "";
            this.richLog.WordWrap = false;
            // 
            // labelTopR
            // 
            this.labelTopR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTopR.BackColor = System.Drawing.SystemColors.Window;
            this.labelTopR.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTopR.Location = new System.Drawing.Point(973, 0);
            this.labelTopR.Name = "labelTopR";
            this.labelTopR.Size = new System.Drawing.Size(91, 18);
            this.labelTopR.TabIndex = 3;
            this.labelTopR.Text = "label2";
            this.labelTopR.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelTopL
            // 
            this.labelTopL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTopL.BackColor = System.Drawing.SystemColors.Window;
            this.labelTopL.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTopL.Location = new System.Drawing.Point(3, 0);
            this.labelTopL.Name = "labelTopL";
            this.labelTopL.Size = new System.Drawing.Size(964, 18);
            this.labelTopL.TabIndex = 2;
            // 
            // FormBigPlot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 623);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBigPlot";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormBigPlot_KeyDown);
            this.panel1.ResumeLayout(false);
            this.splitBigPlot.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitBigPlot)).EndInit();
            this.splitBigPlot.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelTopR;
        private System.Windows.Forms.Label labelTopL;
        private System.Windows.Forms.SplitContainer splitBigPlot;
        private System.Windows.Forms.RichTextBox richLog;
        private System.Windows.Forms.Label labelTopL1;
    }
}