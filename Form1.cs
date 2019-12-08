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
        string Revision = "DE1 Log View v1.22";
        string ApplicationDirectory = "";
        string ApplicationNameNoExt = "";

        GraphPainter GraphTop = null;
        GraphPainter GraphBot = null;

        FormBigPlot FormBigPlot = new FormBigPlot();

        public string MainPlotKey = "";
        public string RefPlotKey = "";

        public Form1()
        {
            InitializeComponent();
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

            string bean_fname = (Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory) + "\\BeanList.csv";
            LoadBeanList(bean_fname);

            string profile_fname = (Directory.Exists(DataFolder) ? DataFolder : ApplicationDirectory) + "\\ProfileInfo.csv";
            ReadProfileInfo(profile_fname);

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
            btnSaveData_Click(null, EventArgs.Empty);
        }

        private void listData_SelectedIndexChanged(object sender, EventArgs e)
        {
            listData.Refresh();

            if (listData.SelectedIndex < 0 || listData.SelectedIndex >= listData.Items.Count)
                return;

            string key = (string)listData.Items[listData.SelectedIndex];

            if (!Data.ContainsKey(key))
            {
                MainPlotKey = "";
                return;
            }

            MainPlotKey = key;

            txtNotes.Text = String.IsNullOrEmpty(Data[key].notes) ? "" : Data[key].notes;

            PlotDataRec(GraphTop, Data[key]);
        }

        public void PlotDataRec(GraphPainter gp, DataStruct ds)
        {
            if (gp == GraphBot)
            {
                labelBotL.Text = ds.getAsInfoTextForGraph(ProfileInfoList, BeanList);
                labelBotR.Text = "";
            }
            else if (gp == GraphTop)
            {
                labelTopL.Text = ds.getAsInfoTextForGraph(ProfileInfoList, BeanList);
                labelTopR.Text = "";
            }
            else if (gp == FormBigPlot.Graph)
            {
                FormBigPlot.SetLabelText(ds.getAsInfoTextForGraph(ProfileInfoList, BeanList));
            }

            gp.SetAxisTitles("", "");

            gp.data.Clear();

            gp.SetData(0, ds.elapsed, ds.flow_goal, Color.Blue, 2, DashStyle.Dash);
            gp.SetData(1, ds.elapsed, ds.pressure_goal, Color.LimeGreen, 2, DashStyle.Dash);

            gp.SetData(2, ds.elapsed, ds.flow, Color.Blue, 3, DashStyle.Solid);
            gp.SetData(3, ds.elapsed, ds.pressure, Color.LimeGreen, 3, DashStyle.Solid);

            gp.SetData(4, ds.elapsed, ds.flow_weight, Color.Brown, 3, DashStyle.Solid);

            List<double> temperature_scaled = new List<double>();
            List<double> temperature_target_scaled = new List<double>();
            foreach (var t in ds.temperature_basket)
                temperature_scaled.Add(t / 10.0);
            foreach (var t in ds.temperature_goal)
                temperature_target_scaled.Add(t / 10.0);

            gp.SetData(5, ds.elapsed, temperature_target_scaled, Color.Red, 2, DashStyle.Dash);
            gp.SetData(6, ds.elapsed, temperature_scaled, Color.Red, 3, DashStyle.Solid);

            var pi = ds.getPreinfTime();
            List<double> x_pi = new List<double>(); x_pi.Add(pi); x_pi.Add(pi);
            List<double> y_pi = new List<double>(); y_pi.Add(0); y_pi.Add(1);
            gp.SetData(7, x_pi, y_pi, Color.Brown, 2, DashStyle.Solid);

            gp.SetAutoLimits();

            gp.panel.Refresh();
        }

        private void listData_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= listData.Items.Count)
                return;

            string key = (string)listData.Items[e.Index];

            if (!Data.ContainsKey(key))
                return;

            DataStruct d = Data[key];

            if (checkShowNotes.Checked)
            {
                if (d.notes != "")
                    e.ItemHeight = (int) (e.ItemHeight * 1.7);
            }
        }

        private string TrimStringToDraw(string s, Graphics g, Font font, int width)
        {
            string out_str = s;

            var x = g.MeasureString(out_str, font).ToSize().Width;

            while(x >= width)
            {
                out_str = out_str.Remove(out_str.Length - 1);
                x = g.MeasureString(out_str, font).ToSize().Width;
            }

            return out_str;
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
            if (e.Index % 2 == 0)
            {
                var brush = new SolidBrush(Color.FromArgb(255, 240, 240, 240));
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            else
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            Rectangle myrec = e.Bounds;

            // plot color bar.  blue is the current one, RefPlotKey is red
            myrec.X = labHasPlot.Left + 2; myrec.Width = labHasPlot.Width - 5;
            if (listData.GetSelected(e.Index))
                e.Graphics.FillRectangle(Brushes.Blue, myrec);
            else
                e.Graphics.FillRectangle(key == RefPlotKey ? Brushes.Red : Brushes.White, myrec);

            // Text. Move the text a bit down
            myrec.Y += 2;

            myrec.X = labName.Left; myrec.Width = labName.Width;
            e.Graphics.DrawString(d.name, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labProfile.Left; myrec.Width = labProfile.Width;
            e.Graphics.DrawString(d.getShortProfileName(ProfileInfoList), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labGrind.Left; myrec.Width = labGrind.Width;
            e.Graphics.DrawString(d.grind, e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labBeanWeight.Left; myrec.Width = labBeanWeight.Width;
            e.Graphics.DrawString(d.bean_weight.ToString("0.0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labKpi.Left; myrec.Width = labKpi.Width;
            e.Graphics.DrawString(d.getKpi(ProfileInfoList).ToString("0.0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labDaysSinceRoast.Left; myrec.Width = labDaysSinceRoast.Width;
            e.Graphics.DrawString(d.getAgeStr(BeanList), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labRatio.Left; myrec.Width = labRatio.Width;
            e.Graphics.DrawString(d.getRatio().ToString("0.0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labAvFlow.Left; myrec.Width = labAvFlow.Width;
            e.Graphics.DrawString(d.getAverageWeightFlow().ToString("0.0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labPI.Left; myrec.Width = labPI.Width;
            e.Graphics.DrawString(d.getPreinfTime().ToString("0"), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            myrec.X = labDate.Left; myrec.Width = labDate.Width;
            e.Graphics.DrawString(d.getNiceDateStr(DateTime.Now), e.Font, myBrush, myrec, StringFormat.GenericTypographic);

            if (checkShowNotes.Checked)
            {
                if (d.notes != "") // notes, on a separate line
                {
                    myrec.X = labGrind.Left+5; myrec.Width = e.Bounds.Width - labName.Left - 10;
                    myrec.Y += e.Bounds.Height / 2;

                    var notes_str = TrimStringToDraw(d.notes, e.Graphics, e.Font, myrec.Width);
                    if (notes_str.StartsWith("*"))
                        myrec.X -= 20;

                    Font font1 = new Font(e.Font.FontFamily, (float)(e.Font.Size * 0.7), FontStyle.Regular);
                    e.Graphics.DrawString(notes_str, font1, myBrush, myrec, StringFormat.GenericTypographic);
                }
            }
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

        List<string> SmartOutputSort(List<string> input)
        {
            List<DataStruct> list = new List<DataStruct>();
            foreach (var i in input)
                list.Add(Data[i]);

            list.Sort(delegate (DataStruct a1, DataStruct a2)
            {
                if (a1.name != a2.name) { return a2.name.CompareTo(a1.name); }
                else if (a1.profile != a2.profile) { return a2.profile.CompareTo(a1.profile); }
                else if (a1.grind != a2.grind) { return a2.grind.CompareTo(a1.grind); }
                else if (a1.bean_weight != a2.bean_weight) { return a2.bean_weight.CompareTo(a1.bean_weight); }
                else
                {
                    if (Math.Abs(a1.getRatio() - a2.getRatio()) < 0.2) return 0;
                    else return a1.getRatio().CompareTo(a2.getRatio());
                }
            });

            List<string> output_list = new List<string>();
            foreach (var x in list)
                output_list.Add(x.date_str);

            return output_list;
        }

        private void FilterData()
        {
            List<string> sorted_keys = new List<string>();

            var flt_name = txtFilterName.Text.Trim().ToLower();
            var flt_profile = txtFilterProfile.Text.Trim().ToLower();

            int max_days = int.MaxValue;
            if (comboNumItemsToShow.Text == "Show last 31 days")
                max_days = 31;
            else if (comboNumItemsToShow.Text == "Show last 90 days")
                max_days = 90;

            foreach (var key in Data.Keys)
            {
                if (!String.IsNullOrEmpty(flt_name) && Data[key].name.ToLower().Contains(flt_name) == false)
                    continue;

                if (!String.IsNullOrEmpty(flt_profile) && Data[key].getShortProfileName(ProfileInfoList).ToLower().Contains(flt_profile) == false)
                    continue;

                if (!Data[key].enabled)
                    continue;

                if ((DateTime.Now - Data[key].date).TotalDays > max_days)
                    continue;

                sorted_keys.Add(key);
            }

            if(comboSortStyle.Text == "Sort by ID")
                sorted_keys.Sort();
            else
                sorted_keys = SmartOutputSort(sorted_keys);

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

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            listData.Height = splitContainer1.Panel2.Height - panel1.Height - panel2.Height - panel3.Height
            - panel4.Height - panel5.Height - 5;
        }

        private void btnRefPlot_Click(object sender, EventArgs e)
        {
            if (listData.SelectedIndex != -1)
                RefPlot(listData.SelectedIndex);
        }

        private void RefPlot(int index)
        {
            string key = (string)listData.Items[index];

            if (!Data.ContainsKey(key))
            {
                RefPlotKey = "";
                return;
            }

            if (RefPlotKey == key)
            {
                RefPlotKey = "";
                GraphBot.data.Clear();
                splitContainer2.Panel2.Refresh();
                labelBotL.Text = "";
                listData.Refresh();
                return;
            }

            RefPlotKey = key;

            PlotDataRec(GraphBot, Data[key]);

            listData.Focus();
            listData.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyValue == 66)  // Ctrl B - bean info
                    beanInfoCtrlBF3ToolStripMenuItem_Click(null, EventArgs.Empty);
                if (e.KeyValue == 67)  // Ctrl C
                    CopyLine();
                if (e.KeyValue == 68)  // Ctrl D - big plot, incl diff plots
                    bigDiffPlotCtrlDToolStripMenuItem_Click(null, EventArgs.Empty);
                if (e.KeyValue == 80)  // Ctrl P - show/diff profiles
                    DiffProfilesToolStripMenuItem_Click(null, EventArgs.Empty);
                if (e.KeyValue == 82)  // Ctrl R - report
                    PrintReport();
                if (e.KeyValue == 83)  // Ctrl S - Save
                {
                    btnSaveData_Click(null, EventArgs.Empty);
                    MessageBox.Show("OK");
                }
            }
            else if (e.KeyValue == 112) // F1
            {
                scatterPlotForAllShownToolStripMenuItem_Click(null, EventArgs.Empty);
            }
            else if (e.KeyValue == 113) // F2
            {
                PrintReport();
            }
            else if (e.KeyValue == 114) // F3
            {
                beanInfoCtrlBF3ToolStripMenuItem_Click(null, EventArgs.Empty);
            }
            else if (e.KeyValue == 115) // F4
            {
                DiffProfilesToolStripMenuItem_Click(null, EventArgs.Empty);
            }
            else if (e.KeyValue == 116) // F5
            {
                showVideoF4ToolStripMenuItem_Click(null, EventArgs.Empty);
            }
            else if (e.KeyValue == 123) // F12
            { }
        }

        private void CopyLine()
        {
            if (!Data.ContainsKey(MainPlotKey))
                return;

            txtCopy.Text = Data[MainPlotKey].getAsInfoTextForGraph(ProfileInfoList, BeanList);
            txtCopy.SelectAll();
            txtCopy.Copy();
        }

        private void listData_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listData.IndexFromPoint(e.Location);
                if (index != -1)
                    RefPlot(index);
            }
        }

        private void btnSaveData_Click(object sender, EventArgs e)
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
            if (!Data.ContainsKey(MainPlotKey))
                return;

            var profile_name = Data[MainPlotKey].profile;

            string fname = ProfilesFolder + "\\" + profile_name + ".tcl";
            if (!File.Exists(fname))
            {
                MessageBox.Show("Profile file does not exist: " + fname);
                return;
            }

            string temp_fname1 = ApplicationDirectory + "\\tmp1.txt";
            string content_fname1 = GetProfileInfo(fname);

            // Check if SelectedPlots exists and different from the main selection
            if(RefPlotKey != MainPlotKey && Data.ContainsKey(RefPlotKey))
            {
                var profile_name2 = Data[RefPlotKey].profile;

                string fname2 = ProfilesFolder + "\\" + profile_name2 + ".tcl";
                if (!File.Exists(fname2))
                {
                    MessageBox.Show("Profile file does not exist: " + fname2);
                    return;
                }

                string temp_fname2 = ApplicationDirectory + "\\tmp2.txt";
                string content_fname2 = GetProfileInfo(fname2);

                if (content_fname2 == content_fname1)
                {
                    FormBigPlot.ShowLog(content_fname1);
                    FormBigPlot.Show();
                }
                else
                {
                    string diff_tool = @"D:\Program Files\Beyond Compare 4\BCompare.exe";
                    if (!File.Exists(diff_tool))
                        diff_tool = @"C:\Program Files\Beyond Compare 4\BCompare.exe";

                    if (File.Exists(diff_tool))
                    {
                        try
                        {
                            File.WriteAllText(temp_fname1, content_fname1);
                            File.WriteAllText(temp_fname2, content_fname2);

                            System.Diagnostics.Process proc = new System.Diagnostics.Process();
                            proc.StartInfo.FileName = diff_tool;
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
                        FormBigPlot.ShowLog(content_fname1 + "\r\n\r\n" + content_fname2);
                        FormBigPlot.Show();
                    }
                }
            }
            else
            {
                FormBigPlot.ShowLog(content_fname1);
                FormBigPlot.Show();
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

            FilterData();
        }
        private void bigDiffPlotCtrlDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormBigPlot == null)
                FormBigPlot = new FormBigPlot();

            List<string> all_keys = new List<string>();

            foreach (string s in listData.Items)
                all_keys.Add(s);

            FormBigPlot.ShowGraph(all_keys);

            FormBigPlot.Show();
        }
        void btnImportData_Click(object sender, EventArgs e)  // this comes from button
        {
            if (!Directory.Exists(ShotsFolder))
            {
                MessageBox.Show("ERROR: ShotsFolder location is not set");
                return;
            }
            var old_count = Data.Count;

            var files = Directory.GetFiles(ShotsFolder, "*.shot", SearchOption.TopDirectoryOnly);
            foreach (var f in files)
            {
                if (Path.GetFileNameWithoutExtension(f) == "0") // skip 0.shot, this is a config file for DE1Win10
                    continue;

                var key = ReadDateFromShotFile(f);
                if (Data.ContainsKey(key))
                    continue;

                if (key == "")
                {
                    MessageBox.Show("ERROR: when reading date from shot file " + f);
                    return;
                }

                if (!ImportShotFile(f))
                {
                    MessageBox.Show("ERROR: when reading shot file " + f);
                    return;
                }
            }
            FilterData();

            MessageBox.Show("Loaded " + (Data.Count - old_count).ToString() + " shot files");
        }
        private void beanInfoCtrlBF3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Data.ContainsKey(MainPlotKey))
                return;

            var text = "";
            var key = Data[MainPlotKey].name;

            
            if (BeanList.ContainsKey(key))
            {
                StringBuilder sb = new StringBuilder();

                var b = BeanList[key];

                sb.AppendLine("ShortName    " + b.ShortName);
                sb.AppendLine("FullName     " + b.FullName);
                sb.AppendLine("Country      " + b.Country);
                sb.AppendLine("CountryCode  " + b.CountryCode);
                sb.AppendLine("From         " + b.From);
                sb.AppendLine("Roasted      " + (b.Roasted == DateTime.MinValue ? "" : b.Roasted.ToString("dd/MM/yyyy")));
                sb.AppendLine("Frozen       " + (b.Frozen == DateTime.MinValue ? "" : b.Frozen.ToString("dd/MM/yyyy")));
                sb.AppendLine("Defrosted    " + (b.Defrosted == DateTime.MinValue ? "" : b.Defrosted.ToString("dd/MM/yyyy")));
                sb.AppendLine("Process      " + b.Process);
                sb.AppendLine("Varietals    " + b.Varietals);
                sb.AppendLine("Notes        " + b.Notes);
                sb.AppendLine("Cupping      " + b.Cupping);
                text = sb.ToString();
            }
            else
                text = "no bean info found";

            FormBigPlot.ShowLog(text);
            FormBigPlot.Show();
        }

        private void splitContainer2_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            var x = GraphTop.ToDataX(e.X);
            var y = GraphTop.ToDataY(e.Y);

            labelTopR.Text = x.ToString("0.0") + ", " + y.ToString("0.0");
        }

        private void splitContainer2_Panel2_MouseMove(object sender, MouseEventArgs e)
        {
            var x = GraphBot.ToDataX(e.X);
            var y = GraphBot.ToDataY(e.Y);

            labelBotR.Text = x.ToString("0.0") + ", " + y.ToString("0.0");
        }
        private void PrintReport()
        {
            StringBuilder sb = new StringBuilder();

            // get the max size for the bean and profile names
            int max_bean_len = 0;
            int max_profile_len = 0;
            foreach (string item in listData.Items)
            {
                var d = Data[item];
                max_bean_len = Math.Max(d.name.Length, max_bean_len);
                max_profile_len = Math.Max(d.getShortProfileName(ProfileInfoList).Length, max_profile_len);

            }

            List<string> keys = new List<string>();
            if (comboSortStyle.Text == "Sort by ID")
            {
                for(int i = listData.Items.Count-1; i >= 0; i--)
                {
                    keys.Add((string) listData.Items[i]);
                }
            }
            else
            {
                foreach (string item in listData.Items)
                    keys.Add(item);
            }

            if (keys.Count != 0)
            {
                var last_name = Data[keys[0]].name;
                foreach (var key in keys)
                {
                    if (last_name != Data[key].name)
                        sb.AppendLine("");

                    sb.AppendLine(Data[key].getAsInfoText(ProfileInfoList, BeanList, max_bean_len: max_bean_len, max_profile_len: max_profile_len));

                    last_name = Data[key].name;
                }
            }

            FormBigPlot.ShowLog(sb.ToString());
            FormBigPlot.Show();
        }
        private void saveDataCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnSaveData_Click(null, EventArgs.Empty);
        }

        private void txtFilterName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                if(txtFilterName.Text == "")
                    txtFilterProfile.Text = "";
                else
                    txtFilterName.Text = "";
            }
        }
        private void txtFilterProfile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                if (txtFilterProfile.Text == "")
                    txtFilterName.Text = "";
                else
                    txtFilterProfile.Text = "";
            }
        }
        private void txtFilterName_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void txtFilterProfile_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void comboNumItemsToShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void comboSortStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void checkShowNotes_CheckedChanged(object sender, EventArgs e)
        {
            FilterData();
        }
        private void showVideoF4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Data.ContainsKey(MainPlotKey))
                return;

            var id = Data[MainPlotKey].id;

            string fname = (Directory.Exists(VideoFolder) ? VideoFolder : ApplicationDirectory) + "\\" + id.ToString() + "-1.m4v";
            if (!File.Exists(fname))
                return;

            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = fname;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Error opening video", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }
        private void openVideoFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!Directory.Exists(VideoFolder))
            {
                MessageBox.Show("Video folder is not set", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }

            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = VideoFolder;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Error opening video", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }
        private void printReportCtrlPF2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintReport();
        }
        private void scatterPlotForAllShownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormBigPlot == null)
                FormBigPlot = new FormBigPlot();

            List<string> all_keys = new List<string>();

            foreach (string s in listData.Items)
                all_keys.Add(s);

            FormBigPlot.ShowScatterGraph(all_keys);

            FormBigPlot.Show();
        }
        public void SetSelected()
        {
            if(MainPlotKey != "" && RefPlotKey == "")
            {
                listData.SelectedItem = MainPlotKey;
                listData.Refresh();
            }
        }
    }
}