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
            this.labelTopR = new System.Windows.Forms.Label();
            this.labelTopL = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelTopR);
            this.panel1.Controls.Add(this.labelTopL);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1067, 623);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // labelTopR
            // 
            this.labelTopR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTopR.BackColor = System.Drawing.SystemColors.Window;
            this.labelTopR.Location = new System.Drawing.Point(705, 0);
            this.labelTopR.Name = "labelTopR";
            this.labelTopR.Size = new System.Drawing.Size(359, 18);
            this.labelTopR.TabIndex = 3;
            this.labelTopR.Text = "label2";
            this.labelTopR.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelTopL
            // 
            this.labelTopL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTopL.BackColor = System.Drawing.SystemColors.Window;
            this.labelTopL.Location = new System.Drawing.Point(3, 0);
            this.labelTopL.Name = "labelTopL";
            this.labelTopL.Size = new System.Drawing.Size(426, 18);
            this.labelTopL.TabIndex = 2;
            this.labelTopL.Text = "label1";
            // 
            // FormBigPlot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(19F, 45F);
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
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormBigPlot_Load);
            this.Shown += new System.EventHandler(this.FormBigPlot_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormBigPlot_KeyDown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelTopR;
        private System.Windows.Forms.Label labelTopL;
    }
}