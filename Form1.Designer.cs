namespace CoffeeLogger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel4 = new System.Windows.Forms.Panel();
            this.radioFlow = new System.Windows.Forms.RadioButton();
            this.radioWeight = new System.Windows.Forms.RadioButton();
            this.txtCopy = new System.Windows.Forms.TextBox();
            this.txtFltImportGrind = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFltImportName = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chkNoPreinf = new System.Windows.Forms.CheckBox();
            this.btnDisableRec = new System.Windows.Forms.Button();
            this.radioAll = new System.Windows.Forms.RadioButton();
            this.radioFlt = new System.Windows.Forms.RadioButton();
            this.radioAm = new System.Windows.Forms.RadioButton();
            this.radioEspro = new System.Windows.Forms.RadioButton();
            this.txtFilterName = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.labNotes = new System.Windows.Forms.Label();
            this.listData = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labPressure = new System.Windows.Forms.Label();
            this.labGBR = new System.Windows.Forms.Label();
            this.labTime = new System.Windows.Forms.Label();
            this.labGrind = new System.Windows.Forms.Label();
            this.labRatio = new System.Windows.Forms.Label();
            this.labCoffeeWeight = new System.Windows.Forms.Label();
            this.labBeanWeight = new System.Windows.Forms.Label();
            this.labBrewType = new System.Windows.Forms.Label();
            this.labDate = new System.Windows.Forms.Label();
            this.labName = new System.Windows.Forms.Label();
            this.labID = new System.Windows.Forms.Label();
            this.labHasPlot = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnImportDataAcaiaLog = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDelPlots = new System.Windows.Forms.Button();
            this.btnAddPlot = new System.Windows.Forms.Button();
            this.btnSaveData = new System.Windows.Forms.Button();
            this.btnImportDataBrewM = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
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
            this.splitContainer1.Size = new System.Drawing.Size(1363, 1000);
            this.splitContainer1.SplitterDistance = 402;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer2_Panel1_Paint);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer2_Panel2_Paint);
            this.splitContainer2.Size = new System.Drawing.Size(402, 1000);
            this.splitContainer2.SplitterDistance = 507;
            this.splitContainer2.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.radioFlow);
            this.panel4.Controls.Add(this.radioWeight);
            this.panel4.Controls.Add(this.txtCopy);
            this.panel4.Controls.Add(this.txtFltImportGrind);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.txtFltImportName);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 955);
            this.panel4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(953, 45);
            this.panel4.TabIndex = 6;
            // 
            // radioFlow
            // 
            this.radioFlow.AutoSize = true;
            this.radioFlow.Checked = true;
            this.radioFlow.Location = new System.Drawing.Point(527, 11);
            this.radioFlow.Name = "radioFlow";
            this.radioFlow.Size = new System.Drawing.Size(59, 22);
            this.radioFlow.TabIndex = 40;
            this.radioFlow.TabStop = true;
            this.radioFlow.Text = "Flow";
            this.radioFlow.UseVisualStyleBackColor = true;
            this.radioFlow.CheckedChanged += new System.EventHandler(this.radioFlow_CheckedChanged);
            // 
            // radioWeight
            // 
            this.radioWeight.AutoSize = true;
            this.radioWeight.Location = new System.Drawing.Point(435, 11);
            this.radioWeight.Name = "radioWeight";
            this.radioWeight.Size = new System.Drawing.Size(75, 22);
            this.radioWeight.TabIndex = 39;
            this.radioWeight.Text = "Weight";
            this.radioWeight.UseVisualStyleBackColor = true;
            // 
            // txtCopy
            // 
            this.txtCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCopy.Location = new System.Drawing.Point(905, 10);
            this.txtCopy.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtCopy.Name = "txtCopy";
            this.txtCopy.Size = new System.Drawing.Size(48, 26);
            this.txtCopy.TabIndex = 38;
            this.txtCopy.Visible = false;
            // 
            // txtFltImportGrind
            // 
            this.txtFltImportGrind.Location = new System.Drawing.Point(342, 10);
            this.txtFltImportGrind.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFltImportGrind.Name = "txtFltImportGrind";
            this.txtFltImportGrind.Size = new System.Drawing.Size(72, 26);
            this.txtFltImportGrind.TabIndex = 37;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(288, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 18);
            this.label2.TabIndex = 36;
            this.label2.Text = "Grind:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 18);
            this.label1.TabIndex = 34;
            this.label1.Text = "Filter data import, Name:";
            // 
            // txtFltImportName
            // 
            this.txtFltImportName.Location = new System.Drawing.Point(188, 10);
            this.txtFltImportName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFltImportName.Name = "txtFltImportName";
            this.txtFltImportName.Size = new System.Drawing.Size(85, 26);
            this.txtFltImportName.TabIndex = 33;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.chkNoPreinf);
            this.panel3.Controls.Add(this.btnDisableRec);
            this.panel3.Controls.Add(this.radioAll);
            this.panel3.Controls.Add(this.radioFlt);
            this.panel3.Controls.Add(this.radioAm);
            this.panel3.Controls.Add(this.radioEspro);
            this.panel3.Controls.Add(this.txtFilterName);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 848);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(953, 45);
            this.panel3.TabIndex = 5;
            // 
            // chkNoPreinf
            // 
            this.chkNoPreinf.AutoSize = true;
            this.chkNoPreinf.Location = new System.Drawing.Point(466, 10);
            this.chkNoPreinf.Name = "chkNoPreinf";
            this.chkNoPreinf.Size = new System.Drawing.Size(90, 22);
            this.chkNoPreinf.TabIndex = 39;
            this.chkNoPreinf.Text = "No preinf";
            this.chkNoPreinf.UseVisualStyleBackColor = true;
            // 
            // btnDisableRec
            // 
            this.btnDisableRec.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDisableRec.Location = new System.Drawing.Point(854, 0);
            this.btnDisableRec.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDisableRec.Name = "btnDisableRec";
            this.btnDisableRec.Size = new System.Drawing.Size(99, 45);
            this.btnDisableRec.TabIndex = 38;
            this.btnDisableRec.TabStop = false;
            this.btnDisableRec.Text = "Disable record";
            this.btnDisableRec.UseVisualStyleBackColor = true;
            this.btnDisableRec.Click += new System.EventHandler(this.btnDisableRec_Click);
            // 
            // radioAll
            // 
            this.radioAll.AutoSize = true;
            this.radioAll.Location = new System.Drawing.Point(371, 10);
            this.radioAll.Name = "radioAll";
            this.radioAll.Size = new System.Drawing.Size(43, 22);
            this.radioAll.TabIndex = 37;
            this.radioAll.Text = "All";
            this.radioAll.UseVisualStyleBackColor = true;
            this.radioAll.CheckedChanged += new System.EventHandler(this.radioEspro_CheckedChanged);
            // 
            // radioFlt
            // 
            this.radioFlt.AutoSize = true;
            this.radioFlt.Location = new System.Drawing.Point(317, 10);
            this.radioFlt.Name = "radioFlt";
            this.radioFlt.Size = new System.Drawing.Size(43, 22);
            this.radioFlt.TabIndex = 36;
            this.radioFlt.Text = "Flt";
            this.radioFlt.UseVisualStyleBackColor = true;
            this.radioFlt.CheckedChanged += new System.EventHandler(this.radioEspro_CheckedChanged);
            // 
            // radioAm
            // 
            this.radioAm.AutoSize = true;
            this.radioAm.Location = new System.Drawing.Point(256, 10);
            this.radioAm.Name = "radioAm";
            this.radioAm.Size = new System.Drawing.Size(50, 22);
            this.radioAm.TabIndex = 35;
            this.radioAm.Text = "Am";
            this.radioAm.UseVisualStyleBackColor = true;
            this.radioAm.CheckedChanged += new System.EventHandler(this.radioEspro_CheckedChanged);
            // 
            // radioEspro
            // 
            this.radioEspro.AutoSize = true;
            this.radioEspro.Checked = true;
            this.radioEspro.Location = new System.Drawing.Point(179, 10);
            this.radioEspro.Name = "radioEspro";
            this.radioEspro.Size = new System.Drawing.Size(68, 22);
            this.radioEspro.TabIndex = 34;
            this.radioEspro.TabStop = true;
            this.radioEspro.Text = "Espro";
            this.radioEspro.UseVisualStyleBackColor = true;
            this.radioEspro.CheckedChanged += new System.EventHandler(this.radioEspro_CheckedChanged);
            // 
            // txtFilterName
            // 
            this.txtFilterName.Location = new System.Drawing.Point(4, 9);
            this.txtFilterName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFilterName.Name = "txtFilterName";
            this.txtFilterName.Size = new System.Drawing.Size(117, 26);
            this.txtFilterName.TabIndex = 33;
            this.txtFilterName.TextChanged += new System.EventHandler(this.txtFilterName_TextChanged);
            this.txtFilterName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilterName_KeyDown);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.labNotes);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 808);
            this.panel5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(953, 40);
            this.panel5.TabIndex = 4;
            // 
            // labNotes
            // 
            this.labNotes.BackColor = System.Drawing.SystemColors.Window;
            this.labNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labNotes.Location = new System.Drawing.Point(0, 0);
            this.labNotes.Name = "labNotes";
            this.labNotes.Size = new System.Drawing.Size(953, 40);
            this.labNotes.TabIndex = 0;
            this.labNotes.Text = "Notes:";
            this.labNotes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listData
            // 
            this.listData.Dock = System.Windows.Forms.DockStyle.Top;
            this.listData.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listData.Font = new System.Drawing.Font("Lucida Sans Typewriter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listData.FormattingEnabled = true;
            this.listData.IntegralHeight = false;
            this.listData.ItemHeight = 24;
            this.listData.Items.AddRange(new object[] {
            "x",
            "y"});
            this.listData.Location = new System.Drawing.Point(0, 87);
            this.listData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listData.Name = "listData";
            this.listData.Size = new System.Drawing.Size(953, 721);
            this.listData.TabIndex = 3;
            this.listData.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listData_DrawItem);
            this.listData.SelectedIndexChanged += new System.EventHandler(this.listData_SelectedIndexChanged);
            this.listData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listData_MouseDown);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labPressure);
            this.panel2.Controls.Add(this.labGBR);
            this.panel2.Controls.Add(this.labTime);
            this.panel2.Controls.Add(this.labGrind);
            this.panel2.Controls.Add(this.labRatio);
            this.panel2.Controls.Add(this.labCoffeeWeight);
            this.panel2.Controls.Add(this.labBeanWeight);
            this.panel2.Controls.Add(this.labBrewType);
            this.panel2.Controls.Add(this.labDate);
            this.panel2.Controls.Add(this.labName);
            this.panel2.Controls.Add(this.labID);
            this.panel2.Controls.Add(this.labHasPlot);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 53);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(953, 34);
            this.panel2.TabIndex = 2;
            // 
            // labPressure
            // 
            this.labPressure.Dock = System.Windows.Forms.DockStyle.Left;
            this.labPressure.Location = new System.Drawing.Point(600, 0);
            this.labPressure.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labPressure.Name = "labPressure";
            this.labPressure.Size = new System.Drawing.Size(80, 34);
            this.labPressure.TabIndex = 10;
            this.labPressure.Text = "Pressure";
            this.labPressure.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labGBR
            // 
            this.labGBR.Dock = System.Windows.Forms.DockStyle.Left;
            this.labGBR.Location = new System.Drawing.Point(558, 0);
            this.labGBR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labGBR.Name = "labGBR";
            this.labGBR.Size = new System.Drawing.Size(42, 34);
            this.labGBR.TabIndex = 7;
            this.labGBR.Text = "GBR";
            this.labGBR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labTime
            // 
            this.labTime.Dock = System.Windows.Forms.DockStyle.Left;
            this.labTime.Location = new System.Drawing.Point(515, 0);
            this.labTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labTime.Name = "labTime";
            this.labTime.Size = new System.Drawing.Size(43, 34);
            this.labTime.TabIndex = 5;
            this.labTime.Text = "Time";
            this.labTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labGrind
            // 
            this.labGrind.Dock = System.Windows.Forms.DockStyle.Left;
            this.labGrind.Location = new System.Drawing.Point(463, 0);
            this.labGrind.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labGrind.Name = "labGrind";
            this.labGrind.Size = new System.Drawing.Size(52, 34);
            this.labGrind.TabIndex = 4;
            this.labGrind.Text = "Grind";
            this.labGrind.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labRatio
            // 
            this.labRatio.Dock = System.Windows.Forms.DockStyle.Left;
            this.labRatio.Location = new System.Drawing.Point(412, 0);
            this.labRatio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labRatio.Name = "labRatio";
            this.labRatio.Size = new System.Drawing.Size(51, 34);
            this.labRatio.TabIndex = 6;
            this.labRatio.Text = "Ratio";
            this.labRatio.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labCoffeeWeight
            // 
            this.labCoffeeWeight.Dock = System.Windows.Forms.DockStyle.Left;
            this.labCoffeeWeight.Location = new System.Drawing.Point(351, 0);
            this.labCoffeeWeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labCoffeeWeight.Name = "labCoffeeWeight";
            this.labCoffeeWeight.Size = new System.Drawing.Size(61, 34);
            this.labCoffeeWeight.TabIndex = 3;
            this.labCoffeeWeight.Text = "Coffee";
            this.labCoffeeWeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labBeanWeight
            // 
            this.labBeanWeight.Dock = System.Windows.Forms.DockStyle.Left;
            this.labBeanWeight.Location = new System.Drawing.Point(300, 0);
            this.labBeanWeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labBeanWeight.Name = "labBeanWeight";
            this.labBeanWeight.Size = new System.Drawing.Size(51, 34);
            this.labBeanWeight.TabIndex = 1;
            this.labBeanWeight.Text = "Bean";
            this.labBeanWeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labBrewType
            // 
            this.labBrewType.Dock = System.Windows.Forms.DockStyle.Left;
            this.labBrewType.Location = new System.Drawing.Point(274, 0);
            this.labBrewType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labBrewType.Name = "labBrewType";
            this.labBrewType.Size = new System.Drawing.Size(26, 34);
            this.labBrewType.TabIndex = 9;
            this.labBrewType.Text = "*";
            this.labBrewType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labDate
            // 
            this.labDate.Dock = System.Windows.Forms.DockStyle.Left;
            this.labDate.Location = new System.Drawing.Point(178, 0);
            this.labDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labDate.Name = "labDate";
            this.labDate.Size = new System.Drawing.Size(96, 34);
            this.labDate.TabIndex = 0;
            this.labDate.Text = "Date";
            this.labDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labName
            // 
            this.labName.Dock = System.Windows.Forms.DockStyle.Left;
            this.labName.Location = new System.Drawing.Point(86, 0);
            this.labName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(92, 34);
            this.labName.TabIndex = 2;
            this.labName.Text = "Name";
            this.labName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labName.Click += new System.EventHandler(this.LabName_Click);
            // 
            // labID
            // 
            this.labID.Dock = System.Windows.Forms.DockStyle.Left;
            this.labID.Location = new System.Drawing.Point(15, 0);
            this.labID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labID.Name = "labID";
            this.labID.Size = new System.Drawing.Size(71, 34);
            this.labID.TabIndex = 11;
            this.labID.Text = "#";
            this.labID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labHasPlot
            // 
            this.labHasPlot.Dock = System.Windows.Forms.DockStyle.Left;
            this.labHasPlot.Location = new System.Drawing.Point(0, 0);
            this.labHasPlot.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labHasPlot.Name = "labHasPlot";
            this.labHasPlot.Size = new System.Drawing.Size(15, 34);
            this.labHasPlot.TabIndex = 8;
            this.labHasPlot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnImportDataAcaiaLog);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnDelPlots);
            this.panel1.Controls.Add(this.btnAddPlot);
            this.panel1.Controls.Add(this.btnSaveData);
            this.panel1.Controls.Add(this.btnImportDataBrewM);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(953, 53);
            this.panel1.TabIndex = 1;
            // 
            // btnImportDataAcaiaLog
            // 
            this.btnImportDataAcaiaLog.Location = new System.Drawing.Point(97, 0);
            this.btnImportDataAcaiaLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnImportDataAcaiaLog.Name = "btnImportDataAcaiaLog";
            this.btnImportDataAcaiaLog.Size = new System.Drawing.Size(99, 50);
            this.btnImportDataAcaiaLog.TabIndex = 14;
            this.btnImportDataAcaiaLog.TabStop = false;
            this.btnImportDataAcaiaLog.Text = "AcaiaLog";
            this.btnImportDataAcaiaLog.UseVisualStyleBackColor = true;
            this.btnImportDataAcaiaLog.Click += new System.EventHandler(this.btnImportDataAcaiaLog_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 18);
            this.label3.TabIndex = 13;
            this.label3.Text = "<Import>";
            // 
            // btnDelPlots
            // 
            this.btnDelPlots.Location = new System.Drawing.Point(310, 0);
            this.btnDelPlots.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDelPlots.Name = "btnDelPlots";
            this.btnDelPlots.Size = new System.Drawing.Size(99, 50);
            this.btnDelPlots.TabIndex = 12;
            this.btnDelPlots.TabStop = false;
            this.btnDelPlots.Text = "Del plots";
            this.btnDelPlots.UseVisualStyleBackColor = true;
            this.btnDelPlots.Click += new System.EventHandler(this.btnDelPlots_Click);
            // 
            // btnAddPlot
            // 
            this.btnAddPlot.Location = new System.Drawing.Point(244, 0);
            this.btnAddPlot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAddPlot.Name = "btnAddPlot";
            this.btnAddPlot.Size = new System.Drawing.Size(54, 50);
            this.btnAddPlot.TabIndex = 11;
            this.btnAddPlot.TabStop = false;
            this.btnAddPlot.Text = "Add plot";
            this.btnAddPlot.UseVisualStyleBackColor = true;
            this.btnAddPlot.Click += new System.EventHandler(this.btnAddPlot_Click);
            // 
            // btnSaveData
            // 
            this.btnSaveData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveData.Location = new System.Drawing.Point(851, 0);
            this.btnSaveData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSaveData.Name = "btnSaveData";
            this.btnSaveData.Size = new System.Drawing.Size(99, 50);
            this.btnSaveData.TabIndex = 10;
            this.btnSaveData.TabStop = false;
            this.btnSaveData.Text = "Save data";
            this.btnSaveData.UseVisualStyleBackColor = true;
            this.btnSaveData.Click += new System.EventHandler(this.btnSaveData_Click);
            // 
            // btnImportDataBrewM
            // 
            this.btnImportDataBrewM.Location = new System.Drawing.Point(3, 0);
            this.btnImportDataBrewM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnImportDataBrewM.Name = "btnImportDataBrewM";
            this.btnImportDataBrewM.Size = new System.Drawing.Size(20, 50);
            this.btnImportDataBrewM.TabIndex = 9;
            this.btnImportDataBrewM.TabStop = false;
            this.btnImportDataBrewM.Text = "B";
            this.btnImportDataBrewM.UseVisualStyleBackColor = true;
            this.btnImportDataBrewM.Click += new System.EventHandler(this.btnImportDataBrewM_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1363, 1000);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Coffee Logger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.Button btnImportDataBrewM;
        private System.Windows.Forms.Label labRatio;
        private System.Windows.Forms.Button btnSaveData;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtFilterName;
        private System.Windows.Forms.Label labNotes;
        private System.Windows.Forms.Label labGBR;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFltImportName;
        private System.Windows.Forms.TextBox txtFltImportGrind;
        private System.Windows.Forms.Label labHasPlot;
        private System.Windows.Forms.RadioButton radioAll;
        private System.Windows.Forms.RadioButton radioFlt;
        private System.Windows.Forms.RadioButton radioAm;
        private System.Windows.Forms.RadioButton radioEspro;
        private System.Windows.Forms.Button btnAddPlot;
        private System.Windows.Forms.Button btnDelPlots;
        private System.Windows.Forms.Button btnImportDataAcaiaLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCopy;
        private System.Windows.Forms.Label labBrewType;
        private System.Windows.Forms.RadioButton radioFlow;
        private System.Windows.Forms.RadioButton radioWeight;
        private System.Windows.Forms.Button btnDisableRec;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label labPressure;
        private System.Windows.Forms.Label labID;
        private System.Windows.Forms.CheckBox chkNoPreinf;
    }
}

