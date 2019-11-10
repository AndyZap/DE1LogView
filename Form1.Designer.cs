namespace DE1LogView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.labelTopR = new System.Windows.Forms.Label();
            this.labelTopL = new System.Windows.Forms.Label();
            this.labelBotR = new System.Windows.Forms.Label();
            this.labelBotL = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.radioFlow = new System.Windows.Forms.RadioButton();
            this.radioWeight = new System.Windows.Forms.RadioButton();
            this.txtCopy = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnSaveNotes = new System.Windows.Forms.Button();
            this.txtFilterName = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.labNotes = new System.Windows.Forms.Label();
            this.listData = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labProfile = new System.Windows.Forms.Label();
            this.labTime = new System.Windows.Forms.Label();
            this.labGrind = new System.Windows.Forms.Label();
            this.labRatio = new System.Windows.Forms.Label();
            this.labCoffeeWeight = new System.Windows.Forms.Label();
            this.labBeanWeight = new System.Windows.Forms.Label();
            this.labDate = new System.Windows.Forms.Label();
            this.labName = new System.Windows.Forms.Label();
            this.labID = new System.Windows.Forms.Label();
            this.labHasPlot = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMenu = new System.Windows.Forms.Button();
            this.btnImportData = new System.Windows.Forms.Button();
            this.btnAddPlot = new System.Windows.Forms.Button();
            this.btnSaveData = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.disableRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.wiresharkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.diffProfilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixProfileFileNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeTextInfoForAllProfilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProfilesFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Panel2.Controls.Add(this.panel5);
            this.splitContainer1.Panel2.Controls.Add(this.listData);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Resize += new System.EventHandler(this.splitContainer1_Panel2_Resize);
            this.splitContainer1.Size = new System.Drawing.Size(1109, 739);
            this.splitContainer1.SplitterDistance = 118;
            this.splitContainer1.SplitterWidth = 7;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.labelTopR);
            this.splitContainer2.Panel1.Controls.Add(this.labelTopL);
            this.splitContainer2.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer2_Panel1_Paint);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.labelBotR);
            this.splitContainer2.Panel2.Controls.Add(this.labelBotL);
            this.splitContainer2.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer2_Panel2_Paint);
            this.splitContainer2.Size = new System.Drawing.Size(118, 739);
            this.splitContainer2.SplitterDistance = 374;
            this.splitContainer2.TabIndex = 0;
            // 
            // labelTopR
            // 
            this.labelTopR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTopR.BackColor = System.Drawing.SystemColors.Window;
            this.labelTopR.Location = new System.Drawing.Point(41, 0);
            this.labelTopR.Name = "labelTopR";
            this.labelTopR.Size = new System.Drawing.Size(74, 18);
            this.labelTopR.TabIndex = 1;
            this.labelTopR.Text = "label2";
            this.labelTopR.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelTopL
            // 
            this.labelTopL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTopL.BackColor = System.Drawing.SystemColors.Window;
            this.labelTopL.Location = new System.Drawing.Point(0, 0);
            this.labelTopL.Name = "labelTopL";
            this.labelTopL.Size = new System.Drawing.Size(45, 18);
            this.labelTopL.TabIndex = 0;
            this.labelTopL.Text = "label1";
            // 
            // labelBotR
            // 
            this.labelBotR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBotR.BackColor = System.Drawing.SystemColors.Window;
            this.labelBotR.Location = new System.Drawing.Point(42, 0);
            this.labelBotR.Name = "labelBotR";
            this.labelBotR.Size = new System.Drawing.Size(74, 18);
            this.labelBotR.TabIndex = 3;
            this.labelBotR.Text = "label2";
            this.labelBotR.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelBotL
            // 
            this.labelBotL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBotL.BackColor = System.Drawing.SystemColors.Window;
            this.labelBotL.Location = new System.Drawing.Point(1, 0);
            this.labelBotL.Name = "labelBotL";
            this.labelBotL.Size = new System.Drawing.Size(45, 18);
            this.labelBotL.TabIndex = 2;
            this.labelBotL.Text = "label1";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.radioFlow);
            this.panel4.Controls.Add(this.radioWeight);
            this.panel4.Controls.Add(this.txtCopy);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 695);
            this.panel4.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(984, 44);
            this.panel4.TabIndex = 6;
            // 
            // radioFlow
            // 
            this.radioFlow.AutoSize = true;
            this.radioFlow.Checked = true;
            this.radioFlow.Location = new System.Drawing.Point(469, 12);
            this.radioFlow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioFlow.Name = "radioFlow";
            this.radioFlow.Size = new System.Drawing.Size(56, 22);
            this.radioFlow.TabIndex = 40;
            this.radioFlow.TabStop = true;
            this.radioFlow.Text = "Flow";
            this.radioFlow.UseVisualStyleBackColor = true;
            this.radioFlow.CheckedChanged += new System.EventHandler(this.radioFlow_CheckedChanged);
            // 
            // radioWeight
            // 
            this.radioWeight.AutoSize = true;
            this.radioWeight.Location = new System.Drawing.Point(387, 12);
            this.radioWeight.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioWeight.Name = "radioWeight";
            this.radioWeight.Size = new System.Drawing.Size(70, 22);
            this.radioWeight.TabIndex = 39;
            this.radioWeight.Text = "Weight";
            this.radioWeight.UseVisualStyleBackColor = true;
            // 
            // txtCopy
            // 
            this.txtCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCopy.Location = new System.Drawing.Point(942, 11);
            this.txtCopy.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.txtCopy.Name = "txtCopy";
            this.txtCopy.Size = new System.Drawing.Size(43, 26);
            this.txtCopy.TabIndex = 38;
            this.txtCopy.Visible = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnSaveNotes);
            this.panel3.Controls.Add(this.txtFilterName);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 605);
            this.panel3.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(984, 44);
            this.panel3.TabIndex = 5;
            // 
            // btnSaveNotes
            // 
            this.btnSaveNotes.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSaveNotes.Location = new System.Drawing.Point(896, 0);
            this.btnSaveNotes.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnSaveNotes.Name = "btnSaveNotes";
            this.btnSaveNotes.Size = new System.Drawing.Size(88, 44);
            this.btnSaveNotes.TabIndex = 39;
            this.btnSaveNotes.TabStop = false;
            this.btnSaveNotes.Text = "TODO";
            this.btnSaveNotes.UseVisualStyleBackColor = true;
            // 
            // txtFilterName
            // 
            this.txtFilterName.Location = new System.Drawing.Point(5, 8);
            this.txtFilterName.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.txtFilterName.Name = "txtFilterName";
            this.txtFilterName.Size = new System.Drawing.Size(105, 26);
            this.txtFilterName.TabIndex = 33;
            this.txtFilterName.TextChanged += new System.EventHandler(this.txtFilterName_TextChanged);
            this.txtFilterName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilterName_KeyDown);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.labNotes);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 565);
            this.panel5.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(984, 40);
            this.panel5.TabIndex = 4;
            // 
            // labNotes
            // 
            this.labNotes.BackColor = System.Drawing.SystemColors.Window;
            this.labNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labNotes.Location = new System.Drawing.Point(0, 0);
            this.labNotes.Name = "labNotes";
            this.labNotes.Size = new System.Drawing.Size(984, 40);
            this.labNotes.TabIndex = 0;
            this.labNotes.Text = "Notes:";
            this.labNotes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listData
            // 
            this.listData.Dock = System.Windows.Forms.DockStyle.Top;
            this.listData.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listData.FormattingEnabled = true;
            this.listData.IntegralHeight = false;
            this.listData.ItemHeight = 24;
            this.listData.Items.AddRange(new object[] {
            "x",
            "y"});
            this.listData.Location = new System.Drawing.Point(0, 87);
            this.listData.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.listData.Name = "listData";
            this.listData.Size = new System.Drawing.Size(984, 478);
            this.listData.TabIndex = 3;
            this.listData.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listData_DrawItem);
            this.listData.SelectedIndexChanged += new System.EventHandler(this.listData_SelectedIndexChanged);
            this.listData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listData_MouseDown);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labProfile);
            this.panel2.Controls.Add(this.labTime);
            this.panel2.Controls.Add(this.labGrind);
            this.panel2.Controls.Add(this.labRatio);
            this.panel2.Controls.Add(this.labCoffeeWeight);
            this.panel2.Controls.Add(this.labBeanWeight);
            this.panel2.Controls.Add(this.labDate);
            this.panel2.Controls.Add(this.labName);
            this.panel2.Controls.Add(this.labID);
            this.panel2.Controls.Add(this.labHasPlot);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 53);
            this.panel2.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(984, 34);
            this.panel2.TabIndex = 2;
            // 
            // labProfile
            // 
            this.labProfile.Dock = System.Windows.Forms.DockStyle.Left;
            this.labProfile.Location = new System.Drawing.Point(498, 0);
            this.labProfile.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labProfile.Name = "labProfile";
            this.labProfile.Size = new System.Drawing.Size(127, 34);
            this.labProfile.TabIndex = 7;
            this.labProfile.Text = "Profile";
            this.labProfile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labTime
            // 
            this.labTime.Dock = System.Windows.Forms.DockStyle.Left;
            this.labTime.Location = new System.Drawing.Point(456, 0);
            this.labTime.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labTime.Name = "labTime";
            this.labTime.Size = new System.Drawing.Size(42, 34);
            this.labTime.TabIndex = 5;
            this.labTime.Text = "Time";
            this.labTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labGrind
            // 
            this.labGrind.Dock = System.Windows.Forms.DockStyle.Left;
            this.labGrind.Location = new System.Drawing.Point(406, 0);
            this.labGrind.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labGrind.Name = "labGrind";
            this.labGrind.Size = new System.Drawing.Size(50, 34);
            this.labGrind.TabIndex = 4;
            this.labGrind.Text = "Grind";
            this.labGrind.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labRatio
            // 
            this.labRatio.Dock = System.Windows.Forms.DockStyle.Left;
            this.labRatio.Location = new System.Drawing.Point(350, 0);
            this.labRatio.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labRatio.Name = "labRatio";
            this.labRatio.Size = new System.Drawing.Size(56, 34);
            this.labRatio.TabIndex = 6;
            this.labRatio.Text = "Ratio";
            this.labRatio.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labCoffeeWeight
            // 
            this.labCoffeeWeight.Dock = System.Windows.Forms.DockStyle.Left;
            this.labCoffeeWeight.Location = new System.Drawing.Point(288, 0);
            this.labCoffeeWeight.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labCoffeeWeight.Name = "labCoffeeWeight";
            this.labCoffeeWeight.Size = new System.Drawing.Size(62, 34);
            this.labCoffeeWeight.TabIndex = 3;
            this.labCoffeeWeight.Text = "Coffee";
            this.labCoffeeWeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labBeanWeight
            // 
            this.labBeanWeight.Dock = System.Windows.Forms.DockStyle.Left;
            this.labBeanWeight.Location = new System.Drawing.Point(243, 0);
            this.labBeanWeight.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labBeanWeight.Name = "labBeanWeight";
            this.labBeanWeight.Size = new System.Drawing.Size(45, 34);
            this.labBeanWeight.TabIndex = 1;
            this.labBeanWeight.Text = "Bean";
            this.labBeanWeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labDate
            // 
            this.labDate.Dock = System.Windows.Forms.DockStyle.Left;
            this.labDate.Location = new System.Drawing.Point(158, 0);
            this.labDate.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labDate.Name = "labDate";
            this.labDate.Size = new System.Drawing.Size(85, 34);
            this.labDate.TabIndex = 0;
            this.labDate.Text = "Date";
            this.labDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labName
            // 
            this.labName.Dock = System.Windows.Forms.DockStyle.Left;
            this.labName.Location = new System.Drawing.Point(76, 0);
            this.labName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(82, 34);
            this.labName.TabIndex = 2;
            this.labName.Text = "Name";
            this.labName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labID
            // 
            this.labID.Dock = System.Windows.Forms.DockStyle.Left;
            this.labID.Location = new System.Drawing.Point(13, 0);
            this.labID.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labID.Name = "labID";
            this.labID.Size = new System.Drawing.Size(63, 34);
            this.labID.TabIndex = 11;
            this.labID.Text = "#";
            this.labID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labHasPlot
            // 
            this.labHasPlot.Dock = System.Windows.Forms.DockStyle.Left;
            this.labHasPlot.Location = new System.Drawing.Point(0, 0);
            this.labHasPlot.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labHasPlot.Name = "labHasPlot";
            this.labHasPlot.Size = new System.Drawing.Size(13, 34);
            this.labHasPlot.TabIndex = 8;
            this.labHasPlot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnMenu);
            this.panel1.Controls.Add(this.btnImportData);
            this.panel1.Controls.Add(this.btnAddPlot);
            this.panel1.Controls.Add(this.btnSaveData);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(984, 53);
            this.panel1.TabIndex = 1;
            // 
            // btnMenu
            // 
            this.btnMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMenu.Location = new System.Drawing.Point(892, 0);
            this.btnMenu.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(88, 50);
            this.btnMenu.TabIndex = 15;
            this.btnMenu.TabStop = false;
            this.btnMenu.Text = ". . .";
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.BtnMenu_Click);
            // 
            // btnImportData
            // 
            this.btnImportData.Location = new System.Drawing.Point(5, 0);
            this.btnImportData.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnImportData.Name = "btnImportData";
            this.btnImportData.Size = new System.Drawing.Size(88, 50);
            this.btnImportData.TabIndex = 14;
            this.btnImportData.TabStop = false;
            this.btnImportData.Text = "Import";
            this.btnImportData.UseVisualStyleBackColor = true;
            this.btnImportData.Click += new System.EventHandler(this.btnImportData_Click);
            // 
            // btnAddPlot
            // 
            this.btnAddPlot.Location = new System.Drawing.Point(103, 0);
            this.btnAddPlot.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnAddPlot.Name = "btnAddPlot";
            this.btnAddPlot.Size = new System.Drawing.Size(91, 50);
            this.btnAddPlot.TabIndex = 11;
            this.btnAddPlot.TabStop = false;
            this.btnAddPlot.Text = "Add plot";
            this.btnAddPlot.UseVisualStyleBackColor = true;
            this.btnAddPlot.Click += new System.EventHandler(this.btnAddPlot_Click);
            // 
            // btnSaveData
            // 
            this.btnSaveData.Location = new System.Drawing.Point(203, 0);
            this.btnSaveData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSaveData.Name = "btnSaveData";
            this.btnSaveData.Size = new System.Drawing.Size(88, 50);
            this.btnSaveData.TabIndex = 10;
            this.btnSaveData.TabStop = false;
            this.btnSaveData.Text = "Save data";
            this.btnSaveData.UseVisualStyleBackColor = true;
            this.btnSaveData.Click += new System.EventHandler(this.btnSaveData_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.disableRecordToolStripMenuItem,
            this.toolStripMenuItem2,
            this.wiresharkToolStripMenuItem,
            this.toolStripMenuItem1,
            this.diffProfilesToolStripMenuItem,
            this.fixProfileFileNamesToolStripMenuItem,
            this.writeTextInfoForAllProfilesToolStripMenuItem,
            this.openProfilesFolderToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(225, 148);
            // 
            // disableRecordToolStripMenuItem
            // 
            this.disableRecordToolStripMenuItem.Name = "disableRecordToolStripMenuItem";
            this.disableRecordToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.disableRecordToolStripMenuItem.Text = "Disable record";
            this.disableRecordToolStripMenuItem.Click += new System.EventHandler(this.DisableRecordToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(221, 6);
            // 
            // wiresharkToolStripMenuItem
            // 
            this.wiresharkToolStripMenuItem.Name = "wiresharkToolStripMenuItem";
            this.wiresharkToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.wiresharkToolStripMenuItem.Text = "Wireshark";
            this.wiresharkToolStripMenuItem.Click += new System.EventHandler(this.WiresharkToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(221, 6);
            // 
            // diffProfilesToolStripMenuItem
            // 
            this.diffProfilesToolStripMenuItem.Name = "diffProfilesToolStripMenuItem";
            this.diffProfilesToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.diffProfilesToolStripMenuItem.Text = "Diff profiles";
            this.diffProfilesToolStripMenuItem.Click += new System.EventHandler(this.DiffProfilesToolStripMenuItem_Click);
            // 
            // fixProfileFileNamesToolStripMenuItem
            // 
            this.fixProfileFileNamesToolStripMenuItem.Name = "fixProfileFileNamesToolStripMenuItem";
            this.fixProfileFileNamesToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.fixProfileFileNamesToolStripMenuItem.Text = "Fix profile file names";
            this.fixProfileFileNamesToolStripMenuItem.Click += new System.EventHandler(this.FixProfileFileNamesToolStripMenuItem_Click);
            // 
            // writeTextInfoForAllProfilesToolStripMenuItem
            // 
            this.writeTextInfoForAllProfilesToolStripMenuItem.Name = "writeTextInfoForAllProfilesToolStripMenuItem";
            this.writeTextInfoForAllProfilesToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.writeTextInfoForAllProfilesToolStripMenuItem.Text = "Write text info for all profiles";
            this.writeTextInfoForAllProfilesToolStripMenuItem.Click += new System.EventHandler(this.writeTextInfoForAllProfilesToolStripMenuItem_Click);
            // 
            // openProfilesFolderToolStripMenuItem
            // 
            this.openProfilesFolderToolStripMenuItem.Name = "openProfilesFolderToolStripMenuItem";
            this.openProfilesFolderToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.openProfilesFolderToolStripMenuItem.Text = "Open profiles folder";
            this.openProfilesFolderToolStripMenuItem.Click += new System.EventHandler(this.openProfilesFolderToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1109, 739);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Form1";
            this.Text = "DE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ListBox listData;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labTime;
        private System.Windows.Forms.Label labGrind;
        private System.Windows.Forms.Label labCoffeeWeight;
        private System.Windows.Forms.Label labBeanWeight;
        private System.Windows.Forms.Label labDate;
        private System.Windows.Forms.Label labName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labRatio;
        private System.Windows.Forms.Button btnSaveData;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtFilterName;
        private System.Windows.Forms.Label labNotes;
        private System.Windows.Forms.Label labProfile;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label labHasPlot;
        private System.Windows.Forms.Button btnAddPlot;
        private System.Windows.Forms.Button btnImportData;
        private System.Windows.Forms.TextBox txtCopy;
        private System.Windows.Forms.RadioButton radioFlow;
        private System.Windows.Forms.RadioButton radioWeight;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label labID;
        private System.Windows.Forms.Label labelTopL;
        private System.Windows.Forms.Label labelTopR;
        private System.Windows.Forms.Button btnSaveNotes;
        private System.Windows.Forms.Label labelBotR;
        private System.Windows.Forms.Label labelBotL;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem disableRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem wiresharkToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem diffProfilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixProfileFileNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem writeTextInfoForAllProfilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProfilesFolderToolStripMenuItem;
    }
}
