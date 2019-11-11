using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DE1LogView
{
    public partial class Form1 : Form
    {
        string Revision = "DE1 Log View v1.7";
        string ApplicationDirectory = "";
        string ApplicationNameNoExt = "";

        // to draw GBR bars in listData_DrawItem
        readonly int _BP1 = 20;
        readonly int _BP2 = 30;

        // these are used to color-code values, in listData_DrawItem only
        readonly double _MIN_R = 1.5;
        readonly double _RANGE_R = 1.9;

        GraphPainter GraphTop = null;
        GraphPainter GraphBot = null;

        FormBigPlot FormBigPlot = new FormBigPlot();

        List<int> WeightPoints = new List<int>();

        public Form1()
        {
            InitializeComponent();

            // TODO: make this configurable
            WeightPoints.Add(_BP1);
            WeightPoints.Add(_BP2);

            HeatMapR = getHeatmap(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = Revision;

            FormBigPlot.parent = this;

            GraphTop = new GraphPainter(splitContainer2.Panel1, this.Font);
            GraphBot = new GraphPainter(splitContainer2.Panel2, this.Font);

            ApplicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            ApplicationNameNoExt = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            LoadSettings();


            string data_fname = (Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory) + "\\" + ApplicationNameNoExt + ".csv";
            string old_data_fname = (Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory) + "\\CoffeeLogger.csv";
            if ((File.Exists(data_fname) == false) && (File.Exists(old_data_fname) == true))
                ReadOldFileFormat(old_data_fname);
            else if (File.Exists(data_fname))
                ReadAllRecords(data_fname);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void listData_SelectedIndexChanged(object sender, EventArgs e)
        {
            listData.Refresh();

            if (listData.SelectedIndex < 0 || listData.SelectedIndex >= listData.Items.Count)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
            {
                FirstPlotKey = "";
                return;
            }

            FirstPlotKey = key;

            txtNotes.Text = String.IsNullOrEmpty(Data[key].notes) ? "" : Data[key].notes;

            PlotDataRec(GraphTop, Data[key]);
        }

        public void PlotDataRec(GraphPainter gp, DataStruct ds)
        {
            if (gp == GraphBot)
            {
                labelBotL.Text = ds.name + "  " + ds.profile;
                labelBotR.Text = ds.coffee_weight.ToString() + "g";
            }
            else
            {
                labelTopL.Text = ds.name + "  " + ds.profile;
                labelTopR.Text = ds.coffee_weight.ToString() + "g";
            }

            gp.SetAxisTitles("", "");

            gp.data.Clear();

            gp.SetData(0, ds.elapsed, ds.flow_goal, Color.Blue, 2, DashStyle.Dash);
            gp.SetData(1, ds.elapsed, ds.pressure_goal, Color.LimeGreen, 2, DashStyle.Dash);

            gp.SetData(2, ds.elapsed, ds.flow, Color.Blue, 3, DashStyle.Solid);
            gp.SetData(3, ds.elapsed, ds.pressure, Color.LimeGreen, 3, DashStyle.Solid);

            gp.SetData(4, ds.elapsed, ds.flow_weight, Color.Brown, 3, DashStyle.Solid);

            gp.SetAutoLimits();

            gp.panel.Refresh();
        }

        private void listData_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= listData.Items.Count)
                return;

            string key = (string)listData.Items[e.Index];

            if (!Data.ContainsKey(key))
                return;

            DataStruct d = Data[key];

            Brush myBrush = Brushes.Black;
            if (listData.GetSelected(e.Index))
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            else
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            Rectangle myrec = e.Bounds;

            myrec.Y += 5;  // move the text a bit down

            myrec.X = labName.Left; myrec.Width = labName.Width;
            e.Graphics.DrawString(d.name, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labID.Left; myrec.Width = labID.Width;
            e.Graphics.DrawString(d.id.ToString(), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labDate.Left; myrec.Width = labDate.Width;

            var dn = DateTime.Now;
            var dn1 = new DateTime(dn.Year, dn.Month, dn.Day, d.date.Hour, d.date.Minute, d.date.Second);

            TimeSpan ts = dn1 - d.date;
            string ddd = "";
            if (ts.TotalDays < 1)
                ddd = "T0 " + d.date.ToString("HH:mm");
            else
            {
                var total = ts.TotalDays;

                int years = (int)(total / 365.0);
                total -= years * 365;

                int months = (int)(total / 30.0);
                total -= months * 30;

                ddd = (years == 0 ? "" : years.ToString() + "y") + (months == 0 ? "" : months.ToString() + "m") + total.ToString("0") + "d";

                if (years == 0 && months == 0)
                    ddd += " " + d.date.ToString("HH:mm");
            }

            e.Graphics.DrawString(ddd, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labBeanWeight.Left; myrec.Width = labBeanWeight.Width;
            e.Graphics.DrawString(d.bean_weight.ToString("0.0").PadLeft(4), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labCoffeeWeight.Left; myrec.Width = labCoffeeWeight.Width;
            e.Graphics.DrawString(d.coffee_weight.ToString("0.0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labRatio.Left; myrec.Width = labRatio.Width;

            var ratio = (d.coffee_weight / d.bean_weight);
                var c = getHeatmapColor((ratio - _MIN_R) / _RANGE_R, HeatMapR);
                e.Graphics.FillRectangle(c, myrec);

            e.Graphics.DrawString(ratio.ToString("0.00"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labGrind.Left; myrec.Width = labGrind.Width;
            e.Graphics.DrawString(d.grind, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labTime.Left; myrec.Width = labTime.Width;
            e.Graphics.DrawString(d.shot_time.ToString(), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labProfile.Left; myrec.Width = labProfile.Width;
            e.Graphics.DrawString(d.profile, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.Y -= 5; // adjust back

            // plot color.  0th plot is the current one, plotted as Blue
            myrec.X = labHasPlot.Left + 2; myrec.Width = labHasPlot.Width - 5;
            if (listData.GetSelected(e.Index))
                e.Graphics.FillRectangle(Brushes.Blue, myrec);
            else
                e.Graphics.FillRectangle(key == SecondPlotKey ? Brushes.Red : Brushes.White, myrec);
        }
        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (GraphTop != null)
                GraphTop.Plot(e.Graphics);
        }
        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {
            if (GraphBot != null)
                GraphBot.Plot(e.Graphics);
        }

        private void txtFilterName_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            List<string> sorted_keys = new List<string>();

            var flt_name = txtFilterName.Text.Trim().ToLower();

            foreach (var key in Data.Keys)
            {
                if (!String.IsNullOrEmpty(flt_name) && Data[key].name.Contains(flt_name) == false)
                    continue;

                if (!Data[key].enabled)
                    continue;

                sorted_keys.Add(key);
            }

            sorted_keys.Sort();


            string saved_key = "";
            if (listData.SelectedIndex != -1)
                saved_key = (string)listData.Items[listData.SelectedIndex];

            listData.Items.Clear();
            bool saved_key_set = false;
            for (int i = sorted_keys.Count - 1; i >= 0; i--)
            {
                listData.Items.Add(sorted_keys[i]);
                if (sorted_keys[i] == saved_key)
                {
                    listData.SelectedItem = sorted_keys[i];
                    saved_key_set = true;
                }
            }
            if (!saved_key_set && listData.Items.Count != 0)
                listData.SelectedIndex = 0;

            listData.Refresh();
        }

        private void txtFilterName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
                txtFilterName.Text = "";
        }

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            listData.Height = splitContainer1.Panel2.Height - panel1.Height - panel2.Height - panel3.Height
            - panel4.Height - panel5.Height - 5;
        }

        public string FirstPlotKey = "";
        public string SecondPlotKey = "";

        private void btnAddPlot_Click(object sender, EventArgs e)
        {
            if (listData.SelectedIndex != -1)
                AddPlot(listData.SelectedIndex);
        }

        private void AddPlot(int index)
        {
            string key = (string)listData.Items[index];

            if (!Data.ContainsKey(key))
            {
                SecondPlotKey = "";
                return;
            }

            if (SecondPlotKey == key)
                return;

            SecondPlotKey = key;

            PlotDataRec(GraphBot, Data[key]);

            listData.Focus();
            listData.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyValue == 67)  // Ctrl C
                    CopyLine();
                if (e.KeyValue == 68)  // Ctrl D - big plot, incl diff plots
                    bigDiffPlotCtrlDToolStripMenuItem_Click(null, EventArgs.Empty);
                if (e.KeyValue == 80)  // Ctrl P - show/diff profiles
                    DiffProfilesToolStripMenuItem_Click(null, EventArgs.Empty);
            }
            else if (e.KeyValue == 112) // F1
            { }
            else if (e.KeyValue == 123) // F12
            { }
        }

        private void CopyLine()
        {
            if (listData.SelectedIndex < 0 || listData.SelectedIndex >= listData.Items.Count)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
                return;

            var d = Data[key];

            StringBuilder sb = new StringBuilder();

            sb.Append("#" + d.id.ToString() + " " + d.date.ToString("MMM:y").Replace(":", "'") + " ");

            string t = "";
            sb.Append(t + ": ");
            sb.Append(d.bean_weight.ToString() + " -> ");
            sb.Append(d.coffee_weight.ToString("0.0") + " in ");
            sb.Append(d.shot_time.ToString() + " sec, ratio ");
            sb.Append((d.coffee_weight / d.bean_weight).ToString("0.00") + " grind ");
            sb.Append(d.grind + " press ");

            txtCopy.Text = sb.ToString();
            txtCopy.SelectAll();
            txtCopy.Copy();
        }

        private void listData_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listData.IndexFromPoint(e.Location);
                if (index != -1)
                    AddPlot(index);
            }
        }

        private void btnSaveData_Click(object sender, EventArgs e) // TODO
        {
            List<string> sorted_keys = new List<string>();
            foreach (var key in Data.Keys)
                sorted_keys.Add(key);

            sorted_keys.Sort();

            StringBuilder sb = new StringBuilder();

            foreach (var key in sorted_keys)
                Data[key].WriteRecord(sb);

            string data_fname = (Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory) + "\\" + ApplicationNameNoExt + ".csv";
            File.WriteAllText(data_fname, sb.ToString());
        }

        private void BtnMenu_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(btnMenu, new Point(0, btnMenu.Size.Height), ToolStripDropDownDirection.BelowLeft);
        }

        private void DisableRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listData.SelectedIndex == -1)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
                return;

            Data[key].enabled = false;

            FilterData();
        }

        private void WiresharkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fname = @"D:\platform-tools\__data\7_de1_1\ws_output5.txt";
            ConvertWireshark(fname);
        }

        private void DiffProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Data.ContainsKey(FirstPlotKey))
                return;

            var profile_name = Data[FirstPlotKey].profile;

            string fname = ProfilesFolder + "\\" + profile_name + ".tcl";
            if (!File.Exists(fname))
            {
                MessageBox.Show("Profile file does not exist: " + fname);
                return;
            }

            string temp_fname1 = ApplicationDirectory + "\\tmp1.txt";
            File.WriteAllText(temp_fname1, GetProfileInfo(fname));

            // Check if SelectedPlots exists and different from the main selection
            if(SecondPlotKey != FirstPlotKey && Data.ContainsKey(SecondPlotKey))
            {
                var profile_name2 = Data[SecondPlotKey].profile;

                string fname2 = ProfilesFolder + "\\" + profile_name2 + ".tcl";
                if (!File.Exists(fname2))
                {
                    MessageBox.Show("Profile file does not exist: " + fname2);
                    return;
                }

                string temp_fname2 = ApplicationDirectory + "\\tmp2.txt";
                File.WriteAllText(temp_fname2, GetProfileInfo(fname2));

                string _DiffTool = @"D:\Program Files\Beyond Compare 4\BCompare.exe";
                if (File.Exists(_DiffTool))
                {
                    try
                    {

                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.StartInfo.FileName = _DiffTool;
                        proc.StartInfo.Arguments = temp_fname1 + " " + temp_fname2;
                        proc.StartInfo.WorkingDirectory = ApplicationDirectory;
                        proc.Start();
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("Error running diff", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    }
                }
                else
                {
                    try
                    {
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.StartInfo.FileName = temp_fname1;
                        proc.StartInfo.UseShellExecute = true;
                        proc.Start();

                        System.Diagnostics.Process proc2 = new System.Diagnostics.Process();
                        proc2.StartInfo.FileName = temp_fname2;
                        proc2.StartInfo.UseShellExecute = true;
                        proc2.Start();
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("Error opening file " + temp_fname1, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    }
                }
            }
            else
            {
                try
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = temp_fname1;
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Error opening file " + temp_fname1, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
            }
        }

        string GetProfileName(string fname)
        {
            var lines = File.ReadAllLines(fname);
            foreach (var line in lines)
            {
                if (!line.Trim().StartsWith("profile_title {"))
                    continue;

                return line.Trim().Replace("profile_title {", "").Replace("}", "").Trim();
            }
            return Path.GetFileNameWithoutExtension(fname);
        }

        private void FixProfileFileNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fnames = Directory.GetFiles(ProfilesFolder, "*.tcl", SearchOption.TopDirectoryOnly);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Files fixed:");
            foreach (var fname in fnames)
            {
                var new_fname = GetProfileName(fname) + ".tcl";
                var old_fname = Path.GetFileNameWithoutExtension(fname) + ".tcl";

                if (new_fname != old_fname)
                {
                    sb.AppendLine(old_fname + " -> " + new_fname);

                    var txt = File.ReadAllText(fname);
                    File.Delete(fname);
                    File.WriteAllText(Path.GetDirectoryName(fname) + "\\" + new_fname, txt);
                }
            }

            MessageBox.Show(sb.ToString());
        }

        private void openProfilesFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = ProfilesFolder;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Error opening Profiles Folder", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void writeTextInfoForAllProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fnames = Directory.GetFiles(ProfilesFolder, "*.tcl", SearchOption.TopDirectoryOnly);

            var folder_name = ApplicationDirectory + "\\profile_info";
            if (!Directory.Exists(folder_name))
                Directory.CreateDirectory(folder_name);

            var fnames_in_output_folder = Directory.GetFiles(folder_name, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (var f in fnames_in_output_folder)
                File.Delete(f);

            foreach (var fname in fnames)
            {
                var output_fname = folder_name + "\\" + Path.GetFileNameWithoutExtension(fname) + ".txt";
                File.WriteAllText(output_fname, GetProfileInfo(fname));
            }

            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = folder_name;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Error opening Folder " + folder_name, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void btnSaveNotes_Click(object sender, EventArgs e)
        {
            if (listData.SelectedIndex < 0 || listData.SelectedIndex >= listData.Items.Count)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
                return;

            Data[key].notes = txtNotes.Text.Replace(",", " ");
        }

        private void bigDiffPlotCtrlDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormBigPlot == null)
                FormBigPlot = new FormBigPlot();

            FormBigPlot.Show();
        }
    }
}